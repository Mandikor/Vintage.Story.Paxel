using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using Vintagestory.API.Config;

namespace Paxel
{
    public class ItemPaxelPlus : Item
    {
        SkillItem[] toolModes;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            ICoreClientAPI capi = api as ICoreClientAPI;

            toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", () =>
            {
                SkillItem[] modes;
                //if (api.World.Config.GetString("propickNodeSearchRadius").ToInt() > 0)
                //{
                modes = new SkillItem[3];
                modes[0] = new SkillItem() { Code = new AssetLocation("simple"), Name = Lang.Get("1x1x1") };
                modes[1] = new SkillItem() { Code = new AssetLocation("charged"), Name = Lang.Get("3x3x1") };
                modes[2] = new SkillItem() { Code = new AssetLocation("supercharged"), Name = Lang.Get("5x5x1") };

                //}
                //else
                //{
                //    modes = new SkillItem[1];
                //    modes[0] = new SkillItem() { Code = new AssetLocation("density"), Name = Lang.Get("Density Search Mode (Long range, chance based search)") };
                //}

                if (capi != null)
                {
                    modes[0].WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 5, ColorUtil.WhiteArgb));
                    modes[0].TexturePremultipliedAlpha = false;
                    if (modes.Length > 1)
                    {
                        modes[1].WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x3.svg"), 48, 48, 5, ColorUtil.WhiteArgb));
                        modes[1].TexturePremultipliedAlpha = false;
                    }
                    if (modes.Length > 2)
                    {
                        modes[2].WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/5x5.svg"), 48, 48, 5, ColorUtil.WhiteArgb));
                        modes[2].TexturePremultipliedAlpha = false;
                    }
                }


                return modes;
            });

        }

        //public static string SetTpHitAnim(BlockSelection blockSel, Entity byEntity, ref string __result)
        //{

        //    var block = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);

        //    __result = "breakhand";

        //    switch (block.BlockMaterial)
        //    {
        //        case (block.BlockMaterial = "Stone"):
        //            __result = "smithing";
        //            break;
        //        case (block.BlockMaterial == EnumBlockMaterial.Wood):
        //            __result = "axechop";
        //            break;

        //        case (block.BlockMaterial == EnumBlockMaterial.Soil):
        //            __result = "shoveldig";
        //            break;

        //    }

        //    return __result;
        //}

        public virtual int MultiBreakQuantity { get { return 9; } }

        public virtual bool CanMultiBreak(Block block)
        {
            return block.BlockMaterial == EnumBlockMaterial.Ceramic;
        }

        public override float OnBlockBreaking(IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter)
        {
            float newResist = base.OnBlockBreaking(player, blockSel, itemslot, remainingResistance, dt, counter);
            int leftDurability = itemslot.Itemstack.Attributes.GetInt("durability", Durability);
            DamageNearbyBlocks(player, blockSel, remainingResistance - newResist, leftDurability);

            return newResist;
        }

        private void DamageNearbyBlocks(IPlayer player, BlockSelection blockSel, float damage, int leftDurability)
        {
            Block block = player.Entity.World.BlockAccessor.GetBlock(blockSel.Position);

            if (!CanMultiBreak(block)) return;

            Vec3d hitPos = blockSel.Position.ToVec3d().Add(blockSel.HitPosition);
            OrderedDictionary<BlockPos, float> dict = GetNearblyMultibreakables(player.Entity.World, blockSel.Position, hitPos);
            var orderedPositions = dict.OrderBy(x => x.Value).Select(x => x.Key);

            int q = Math.Min(MultiBreakQuantity, leftDurability);
            foreach (var pos in orderedPositions)
            {
                if (q == 0) break;
                BlockFacing facing = BlockFacing.FromNormal(player.Entity.ServerPos.GetViewVector()).Opposite;

                if (!player.Entity.World.Claims.TryAccess(player, pos, EnumBlockAccessFlags.BuildOrBreak)) continue;

                player.Entity.World.BlockAccessor.DamageBlock(pos, facing, damage);
                q--;
            }
        }

        public override bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1)
        {
            Block block = world.BlockAccessor.GetBlock(blockSel.Position);

            base.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, dropQuantityMultiplier);

            if (byEntity as EntityPlayer == null || itemslot.Itemstack == null) return true;

            IPlayer plr = world.PlayerByUid((byEntity as EntityPlayer).PlayerUID);

            if (!CanMultiBreak(block)) return true;

            Vec3d hitPos = blockSel.Position.ToVec3d().Add(blockSel.HitPosition);
            var orderedPositions = GetNearblyMultibreakables(world, blockSel.Position, hitPos).OrderBy(x => x.Value);

            int leftDurability = itemslot.Itemstack.Attributes.GetInt("durability", Durability);
            int q = 0;


            foreach (var val in orderedPositions)
            {
                if (!plr.Entity.World.Claims.TryAccess(plr, val.Key, EnumBlockAccessFlags.BuildOrBreak)) continue;

                world.BlockAccessor.BreakBlock(val.Key, plr);
                world.BlockAccessor.MarkBlockDirty(val.Key);
                DamageItem(world, byEntity, itemslot);

                q++;
                if (q >= MultiBreakQuantity || itemslot.Itemstack == null) break;
            }

            return true;
        }

        OrderedDictionary<BlockPos, float> GetNearblyMultibreakables(IWorldAccessor world, BlockPos pos, Vec3d hitPos)
        {
            OrderedDictionary<BlockPos, float> positions = new OrderedDictionary<BlockPos, float>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (dx == 0 && dy == 0 && dz == 0) continue;

                        BlockPos dpos = pos.AddCopy(dx, dy, dz);
                        if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                        {
                            positions.Add(dpos, hitPos.SquareDistanceTo(dpos.X + 0.5, dpos.Y + 0.5, dpos.Z + 0.5));
                        }
                    }
                }
            }

            return positions;
        }

        public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
        {
            return toolModes;
        }

        public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
        {
            return Math.Min(toolModes.Length - 1, slot.Itemstack.Attributes.GetInt("toolMode"));
        }

        public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
        {
            slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
        }

        public override void OnUnloaded(ICoreAPI api)
        {
            base.OnUnloaded(api);

            for (int i = 0; toolModes != null && i < toolModes.Length; i++)
            {
                toolModes[i]?.Dispose();
            }
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[] {
                new WorldInteraction()
                {
                    ActionLangCode = Lang.Get("Change tool mode"),
                    HotKeyCodes = new string[] { "toolmodeselect" },
                    MouseButton = EnumMouseButton.None
                }
            };

        }
    }

}

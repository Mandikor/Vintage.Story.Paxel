using Cairo;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using static Paxel.ItemMiningDrill;

namespace Paxel
{
    public class ItemPaxelPowered2 : Item
    {

        public SkillItem[] toolModes;
        private string[] allowedPrefixes;
        public static readonly int HighlightSlotId = 23;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            allowedPrefixes = Attributes["codePrefixes"].AsArray<string>();

            if (api is ICoreClientAPI capi)
            {
                toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", () =>
                {
                    SkillItem[] modes = new SkillItem[7];

                    modes[0] = new SkillItem() { Code = new AssetLocation("1x1"), Name = Lang.Get("paxel:1x1") }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 0, ColorUtil.WhiteArgb));
                    modes[1] = new SkillItem() { Code = new AssetLocation("1x2"), Name = Lang.Get("paxel:1x2") }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 48, 48, 0, ColorUtil.WhiteArgb));
                    modes[2] = new SkillItem() { Code = new AssetLocation("1x3"), Name = Lang.Get("paxel:1x3") }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb));
                    modes[3] = new SkillItem() { Code = new AssetLocation("3x3"), Name = Lang.Get("paxel:3x3") }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb));
                    modes[4] = new SkillItem() { Code = new AssetLocation("5x5"), Name = Lang.Get("paxel:5x5") }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/5x5.svg"), 48, 48, 0, ColorUtil.WhiteArgb));
                    modes[5] = new SkillItem() { Code = new AssetLocation("chess"), Name = Lang.Get("paxel:Chessboard") }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/chess.svg"), 48, 48, 0, ColorUtil.WhiteArgb));
                    modes[6] = new SkillItem() { Code = new AssetLocation("veinmine"), Name = Lang.Get("paxel:Veinmining") }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/vein.svg"), 48, 48, 0, ColorUtil.WhiteArgb));

                    SkillItem[] array = modes;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].TexturePremultipliedAlpha = false;
                    }

                    return modes;
                });
            }
        }

        public override void OnUnloaded(ICoreAPI api)
        {
            for (int i = 0; toolModes != null && i < toolModes.Length; i++)
            {
                toolModes[i]?.Dispose();
            }
        }

        public override void OnHeldIdle(ItemSlot slot, EntityAgent byEntity)
        {
            base.OnHeldIdle(slot, byEntity);
            EntityPlayer entityPlayer = byEntity as EntityPlayer;
            IPlayer player = entityPlayer?.Player;
            ClearHighlights(api.World, player);
            BlockSelection currentBlockSelection = player.CurrentBlockSelection;
            bool flag = currentBlockSelection == null;
            if (!flag)
            {
                List<BlockPos> currentBlockList = GetCurrentBlockList(currentBlockSelection, slot);
                bool flag2 = currentBlockList == null || currentBlockList.Count == 0;
                if (!flag2)
                {
                    List<int> list = new();
                    for (int i = 0; i < currentBlockList.Count; i++)
                    {
                        bool flag3 = CanMine(api, currentBlockList[i]);
                        if (flag3)
                        {
                            list.Add(ColorUtil.ColorFromRgba(255, 255, 0, 32));
                        }
                        else
                        {
                            list.Add(ColorUtil.ColorFromRgba(255, 0, 0, 32));
                        }
                    }
                    api.World.HighlightBlocks(player, ItemMiningDrill.HighlightSlotId, currentBlockList, list, 0, 0, 1f);
                }
            }
        }

        public override float OnBlockBreaking(IPlayer player, BlockSelection blockSel, ItemSlot itemSlot, float remainingResistance, float dt, int counter)
        {
            return base.OnBlockBreaking(player, blockSel, itemSlot, remainingResistance, dt, counter);
        }

        public override bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 0.95f)
        {
            return base.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, dropQuantityMultiplier);
        }

        public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
        {
            if (blockSel == null) return null;

            Block block = forPlayer.Entity.World.BlockAccessor.GetBlock(blockSel.Position);

            for (int i = 0; i < allowedPrefixes.Length; i++)
            {
                if (block.Code.Path.StartsWith(allowedPrefixes[i]))
                {
                    return toolModes;
                }
            }

            return null;
        }

        public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
        {
            return slot.Itemstack.Attributes.GetInt("toolMode", 0);
        }

        public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
        {
            slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot itemSlot)
        {
            return ArrayExtensions.Append(base.GetHeldInteractionHelp(itemSlot), new WorldInteraction
            {
                ActionLangCode = "paxel:heldhelp-settoolmode",
                HotKeyCode = "toolmodeselect"
            });
        }

        public virtual List<BlockPos> GetCurrentBlockList(BlockSelection blockSel, ItemSlot slot)
        {
            List<BlockPos> list = new();
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            EnumDrillModes @int = (EnumDrillModes)slot.Itemstack.Attributes.GetInt("toolMode", 0);
            bool flag = @int == EnumDrillModes.DrillX;
            if (flag)
            {
                bool isVertical = blockSel.Face.IsVertical;
                if (isVertical)
                {
                    BlockPos blockPos = blockSel.Position.Copy();
                    list.Add(blockPos);
                    blockPos = blockPos.Copy();
                    blockPos.X--;
                    blockPos.Z--;
                    list.Add(blockPos);
                    blockPos = blockPos.Copy();
                    blockPos.X += 2;
                    list.Add(blockPos);
                    blockPos = blockPos.Copy();
                    blockPos.Z += 2;
                    list.Add(blockPos);
                    blockPos = blockPos.Copy();
                    blockPos.X -= 2;
                    list.Add(blockPos);
                }
                else
                {
                    bool isAxisWE = blockSel.Face.IsAxisWE;
                    if (isAxisWE)
                    {
                        BlockPos blockPos2 = blockSel.Position.Copy();
                        list.Add(blockPos2);
                        blockPos2 = blockPos2.Copy();
                        blockPos2.Y--;
                        blockPos2.Z--;
                        list.Add(blockPos2);
                        blockPos2 = blockPos2.Copy();
                        blockPos2.Y += 2;
                        list.Add(blockPos2);
                        blockPos2 = blockPos2.Copy();
                        blockPos2.Z += 2;
                        list.Add(blockPos2);
                        blockPos2 = blockPos2.Copy();
                        blockPos2.Y -= 2;
                        list.Add(blockPos2);
                    }
                    else
                    {
                        bool isAxisNS = blockSel.Face.IsAxisNS;
                        if (isAxisNS)
                        {
                            BlockPos blockPos3 = blockSel.Position.Copy();
                            list.Add(blockPos3);
                            blockPos3 = blockPos3.Copy();
                            blockPos3.Y--;
                            blockPos3.X--;
                            list.Add(blockPos3);
                            blockPos3 = blockPos3.Copy();
                            blockPos3.Y += 2;
                            list.Add(blockPos3);
                            blockPos3 = blockPos3.Copy();
                            blockPos3.X += 2;
                            list.Add(blockPos3);
                            blockPos3 = blockPos3.Copy();
                            blockPos3.Y -= 2;
                            list.Add(blockPos3);
                        }
                    }
                }
            }
            else
            {
                bool flag2 = @int == EnumDrillModes.DrillPLUS;
                if (flag2)
                {
                    bool isVertical2 = blockSel.Face.IsVertical;
                    if (isVertical2)
                    {
                        BlockPos blockPos4 = blockSel.Position.Copy();
                        blockPos4.Z--;
                        list.Add(blockPos4);
                        blockPos4 = blockPos4.Copy();
                        blockPos4.Z += 2;
                        list.Add(blockPos4);
                        blockPos4 = blockSel.Position.Copy();
                        blockPos4.X--;
                        list.Add(blockPos4);
                        blockPos4 = blockPos4.Copy();
                        blockPos4.X += 2;
                        list.Add(blockPos4);
                    }
                    else
                    {
                        bool isAxisWE2 = blockSel.Face.IsAxisWE;
                        if (isAxisWE2)
                        {
                            BlockPos blockPos5 = blockSel.Position.Copy();
                            blockPos5.Z--;
                            list.Add(blockPos5);
                            blockPos5 = blockPos5.Copy();
                            blockPos5.Z += 2;
                            list.Add(blockPos5);
                            blockPos5 = blockSel.Position.Copy();
                            blockPos5.Y--;
                            list.Add(blockPos5);
                            blockPos5 = blockPos5.Copy();
                            blockPos5.Y += 2;
                            list.Add(blockPos5);
                        }
                        else
                        {
                            bool isAxisNS2 = blockSel.Face.IsAxisNS;
                            if (isAxisNS2)
                            {
                                BlockPos blockPos6 = blockSel.Position.Copy();
                                blockPos6.Y--;
                                list.Add(blockPos6);
                                blockPos6 = blockPos6.Copy();
                                blockPos6.Y += 2;
                                list.Add(blockPos6);
                                blockPos6 = blockSel.Position.Copy();
                                blockPos6.X--;
                                list.Add(blockPos6);
                                blockPos6 = blockPos6.Copy();
                                blockPos6.X += 2;
                                list.Add(blockPos6);
                            }
                        }
                    }
                }
                else
                {
                    bool flag3 = @int == EnumDrillModes.Drill1x1;
                    if (flag3)
                    {
                        list.Add(blockSel.Position);
                    }
                    else
                    {
                        bool isVertical3 = blockSel.Face.IsVertical;
                        if (isVertical3)
                        {
                            bool flag4 = @int == EnumDrillModes.Drill2x1;
                            if (flag4)
                            {
                                num3 = -1;
                                num4 = 0;
                            }
                            else
                            {
                                bool flag5 = @int == EnumDrillModes.Drill3x1;
                                if (flag5)
                                {
                                    num3 = -2;
                                    num4 = 0;
                                }
                            }
                            bool flag6 = @int == EnumDrillModes.Drill3x3;
                            if (flag6)
                            {
                                num = -1;
                                num2 = 1;
                                num5 = -1;
                                num6 = 1;
                            }
                        }
                        else
                        {
                            bool flag7 = blockSel.Face == BlockFacing.EAST || blockSel.Face == BlockFacing.WEST;
                            if (flag7)
                            {
                                num3 = -1;
                                num4 = 1;
                                num5 = -1;
                                num6 = 1;
                                bool flag8 = @int == EnumDrillModes.Drill2x1;
                                if (flag8)
                                {
                                    num5 = 0;
                                    num6 = 0;
                                    num3 = 0;
                                }
                                else
                                {
                                    bool flag9 = @int == EnumDrillModes.Drill3x1;
                                    if (flag9)
                                    {
                                        num5 = 0;
                                        num6 = 0;
                                    }
                                }
                            }
                            else
                            {
                                bool flag10 = blockSel.Face == BlockFacing.NORTH || blockSel.Face == BlockFacing.SOUTH;
                                if (flag10)
                                {
                                    num3 = -1;
                                    num4 = 1;
                                    num = -1;
                                    num2 = 1;
                                    bool flag11 = @int == EnumDrillModes.Drill2x1;
                                    if (flag11)
                                    {
                                        num = 0;
                                        num2 = 0;
                                        num3 = 0;
                                    }
                                    else
                                    {
                                        bool flag12 = @int == EnumDrillModes.Drill3x1;
                                        if (flag12)
                                        {
                                            num = 0;
                                            num2 = 0;
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = num; i < num2 + 1; i++)
                        {
                            for (int j = num5; j < num6 + 1; j++)
                            {
                                for (int k = num3; k < num4 + 1; k++)
                                {
                                    BlockPos blockPos7 = blockSel.Position.Copy();
                                    blockPos7.X += i;
                                    blockPos7.Y += k;
                                    blockPos7.Z += j;
                                    list.Add(blockPos7);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        public virtual bool CanMultiBreak(Block block)
        {
            for (int i = 0; i < this.allowedPrefixes.Length; i++)
            {
                if (block.Code.Path.StartsWith(this.allowedPrefixes[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool CanMine(ICoreAPI api, BlockPos atpos)
        {
            Block block = api.World.BlockAccessor.GetBlock(atpos);
            for (int i = 0; i < allowedPrefixes.Length; i++)
            {
                if (block.Code.Path.StartsWith(allowedPrefixes[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static void ClearHighlights(IWorldAccessor world, IPlayer player)
        {
            world.HighlightBlocks(player, ItemMiningDrill.HighlightSlotId, new List<BlockPos>(), new List<int>(), 0, 0, 1f);
        }

    }
}

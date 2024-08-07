using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Paxel
{
    public class ItemPaxelPowered : Item
    {

        public SkillItem[] toolModes;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            allowedPrefixes = Attributes["codePrefixes"].AsArray<string>(null, null);
            

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

        public virtual bool CanMultiBreak(Block block)
        {
            for (int i = 0; i < allowedPrefixes.Length; i++)
            {
                if (block.Code.Path.StartsWith(allowedPrefixes[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private void DamageNearbyBlocks(IPlayer player, BlockSelection blockSel, float damage, int leftDurability) //, ItemSlot itemSlot
        {
            Block block = player.Entity.World.BlockAccessor.GetBlock(blockSel.Position);
            if (!CanMultiBreak(block))
            {
                return;
            }
            foreach (BlockPos pos in blockPositions)
            {
                if (leftDurability == 0)
                {
                    break;
                }
                BlockFacing facing = BlockFacing.FromNormal(player.Entity.ServerPos.GetViewVector()).Opposite;
                if (player.Entity.World.Claims.TryAccess(player, pos, EnumBlockAccessFlags.BuildOrBreak))
                {
                    Block block2 = player.Entity.World.BlockAccessor.GetBlock(pos);
                    IBlockAccessor blockAccessor = player.Entity.World.BlockAccessor;
                    block2.GetSelectionBoxes(blockAccessor, pos);
                    blockAccessor.DamageBlock(pos, facing, damage);
                    leftDurability--;
                }
            }
        }

        public override float OnBlockBreaking(IPlayer player, BlockSelection blockSel, ItemSlot itemSlot, float remainingResistance, float dt, int counter)
        {
            player.Entity.World.BlockAccessor.GetBlock(blockSel.Position);
            curMode = GetToolMode(itemSlot, player, blockSel);
            if (curMode < 5)
            {
                SetRangesFromFace(blockSel, player);
            }
            if (curMode == 5)
            {
                numBlocks = 8;
            }
            if (curMode == 6)
            {
                numBlocks = 5;
            }
            blockPositions = GetBlocksToBreak(player.Entity.World, blockSel, xMin, xMax, yMin, yMax, zMin, zMax, player);
            float num = (curMode == 0) ? 1 : (numBlocks - 1);
            num *= 0.75f;
            float newResist = base.OnBlockBreaking(player, blockSel, itemSlot, remainingResistance, dt / num, counter);
            int leftDurability = itemSlot.Itemstack.Attributes.GetInt("durability", Durability);
            DamageNearbyBlocks(player, blockSel, remainingResistance - newResist, leftDurability); //, itemSlot
            return newResist;
        }

        public override bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 0.95f)
        {
            Block block = world.BlockAccessor.GetBlock(blockSel.Position);
            if (byEntity is not EntityPlayer || itemslot.Itemstack == null)
            {
                return true;
            }
            IPlayer player = world.PlayerByUid((byEntity as EntityPlayer).PlayerUID);
            dropQuantityMultiplier = GetToolMode(itemslot, player, blockSel) switch
            {
                1 => 0.9f,
                2 => 0.8f,
                3 => 0.65f,
                4 => 0.7f,
                5 => 0.85f,
                6 => 0.75f,
                _ => 0.95f,
            };
            base.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, dropQuantityMultiplier);
            if (!CanMultiBreak(block))
            {
                return true;
            }
            itemslot.Itemstack.Attributes.GetInt("durability", Durability);
            blockPositions ??= GetBlocksToBreak(player.Entity.World, blockSel, xMin, xMax, yMin, yMax, zMin, zMax, player);
            foreach (BlockPos val in blockPositions)
            {
                if (player.Entity.World.Claims.TryAccess(player, val, (EnumBlockAccessFlags)1) && world.BlockAccessor.GetBlock(val).RequiredMiningTier <= itemslot.Itemstack.Attributes.GetInt("miningTier", ToolTier))
                {
                    world.BlockAccessor.BreakBlock(val, player, 1f);
                    world.BlockAccessor.MarkBlockDirty(val);
                    DamageItem(world, byEntity, itemslot, 1);
                    if (itemslot.Itemstack == null)
                    {
                        break;
                    }
                }
            }
            return true;
        }

        public void SetRangesFromFace(BlockSelection sel, IPlayer player)
        {
            GetRangesFromToolMode(curMode);
            string code = sel.Face.Opposite.Code;
            if (code == "north")
            {
                xMin = minWidth;
                xMax = maxWidth;
                yMin = minHeight;
                yMax = maxHeight;
                zMin = -maxDepth;
                zMax = -minDepth;
                return;
            }
            if (code == "south")
            {
                xMin = -maxWidth;
                xMax = -minWidth;
                yMin = minHeight;
                yMax = maxHeight;
                zMin = minDepth;
                zMax = maxDepth;
                return;
            }
            if (code == "east")
            {
                xMin = minDepth;
                xMax = maxDepth;
                yMin = minHeight;
                yMax = maxHeight;
                zMin = minWidth;
                zMax = maxWidth;
                return;
            }
            if (code == "west")
            {
                xMin = -maxDepth;
                xMax = -minDepth;
                yMin = minHeight;
                yMax = maxHeight;
                zMin = -maxWidth;
                zMax = -minWidth;
                return;
            }
            if (code == "up")
            {
                SetRangesFromFace(GetDirectionForUpDown(player));
                return;
            }
            if (!(code == "down"))
            {
                return;
            }
            SetRangesFromFace(GetDirectionForUpDown(player));
        }

        public void GetRangesFromToolMode(int toolMode)
        {
            switch (toolMode)
            {
                case 0:
                    minWidth = 0;
                    maxWidth = 0;
                    minHeight = 0;
                    maxHeight = 0;
                    minDepth = 0;
                    maxDepth = 0;
                    numBlocks = 1;
                    return;
                case 1:
                    minWidth = 0;
                    maxWidth = 0;
                    minHeight = -1;
                    maxHeight = 0;
                    minDepth = 0;
                    maxDepth = 0;
                    numBlocks = 2;
                    return;
                case 2:
                    minWidth = 0;
                    maxWidth = 0;
                    minHeight = -1;
                    maxHeight = 1;
                    minDepth = 0;
                    maxDepth = 0;
                    numBlocks = 3;
                    return;
                case 3:
                    minWidth = -1;
                    maxWidth = 1;
                    minHeight = -1;
                    maxHeight = 1;
                    minDepth = 0;
                    maxDepth = 0;
                    numBlocks = 9;
                    return;
                case 4:
                    minWidth = -2;
                    maxWidth = 2;
                    minHeight = -2;
                    maxHeight = 2;
                    minDepth = 0;
                    maxDepth = 0;
                    numBlocks = 25;
                    return;
                //case 5:
                //    minWidth = -1;
                //    maxWidth = 2;
                //    minHeight = -1;
                //    maxHeight = 2;
                //    minDepth = 0;
                //    maxDepth = 0;
                //    numBlocks = 8;
                //    return;
                default:
                    return;
            }
        }

        public void SetRangesFromFace(string dir)
        {
            if (dir == "north")
            {
                xMin = minWidth;
                xMax = maxWidth;
                yMin = -maxDepth;
                yMax = -minDepth;
                zMin = minHeight;
                zMax = maxHeight;
                return;
            }
            if (dir == "south")
            {
                xMin = -maxWidth;
                xMax = -minWidth;
                yMin = -maxDepth;
                yMax = -minDepth;
                zMin = -maxHeight;
                zMax = -minHeight;
                return;
            }
            if (dir == "east")
            {
                xMin = -maxHeight;
                xMax = -minHeight;
                yMin = -maxDepth;
                yMax = -minDepth;
                zMin = minWidth;
                zMax = maxWidth;
                return;
            }
            if (!(dir == "west"))
            {
                return;
            }
            xMin = minHeight;
            xMax = maxHeight;
            yMin = -maxDepth;
            yMax = -minDepth;
            zMin = -maxWidth;
            zMax = -minWidth;
        }

        private List<BlockPos> GetBlocksToBreak(IWorldAccessor world, BlockSelection sel, int minX, int maxX, int minY, int maxY, int minZ, int maxZ, IPlayer player)
        {
            List<BlockPos> positions = new();
            if (curMode == 5)
            {
                string code = sel.Face.Opposite.Code;
                if (!(code == "north"))
                {
                    if (!(code == "south"))
                    {
                        if (!(code == "east"))
                        {
                            if (!(code == "west"))
                            {
                                if (code == "up" || code == "down")
                                {
                                    string dir = GetDirectionForUpDown(player);
                                    if (!(dir == "north"))
                                    {
                                        if (!(dir == "south"))
                                        {
                                            if (!(dir == "east"))
                                            {
                                                if (dir == "west")
                                                {
                                                    BlockPos dpos = sel.Position.AddCopy(-1, 0, -1);
                                                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                                    {
                                                        positions.Add(dpos);
                                                    }
                                                    dpos = sel.Position.AddCopy(-2, 0, 0);
                                                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                                    {
                                                        positions.Add(dpos);
                                                    }
                                                    dpos = sel.Position.AddCopy(-3, 0, -1);
                                                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                                    {
                                                        positions.Add(dpos);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                BlockPos dpos = sel.Position.AddCopy(1, 0, 1);
                                                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                                {
                                                    positions.Add(dpos);
                                                }
                                                dpos = sel.Position.AddCopy(2, 0, 0);
                                                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                                {
                                                    positions.Add(dpos);
                                                }
                                                dpos = sel.Position.AddCopy(3, 0, 1);
                                                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                                {
                                                    positions.Add(dpos);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            BlockPos dpos = sel.Position.AddCopy(-1, 0, 1);
                                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                            {
                                                positions.Add(dpos);
                                            }
                                            dpos = sel.Position.AddCopy(0, 0, 2);
                                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                            {
                                                positions.Add(dpos);
                                            }
                                            dpos = sel.Position.AddCopy(-1, 0, 3);
                                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                            {
                                                positions.Add(dpos);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        BlockPos dpos = sel.Position.AddCopy(1, 0, -1);
                                        if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                        {
                                            positions.Add(dpos);
                                        }
                                        dpos = sel.Position.AddCopy(0, 0, -2);
                                        if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                        {
                                            positions.Add(dpos);
                                        }
                                        dpos = sel.Position.AddCopy(1, 0, -3);
                                        if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                        {
                                            positions.Add(dpos);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                BlockPos dpos = sel.Position.AddCopy(0, 1, -1);
                                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                {
                                    positions.Add(dpos);
                                }
                                dpos = sel.Position.AddCopy(0, 2, 0);
                                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                {
                                    positions.Add(dpos);
                                }
                                dpos = sel.Position.AddCopy(0, 3, -1);
                                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                                {
                                    positions.Add(dpos);
                                }
                            }
                        }
                        else
                        {
                            BlockPos dpos = sel.Position.AddCopy(0, 1, 1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                            {
                                positions.Add(dpos);
                            }
                            dpos = sel.Position.AddCopy(0, 2, 0);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                            {
                                positions.Add(dpos);
                            }
                            dpos = sel.Position.AddCopy(0, 3, 1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                            {
                                positions.Add(dpos);
                            }
                        }
                    }
                    else
                    {
                        BlockPos dpos = sel.Position.AddCopy(-1, 1, 0);
                        if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                        {
                            positions.Add(dpos);
                        }
                        dpos = sel.Position.AddCopy(0, 2, 0);
                        if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                        {
                            positions.Add(dpos);
                        }
                        dpos = sel.Position.AddCopy(-1, 3, 0);
                        if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                        {
                            positions.Add(dpos);
                        }
                    }
                }
                else
                {
                    BlockPos dpos = sel.Position.AddCopy(0, -1, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                    dpos = sel.Position.AddCopy(2, -1, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                    dpos = sel.Position.AddCopy(1, 0, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                    dpos = sel.Position.AddCopy(-1, 0, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                    dpos = sel.Position.AddCopy(0, 1, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                    dpos = sel.Position.AddCopy(2, 1, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                    dpos = sel.Position.AddCopy(-1, 2, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                    dpos = sel.Position.AddCopy(1, 2, 0);
                    if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                    {
                        positions.Add(dpos);
                    }
                }
                return positions;
            }
            if (curMode == 6)
            {
                switch (sel.Face.Axis)
                {
                    case 0:
                        {
                            BlockPos dpos2 = sel.Position.AddCopy(0, -1, 1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(0, -1, -1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(0, 1, 1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(0, 1, -1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            break;
                        }
                    case (EnumAxis)1:
                        {
                            BlockPos dpos2 = sel.Position.AddCopy(-1, 0, 1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(-1, 0, -1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(1, 0, 1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(1, 0, -1);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            break;
                        }
                    case (EnumAxis)2:
                        {
                            BlockPos dpos2 = sel.Position.AddCopy(-1, 1, 0);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(-1, -1, 0);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(1, 1, 0);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            dpos2 = sel.Position.AddCopy(1, -1, 0);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
                            {
                                positions.Add(dpos2);
                            }
                            break;
                        }
                }
                return positions;
            }
            for (int dx = minX; dx <= maxX; dx++)
            {
                for (int dy = minY; dy <= maxY; dy++)
                {
                    for (int dz = minZ; dz <= maxZ; dz++)
                    {
                        if (dx != 0 || dy != 0 || dz != 0)
                        {
                            BlockPos pos = sel.Position.AddCopy(dx, dy, dz);
                            if (CanMultiBreak(world.BlockAccessor.GetBlock(pos)))
                            {
                                positions.Add(pos);
                            }
                        }
                    }
                }
            }
            return positions;
        }

        public static string GetDirectionForUpDown(IPlayer player)
        {
            string dir = "north";
            float playerFacing = player.WorldData.EntityPlayer.SidedPos.Yaw;
            if (playerFacing >= 0.75f && playerFacing < 2.3f)
            {
                dir = "north";
            }
            if (playerFacing >= 2.3f && playerFacing < 3.9f)
            {
                dir = "west";
            }
            if (playerFacing >= 3.9f && playerFacing < 5.5f)
            {
                dir = "south";
            }
            if (playerFacing >= 5.5f || playerFacing < 0.75f)
            {
                dir = "east";
            }
            return dir;
        }

        public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
        {
            if (blockSel == null) return null;
            //Block block = forPlayer.Entity.World.BlockAccessor.GetBlock(blockSel.Position);
            //return block is BlockAnvil ? toolModes : null;
            return toolModes;
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

        private string[] allowedPrefixes;

        private List<BlockPos> blockPositions;

        private int minWidth;

        private int minHeight;

        private int minDepth;

        private int xMin;

        private int yMin;

        private int zMin;

        private int maxWidth;

        private int maxHeight;

        private int maxDepth;

        private int xMax;

        private int yMax;

        private int zMax;

		private int curMode = 0;

		private int numBlocks = 1;
    }
}

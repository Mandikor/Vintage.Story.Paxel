using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.API.Config;
using System;

namespace Paxel;

public class ItemPaxelTMfull : ItemAxe
{
    public SkillItem[] toolModes;

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

    private List<BlockPos> blockPositions;

    private int curMode = 3;

    private int numBlocks = 1;

    /// <summary>
    /// A (hopefully) unique id for my high lighting
    /// </summary>
    private readonly int highlightID = 92;

    /// <summary>
    /// Current Highlighted blocks
    /// </summary>
    private readonly List<BlockPos> highlightedBlocks;


    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);

        if (api.Side != EnumAppSide.Client) return;

        if (api is ICoreClientAPI capi)
        {
            SkillItem[] modes;
            toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", () =>
            {
                modes = new SkillItem[8];
                modes[0] = new SkillItem() { Code = new AssetLocation("1x1"), Name = Lang.Get("paxel:1x1", Array.Empty<object>()) }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 64, 64, 5, ColorUtil.WhiteArgb));
                modes[1] = new SkillItem() { Code = new AssetLocation("1x2"), Name = Lang.Get("paxel:1x2", Array.Empty<object>()) }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 64, 64, 5, ColorUtil.WhiteArgb));
                modes[2] = new SkillItem() { Code = new AssetLocation("1x3"), Name = Lang.Get("paxel:1x3", Array.Empty<object>()) }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x3.svg"), 64, 64, 5, ColorUtil.WhiteArgb));
                modes[3] = new SkillItem() { Code = new AssetLocation("chess"), Name = Lang.Get("paxel:chessboard", Array.Empty<object>()) }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/chess.svg"), 64, 64, 5, ColorUtil.WhiteArgb));
                modes[4] = new SkillItem() { Code = new AssetLocation("3x3"), Name = Lang.Get("paxel:3x3", Array.Empty<object>()) }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x3.svg"), 64, 64, 5, ColorUtil.WhiteArgb));
                modes[5] = new SkillItem() { Code = new AssetLocation("5x5"), Name = Lang.Get("paxel:5x5", Array.Empty<object>()) }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/5x5.svg"), 64, 64, 5, ColorUtil.WhiteArgb));
                modes[6] = new SkillItem() { Code = new AssetLocation("tunnel"), Name = Lang.Get("paxel:tunnel", Array.Empty<object>()), Linebreak = true }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/tunnel.svg"), 64, 64, 5, ColorUtil.WhiteArgb));
                modes[7] = new SkillItem() { Code = new AssetLocation("veinmine"), Name = Lang.Get("paxel:veinmining", Array.Empty<object>()) }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/vein.svg"), 64, 64, 5, ColorUtil.WhiteArgb));

                if (capi != null)
                {
                    SkillItem[] array = modes;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].TexturePremultipliedAlpha = false;
                    }
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

    public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
    {
        if (blockSel == null) return null;

        return toolModes;
    }

    public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
    {
        return slot.Itemstack.Attributes.GetInt("toolMode", 0);
    }

    public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
    {
        if (blockSel == null) return;
        slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
        ClearHighlight(byPlayer.Entity.World, byPlayer, slot); // clear the highlight so it can be redrawn with new settings
    }

    public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot itemSlot) => new WorldInteraction[] {
            new()
            {
                ActionLangCode = "paxel:heldhelp-settoolmode",
                HotKeyCode = "toolmodeselect",
                MouseButton = EnumMouseButton.None
            }
        }.Append(base.GetHeldInteractionHelp(itemSlot));

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
            numBlocks = 4;
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
        DamageNearbyBlocks(player, blockSel, remainingResistance - newResist, leftDurability, itemSlot);
        return newResist;
    }

    public virtual bool CanMultiBreak(Block block)
    {
        // checks what block your trying to destroy
        if (
            block.BlockMaterial == EnumBlockMaterial.Stone
            | block.BlockMaterial == EnumBlockMaterial.Soil
            | block.BlockMaterial == EnumBlockMaterial.Sand
            | block.BlockMaterial == EnumBlockMaterial.Gravel
            | block.BlockMaterial == EnumBlockMaterial.Snow
            | block.BlockMaterial == EnumBlockMaterial.Ceramic
            | block.BlockMaterial == EnumBlockMaterial.Stone
            | block.BlockMaterial == EnumBlockMaterial.Ore
            | block.BlockMaterial == EnumBlockMaterial.Metal
            | block.BlockMaterial == EnumBlockMaterial.Ice
            | block.BlockMaterial == EnumBlockMaterial.Wood
            | block.BlockMaterial == EnumBlockMaterial.Plant
            | block.BlockMaterial == EnumBlockMaterial.Leaves
            )
        {
            return true;
        }

        return false;
    }

    private void DamageNearbyBlocks(IPlayer player, BlockSelection blockSel, float damage, int leftDurability, ItemSlot itemSlot)
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
            if (player.Entity.World.Claims.TryAccess(player, val, EnumBlockAccessFlags.BuildOrBreak) && world.BlockAccessor.GetBlock(val).RequiredMiningTier <= itemslot.Itemstack.Attributes.GetInt("miningTier", ToolTier))
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
                BlockPos dpos = sel.Position.AddCopy(1, 1, 0);
                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                {
                    positions.Add(dpos);
                }
                dpos = sel.Position.AddCopy(0, 2, 0);
                if (CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
                {
                    positions.Add(dpos);
                }
                dpos = sel.Position.AddCopy(1, 3, 0);
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
                case EnumAxis.X:
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
                case EnumAxis.Y:
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
                case EnumAxis.Z:
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
                minWidth = -2;
                maxWidth = 2;
                minHeight = -1;
                maxHeight = 2;
                minDepth = 0;
                maxDepth = 0;
                numBlocks = 10;
                return;
            case 4:
                minWidth = -1;
                maxWidth = 1;
                minHeight = -1;
                maxHeight = 1;
                minDepth = 0;
                maxDepth = 0;
                numBlocks = 9;
                return;
            case 5:
                minWidth = -2;
                maxWidth = 2;
                minHeight = -2;
                maxHeight = 2;
                minDepth = 0;
                maxDepth = 0;
                numBlocks = 25;
                return;
            default:
                return;
        }
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

    /// <summary>
    /// Highlight the blocks the tool will cover for the given player. 
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="player"></param>
    /// <param name="blocks"></param>
    /// <param name="selectedFace"></param>
    //public void HighlightToolBlocks(ItemSlot slot, EntityPlayer player, BlockPos blockSel, BlockPos playerPos, BlockFacing selectedFace)
    //{
    //    int toolMode = GetToolMode(slot, player.Player, null);
    //    //int toolFunc = GetToolFunction(slot, player.Player);
    //    //int toolPlac = GetToolPlaceMode(slot, player.Player);
    //    //int toolrange = GetToolRange(slot, player.Player);
    //    //if (toolrange == -1) toolrange = 0; // if no range is saved, default of 0
    //    //BlockPos toolAnchor = GetAnchor(slot, player.Player); // can be used for everything
    //    BlockPos startPoint = null; // start of highlight
    //    BlockPos endPoint = null;   // end of highlight
    //                                // Tool Mode ... Largely ignored as all modes affect in the same way.
    //                                // Function Mode ... Build2Me ignores Range ... Others based on player view direction :(
    //                                // Placement mode ... taken care of in OnHeldIdle

    //    if (toolFunc <= 0) // selection for Build2Me specifically, the only one that uses playerPos
    //    {
    //        // The start is obvious, the selected block
    //        if (toolAnchor.X != int.MinValue) startPoint = toolAnchor.Copy();
    //        else startPoint = blockSel.Copy();
    //        // the end, however isn't obvious...                 
    //        switch (selectedFace.Code)
    //        {
    //            case "north": endPoint = blockSel.NorthCopy(Math.Abs(blockSel.Z - playerPos.Z)); break;
    //            case "east": endPoint = blockSel.EastCopy(Math.Abs(blockSel.X - playerPos.X)); break;
    //            case "south": endPoint = blockSel.SouthCopy(Math.Abs(blockSel.Z - playerPos.Z)); break;
    //            case "west": endPoint = blockSel.WestCopy(Math.Abs(blockSel.X - playerPos.X)); break;
    //            case "up": endPoint = blockSel.UpCopy(Math.Abs(blockSel.Y - playerPos.Y)); break;
    //            case "down": endPoint = blockSel.DownCopy(Math.Abs(blockSel.Y - playerPos.Y)); break;
    //            default: endPoint = blockSel.AddCopy(BlockFacing.UP, 64); break;
    //        }
    //        if (blockSel.Y == playerPos.Y) endPoint.Y = blockSel.Y;
    //    }
    //    else
    //    {
    //        // other tool functions!
    //        if (toolrange == 0) startPoint = endPoint = blockSel;
    //        else
    //        {
    //            string playerfacecode = BlockFacing.HorizontalFromAngle(GameMath.Mod(player.Pos.Yaw, 6.28318548f)).Code;
    //            switch (toolFunc)
    //            {
    //                case 1: // Horizontal Line
    //                    {
    //                        switch (selectedFace.Code)
    //                        {
    //                            case "north":
    //                            case "south":
    //                                {
    //                                    startPoint = blockSel.EastCopy(toolrange);
    //                                    endPoint = blockSel.WestCopy(toolrange);
    //                                    break;
    //                                }
    //                            case "east":
    //                            case "west":
    //                                {
    //                                    startPoint = blockSel.NorthCopy(toolrange);
    //                                    endPoint = blockSel.SouthCopy(toolrange);
    //                                    break;
    //                                }
    //                            case "up":
    //                            case "down":
    //                                {
    //                                    if (playerfacecode == "north" || playerfacecode == "south")
    //                                    {
    //                                        startPoint = blockSel.EastCopy(toolrange);
    //                                        endPoint = blockSel.WestCopy(toolrange);
    //                                    }
    //                                    else
    //                                    {
    //                                        startPoint = blockSel.NorthCopy(toolrange);
    //                                        endPoint = blockSel.SouthCopy(toolrange);
    //                                    }
    //                                    break;
    //                                }
    //                        }
    //                        break;
    //                    }
    //                case 2: // Vertical Line
    //                    {
    //                        switch (selectedFace.Code)
    //                        {
    //                            case "north":
    //                            case "south":
    //                                {
    //                                    startPoint = blockSel.UpCopy(toolrange);
    //                                    endPoint = blockSel.DownCopy(toolrange);
    //                                    break;
    //                                }
    //                            case "east":
    //                            case "west":
    //                                {
    //                                    startPoint = blockSel.UpCopy(toolrange);
    //                                    endPoint = blockSel.DownCopy(toolrange);
    //                                    break;
    //                                }
    //                            case "up":
    //                                {
    //                                    startPoint = blockSel.Copy();
    //                                    endPoint = blockSel.UpCopy(toolrange * 2);
    //                                    break;
    //                                }
    //                            case "down":
    //                                {
    //                                    startPoint = blockSel.Copy();
    //                                    endPoint = blockSel.DownCopy(toolrange * 2);
    //                                    break;
    //                                }
    //                        }
    //                        break;
    //                    }
    //                case 3: // Area - 2 dimensional area of blocks
    //                    {
    //                        switch (selectedFace.Code)
    //                        {
    //                            case "north":
    //                            case "south":
    //                                {
    //                                    startPoint = blockSel.EastCopy(toolrange).Up(toolrange);
    //                                    endPoint = blockSel.WestCopy(toolrange).Down(toolrange);
    //                                    break;
    //                                }
    //                            case "east":
    //                            case "west":
    //                                {
    //                                    startPoint = blockSel.NorthCopy(toolrange).Up(toolrange);
    //                                    endPoint = blockSel.SouthCopy(toolrange).Down(toolrange);
    //                                    break;
    //                                }
    //                            case "up":
    //                            case "down":
    //                                {
    //                                    startPoint = blockSel.AddCopy(toolrange, 0, toolrange);
    //                                    endPoint = blockSel.AddCopy(-toolrange, 0, -toolrange);
    //                                    break;
    //                                }
    //                        }
    //                        break;
    //                    }
    //                case 4: // Volume ... Around the player? Or away from player? Using Anchor will be key
    //                    {
    //                        startPoint = blockSel.AddCopy(toolrange, toolrange, toolrange);
    //                        endPoint = blockSel.AddCopy(-toolrange, -toolrange, -toolrange);
    //                        break;
    //                    }
    //            }
    //        }
    //    }

    //    List<int> usecolors = new List<int>
    //        {
    //            ColorUtil.ColorFromRgba( 0, 0, 110, 110 ) // Blue for build
    //        };
    //    // if in destroy mode, highlight goes RED
    //    if (toolMode == 1) usecolors[0] = ColorUtil.ColorFromRgba(110, 0, 0, 110); // Red for Destroy
    //    if (toolMode == 2) usecolors[0] = ColorUtil.ColorFromRgba(110, 0, 110, 110); // Purple for Exchange

    //    // Build the Highlight and list of blocks for this selection
    //    if (startPoint != null && endPoint != null)
    //    {
    //        if (highlightedBlocks != null && highlightedBlocks.Count > 0)
    //        {
    //            ClearHighlight(player.World, player.Player, slot); // clears the items in the array AND the highlight object in the world...
    //        }
    //        highlightedBlocks = GetToolBlocks(slot, startPoint, endPoint, selectedFace, blockSel);
    //        try
    //        {
    //            api.World.HighlightBlocks(player.Player, highlightID, highlightedBlocks,
    //                    usecolors, EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Arbitrary);
    //        }
    //        catch (Exception e)
    //        {
    //            //capi?.ShowChatMessage($"Exception : {e}");
    //        }
    //    }

    //}

    /// <summary>
    /// Clears the block highlight for a given player.
    /// Resets the gadget internals, called after build.
    /// </summary>
    /// <param name="world">IWorldAccessor</param>
    /// <param name="player">IPlayer</param>
    /// <param name="slot">Gadget</param>
    /// <param name="resetGadget">If true, resets stored BlockPos values</param>
    public void ClearHighlight(IWorldAccessor world, IPlayer player, ItemSlot slot) //, bool resetGadget
    {
        if (highlightedBlocks != null && highlightedBlocks.Count > 0)
        {
            highlightedBlocks.Clear();
            //if (resetGadget)
            //{
            //    cur_blockSelection = null;
            //    cur_playerPosition = null;
            //    cur_blockFaceSelection = null;
            //}
        }
        api.World.HighlightBlocks(player, highlightID, new List<BlockPos>(), new List<int>(), EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Arbitrary);
    }


    /// <summary>
    /// Gets a List of BlockPos to highlight/edit
    /// </summary>
    /// <param name="slot">Slot holding the gadget</param>
    /// <param name="startBlock">Start Block, typically the block right at the selected block.</param>
    /// <param name="endBlock">End Block, depends on toolMode</param>
    /// <param name="blockFacing">Block Face player is looking at</param>
    /// <param name="originalPos">Original Block Selected, before the offsets</param>
    /// <returns>List of valid matching Blocks for tool mode selections.</returns>
    //public List<BlockPos> GetToolBlocks(ItemSlot slot, BlockPos startBlock, BlockPos endBlock, BlockFacing blockFacing, BlockPos originalPos)
    //{
    //    int toolMode = GetToolMode(slot, null, null);

    //    if (startBlock.dimension != endBlock.dimension)
    //    {
    //        api.Logger.Debug($"BuildingGadget: GetToolBlocks crosses from dimension {startBlock.dimension} to {endBlock.dimension}");
    //    }
    //    int dim = startBlock.dimension;

    //    List<BlockPos> blocksForTool = new List<BlockPos>();
    //    BlockPos minBlock = new BlockPos(Math.Min(startBlock.X, endBlock.X), Math.Min(startBlock.Y, endBlock.Y),
    //        Math.Min(startBlock.Z, endBlock.Z), dim);
    //    BlockPos maxBlock = new BlockPos(Math.Max(startBlock.X, endBlock.X), Math.Max(startBlock.Y, endBlock.Y),
    //        Math.Max(startBlock.Z, endBlock.Z), dim);

    //    if (toolMode != 2)
    //    {
    //        for (int x = minBlock.X; x <= maxBlock.X; x++)
    //        {
    //            for (int y = minBlock.Y; y <= maxBlock.Y; y++)
    //            {
    //                for (int z = minBlock.Z; z <= maxBlock.Z; z++)
    //                {
    //                    blocksForTool.Add(new BlockPos(x, y, z, dim));
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Block originalBlock = api.World.BlockAccessor.GetBlock(originalPos);
    //        for (int x = minBlock.X; x <= maxBlock.X; x++)
    //        {
    //            for (int y = minBlock.Y; y <= maxBlock.Y; y++)
    //            {
    //                for (int z = minBlock.Z; z <= maxBlock.Z; z++)
    //                {
    //                    Block blockCheck = api.World.BlockAccessor.GetBlock(new BlockPos(x, y, z, dim));
    //                    if (blockCheck.Id == 0) continue; // skip air
    //                                                      // ignore blocks with an entity
    //                    if (api.World.BlockAccessor.GetBlockEntity(new BlockPos(x, y, z, dim)) == null)
    //                    {
    //                        if (blockCheck.FirstCodePart() == block_build.FirstCodePart())
    //                        {
    //                            // they are both soil, or rock, or whatever                                
    //                            if (blockCheck.CodeWithoutParts(1) != block_build.CodeWithoutParts(1))
    //                            {
    //                                //checked the code without the end part to see if they are the same.
    //                                // if they are NOT the same, add to list. 
    //                                // IOW soil-medium-none != soil-medium-grassy
    //                                blocksForTool.Add(new BlockPos(x, y, z, dim));
    //                            }
    //                        }
    //                        else
    //                        {
    //                            blocksForTool.Add(new BlockPos(x, y, z, dim));
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return blocksForTool;
    //}


    //private void AttemptVeinMine(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1f)
    //{
    //    if (this.api.Side != EnumAppSide.Server)
    //    {
    //        return;
    //    }
    //    if (world == null)
    //    {
    //        this.capi.ShowChatMessage("World is null!");
    //    }
    //    if (byEntity == null)
    //    {
    //        this.capi.ShowChatMessage("Entity is null!");
    //    }
    //    if (itemslot == null)
    //    {
    //        this.capi.ShowChatMessage("ItemSlot is null!");
    //    }
    //    if (blockSel == null)
    //    {
    //        this.capi.ShowChatMessage("BlockSel is null!");
    //    }
    //    Block block = world.BlockAccessor.GetBlock(blockSel.Position);
    //    if (block.Id != 0)
    //    {
    //        string path = block.Code.Path;
    //    }
    //    IPlayer player = null;
    //    List<ItemStack> itemsToDrop = new List<ItemStack>();
    //    if (byEntity is EntityPlayer)
    //    {
    //        player = this.api.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
    //    }
    //    if (player == null)
    //    {
    //        this.api.World.Logger.VerboseDebug("FlexibleTools: Worldeater Player isnt EntityPlayer; Player is null!");
    //        return;
    //    }
    //    bool istree = false;
    //    string treeGroupCode = string.Empty;
    //    int savedLimit = this.config.WorldEaterVeinMineLimit;
    //    if (this.blockToMatch.Contains("log"))
    //    {
    //        istree = true;
    //        treeGroupCode = block.Attributes["treeFellingGroupCode"].AsString(null);
    //        if (treeGroupCode != null)
    //        {
    //            this.config.WorldEaterVeinMineLimit = 1024;
    //        }
    //        else
    //        {
    //            istree = false;
    //        }
    //    }
    //    List<BlockPos> blocksToMine = new List<BlockPos>();
    //    List<BlockPos> blocksToCheck = new List<BlockPos>();
    //    blocksToCheck.Add(blockSel.Position);
    //    blocksToMine.Add(blockSel.Position);
    //    while (blocksToCheck.Count > 0 && blocksToMine.Count < this.config.WorldEaterVeinMineLimit)
    //    {
    //        List<BlockPos> blockstoadd = new List<BlockPos>();
    //        Action<Block, int, int, int> <> 9__0;
    //        foreach (BlockPos blockPos2 in blocksToCheck)
    //        {
    //            BlockPos start = blockPos2.AddCopy(-1, -1, -1);
    //            BlockPos end = blockPos2.AddCopy(1, 1, 1);
    //            IBlockAccessor blockAccessor = world.BlockAccessor;
    //            BlockPos minPos = start;
    //            BlockPos maxPos = end;
    //            Action<Block, int, int, int> onBlock;
    //            if ((onBlock = <> 9__0) == null)
    //            {
    //                onBlock = (<> 9__0 = delegate (Block dblock, int x, int y, int z)
    //                {
    //                    if (dblock.BlockId != 0)
    //                    {
    //                        BlockPos bcheck = new BlockPos(x, y, z, 0);
    //                        if (istree)
    //                        {
    //                            if (dblock.Attributes != null && dblock.Attributes["treeFellingGroupCode"].Exists && dblock.Attributes["treeFellingGroupCode"].AsString(null).Contains(treeGroupCode) && !dblock.Code.Path.Contains("leaves") && !blocksToMine.Contains(bcheck))
    //                            {
    //                                blockstoadd.Add(bcheck);
    //                                blocksToMine.Add(bcheck);
    //                                return;
    //                            }
    //                        }
    //                        else if (dblock.Code.Path.Contains(this.blockToMatch) && !blocksToMine.Contains(bcheck))
    //                        {
    //                            blockstoadd.Add(bcheck);
    //                            blocksToMine.Add(bcheck);
    //                        }
    //                    }
    //                });
    //            }
    //            blockAccessor.WalkBlocks(minPos, maxPos, onBlock, false);
    //        }
    //        blocksToCheck.Clear();
    //        if (blockstoadd.Count > 0 && blocksToMine.Count < this.config.WorldEaterVeinMineLimit)
    //        {
    //            blocksToCheck.AddRange(blockstoadd);
    //        }
    //        blockstoadd.Clear();
    //    }
    //    blocksToCheck.Clear();
    //    this.config.WorldEaterVeinMineLimit = savedLimit;
    //    if (blocksToMine.Count > 0)
    //    {
    //        foreach (BlockPos blockPos in blocksToMine)
    //        {
    //            ItemStack[] blockDrops = world.BlockAccessor.GetBlock(blockPos).GetDrops(world, blockPos, player, dropQuantityMultiplier);
    //            if (blockDrops != null)
    //            {
    //                if (blockDrops.Length != 0)
    //                {
    //                    foreach (ItemStack itemStack in blockDrops)
    //                    {
    //                        itemsToDrop.Add(itemStack);
    //                    }
    //                }
    //                this.coreapi.World.BlockAccessor.SetBlock(0, blockPos);
    //                this.coreapi.World.BlockAccessor.MarkBlockDirty(blockPos, null);
    //                world.BlockAccessor.TriggerNeighbourBlockUpdate(blockPos);
    //                player.InventoryManager.ActiveHotbarSlot.Itemstack.Collectible.DamageItem(world, player.Entity, player.InventoryManager.ActiveHotbarSlot, 1);
    //                if (player.InventoryManager.ActiveHotbarSlot.Itemstack == null)
    //                {
    //                    break;
    //                }
    //                if (player.InventoryManager.ActiveHotbarSlot.Itemstack.Item != null && player.InventoryManager.ActiveHotbarSlot.Itemstack.Item != this)
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //        foreach (ItemStack stack in itemsToDrop)
    //        {
    //            try
    //            {
    //                if (!player.InventoryManager.TryGiveItemstack(stack, true))
    //                {
    //                    world.SpawnItemEntity(stack, blockSel.Position.ToVec3d(), null);
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                ILogger logger = this.coreapi.World.Logger;
    //                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
    //                defaultInterpolatedStringHandler.AppendLiteral("Exception while trying to process drops! ");
    //                defaultInterpolatedStringHandler.AppendFormatted<Exception>(e);
    //                logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
    //                this.coreapi.World.Logger.Warning("Stacktrace: " + e.StackTrace);
    //            }
    //        }
    //        blocksToMine.Clear();
    //        itemsToDrop.Clear();
    //    }
    //}

}

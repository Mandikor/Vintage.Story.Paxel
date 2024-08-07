using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Paxel
{
	public class ItemAOETool : Item
	{
		public override void OnLoaded(ICoreAPI api)
		{
			base.OnLoaded(api);
			allowedPrefixes = Attributes["codePrefixes"].AsArray<string>(null, null);
            if (api is ICoreClientAPI capi)
            {
                this.toolModes = ObjectCacheUtil.GetOrCreate<SkillItem[]>(api, "aoeToolModes", delegate ()
                {
                    SkillItem[] modes = new SkillItem[]
                    {
                        new SkillItem{Code = new AssetLocation("1size"),Name = Lang.Get("1x1x1", Array.Empty<object>())}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1x1.svg"), 48, 48, 30, new int?(-1))),
                        new SkillItem{Code = new AssetLocation("1x2x1size"),Name = Lang.Get("1x2x1", Array.Empty<object>())}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2x1.svg"), 48, 48, 15, new int?(-1))),
                        new SkillItem{Code = new AssetLocation("2size"),Name = Lang.Get("2x2x1", Array.Empty<object>())}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/2x2x1.svg"), 48, 48, 5, new int?(-1))),
                        new SkillItem{Code = new AssetLocation("3size"),Name = Lang.Get("3x3x1", Array.Empty<object>())}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x3x1.svg"), 48, 48, 5, new int?(-1))),
                        new SkillItem{Code = new AssetLocation("3x2x1size"),Name = Lang.Get("3x2x1", Array.Empty<object>())}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x2x1.svg"), 48, 48, 5, new int?(-1))),
                        new SkillItem{Code = new AssetLocation("2x4x1checker"),Name = Lang.Get("2x4 Checker", Array.Empty<object>())}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/2x4checker.svg"), 48, 48, 5, new int?(-1))),
                        new SkillItem{Code = new AssetLocation("checker"),Name = Lang.Get("Checkerboard", Array.Empty<object>())}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/checker.svg"), 48, 48, 5, new int?(-1)))
                    };
                    SkillItem[] array = modes;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].TexturePremultipliedAlpha = false;
                    }
                    return modes;
                });
            }
        }

		public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot itemSlot)
		{
			return ArrayExtensions.Append<WorldInteraction>(base.GetHeldInteractionHelp(itemSlot), new WorldInteraction
			{
				ActionLangCode = "heldhelp-settoolmode",
				HotKeyCode = "toolmodeselect"
			});
		}

		public override void OnUnloaded(ICoreAPI api)
		{
			int i = 0;
			while (this.toolModes != null && i < this.toolModes.Length)
			{
				SkillItem skillItem = this.toolModes[i];
				skillItem?.Dispose();
				i++;
			}
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

		public override float OnBlockBreaking(IPlayer player, BlockSelection blockSel, ItemSlot itemSlot, float remainingResistance, float dt, int counter)
		{
			player.Entity.World.BlockAccessor.GetBlock(blockSel.Position);
			this.curMode = this.GetToolMode(itemSlot, player, blockSel);
			if (this.curMode < 5)
			{
				this.SetRangesFromFace(blockSel, player);
			}
			if (this.curMode == 5)
			{
				this.numBlocks = 4;
			}
			if (this.curMode == 6)
			{
				this.numBlocks = 5;
			}
			this.blockPositions = this.GetBlocksToBreak(player.Entity.World, blockSel, this.xMin, this.xMax, this.yMin, this.yMax, this.zMin, this.zMax, player);
			float num = (float)((this.curMode == 0) ? 1 : (this.numBlocks - 1));
			num *= 0.75f;
			float newResist = base.OnBlockBreaking(player, blockSel, itemSlot, remainingResistance, dt / num, counter);
			int leftDurability = itemSlot.Itemstack.Attributes.GetInt("durability", this.Durability);
			this.DamageNearbyBlocks(player, blockSel, remainingResistance - newResist, leftDurability, itemSlot);
			return newResist;
		}

		private void DamageNearbyBlocks(IPlayer player, BlockSelection blockSel, float damage, int leftDurability, ItemSlot itemSlot)
		{
			Block block = player.Entity.World.BlockAccessor.GetBlock(blockSel.Position);
			if (!this.CanMultiBreak(block))
			{
				return;
			}
			foreach (BlockPos pos in this.blockPositions)
			{
				if (leftDurability == 0)
				{
					break;
				}
				BlockFacing facing = BlockFacing.FromNormal(player.Entity.ServerPos.GetViewVector()).Opposite;
				if (player.Entity.World.Claims.TryAccess(player, pos, (EnumBlockAccessFlags)1))
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
			switch (this.GetToolMode(itemslot, player, blockSel))
			{
			case 1:
				dropQuantityMultiplier = 0.9f;
				break;
			case 2:
				dropQuantityMultiplier = 0.8f;
				break;
			case 3:
				dropQuantityMultiplier = 0.65f;
				break;
			case 4:
				dropQuantityMultiplier = 0.7f;
				break;
			case 5:
				dropQuantityMultiplier = 0.85f;
				break;
			case 6:
				dropQuantityMultiplier = 0.75f;
				break;
			default:
				dropQuantityMultiplier = 0.95f;
				break;
			}
			base.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, dropQuantityMultiplier);
			if (!this.CanMultiBreak(block))
			{
				return true;
			}
			itemslot.Itemstack.Attributes.GetInt("durability", this.Durability);
			if (this.blockPositions == null)
			{
				this.blockPositions = this.GetBlocksToBreak(player.Entity.World, blockSel, this.xMin, this.xMax, this.yMin, this.yMax, this.zMin, this.zMax, player);
			}
			foreach (BlockPos val in this.blockPositions)
			{
				if (player.Entity.World.Claims.TryAccess(player, val, (EnumBlockAccessFlags)1) && world.BlockAccessor.GetBlock(val).RequiredMiningTier <= itemslot.Itemstack.Attributes.GetInt("miningTier", this.ToolTier))
				{
					world.BlockAccessor.BreakBlock(val, player, 1f);
					world.BlockAccessor.MarkBlockDirty(val);
					this.DamageItem(world, byEntity, itemslot, 1);
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
			this.GetRangesFromToolMode(this.curMode);
			string code = sel.Face.Opposite.Code;
			if (code == "north")
			{
				this.xMin = this.minWidth;
				this.xMax = this.maxWidth;
				this.yMin = this.minHeight;
				this.yMax = this.maxHeight;
				this.zMin = -this.maxDepth;
				this.zMax = -this.minDepth;
				return;
			}
			if (code == "south")
			{
				this.xMin = -this.maxWidth;
				this.xMax = -this.minWidth;
				this.yMin = this.minHeight;
				this.yMax = this.maxHeight;
				this.zMin = this.minDepth;
				this.zMax = this.maxDepth;
				return;
			}
			if (code == "east")
			{
				this.xMin = this.minDepth;
				this.xMax = this.maxDepth;
				this.yMin = this.minHeight;
				this.yMax = this.maxHeight;
				this.zMin = this.minWidth;
				this.zMax = this.maxWidth;
				return;
			}
			if (code == "west")
			{
				this.xMin = -this.maxDepth;
				this.xMax = -this.minDepth;
				this.yMin = this.minHeight;
				this.yMax = this.maxHeight;
				this.zMin = -this.maxWidth;
				this.zMax = -this.minWidth;
				return;
			}
			if (code == "up")
			{
				this.SetRangesFromFace(this.GetDirectionForUpDown(player));
				return;
			}
			if (!(code == "down"))
			{
				return;
			}
			this.SetRangesFromFace(this.GetDirectionForUpDown(player));
		}

		public void SetRangesFromFace(string dir)
		{
			if (dir == "north")
			{
				this.xMin = this.minWidth;
				this.xMax = this.maxWidth;
				this.yMin = -this.maxDepth;
				this.yMax = -this.minDepth;
				this.zMin = this.minHeight;
				this.zMax = this.maxHeight;
				return;
			}
			if (dir == "south")
			{
				this.xMin = -this.maxWidth;
				this.xMax = -this.minWidth;
				this.yMin = -this.maxDepth;
				this.yMax = -this.minDepth;
				this.zMin = -this.maxHeight;
				this.zMax = -this.minHeight;
				return;
			}
			if (dir == "east")
			{
				this.xMin = -this.maxHeight;
				this.xMax = -this.minHeight;
				this.yMin = -this.maxDepth;
				this.yMax = -this.minDepth;
				this.zMin = this.minWidth;
				this.zMax = this.maxWidth;
				return;
			}
			if (!(dir == "west"))
			{
				return;
			}
			this.xMin = this.minHeight;
			this.xMax = this.maxHeight;
			this.yMin = -this.maxDepth;
			this.yMax = -this.minDepth;
			this.zMin = -this.maxWidth;
			this.zMax = -this.minWidth;
		}

		private List<BlockPos> GetBlocksToBreak(IWorldAccessor world, BlockSelection sel, int minX, int maxX, int minY, int maxY, int minZ, int maxZ, IPlayer player)
		{
			List<BlockPos> positions = new List<BlockPos>();
			if (this.curMode == 5)
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
									string dir = this.GetDirectionForUpDown(player);
									if (!(dir == "north"))
									{
										if (!(dir == "south"))
										{
											if (!(dir == "east"))
											{
												if (dir == "west")
												{
													BlockPos dpos = sel.Position.AddCopy(-1, 0, -1);
													if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
													{
														positions.Add(dpos);
													}
													dpos = sel.Position.AddCopy(-2, 0, 0);
													if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
													{
														positions.Add(dpos);
													}
													dpos = sel.Position.AddCopy(-3, 0, -1);
													if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
													{
														positions.Add(dpos);
													}
												}
											}
											else
											{
												BlockPos dpos = sel.Position.AddCopy(1, 0, 1);
												if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
												{
													positions.Add(dpos);
												}
												dpos = sel.Position.AddCopy(2, 0, 0);
												if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
												{
													positions.Add(dpos);
												}
												dpos = sel.Position.AddCopy(3, 0, 1);
												if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
												{
													positions.Add(dpos);
												}
											}
										}
										else
										{
											BlockPos dpos = sel.Position.AddCopy(-1, 0, 1);
											if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
											{
												positions.Add(dpos);
											}
											dpos = sel.Position.AddCopy(0, 0, 2);
											if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
											{
												positions.Add(dpos);
											}
											dpos = sel.Position.AddCopy(-1, 0, 3);
											if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
											{
												positions.Add(dpos);
											}
										}
									}
									else
									{
										BlockPos dpos = sel.Position.AddCopy(1, 0, -1);
										if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
										{
											positions.Add(dpos);
										}
										dpos = sel.Position.AddCopy(0, 0, -2);
										if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
										{
											positions.Add(dpos);
										}
										dpos = sel.Position.AddCopy(1, 0, -3);
										if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
										{
											positions.Add(dpos);
										}
									}
								}
							}
							else
							{
								BlockPos dpos = sel.Position.AddCopy(0, 1, -1);
								if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
								{
									positions.Add(dpos);
								}
								dpos = sel.Position.AddCopy(0, 2, 0);
								if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
								{
									positions.Add(dpos);
								}
								dpos = sel.Position.AddCopy(0, 3, -1);
								if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
								{
									positions.Add(dpos);
								}
							}
						}
						else
						{
							BlockPos dpos = sel.Position.AddCopy(0, 1, 1);
							if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
							{
								positions.Add(dpos);
							}
							dpos = sel.Position.AddCopy(0, 2, 0);
							if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
							{
								positions.Add(dpos);
							}
							dpos = sel.Position.AddCopy(0, 3, 1);
							if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
							{
								positions.Add(dpos);
							}
						}
					}
					else
					{
						BlockPos dpos = sel.Position.AddCopy(-1, 1, 0);
						if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
						{
							positions.Add(dpos);
						}
						dpos = sel.Position.AddCopy(0, 2, 0);
						if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
						{
							positions.Add(dpos);
						}
						dpos = sel.Position.AddCopy(-1, 3, 0);
						if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
						{
							positions.Add(dpos);
						}
					}
				}
				else
				{
					BlockPos dpos = sel.Position.AddCopy(1, 1, 0);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
					{
						positions.Add(dpos);
					}
					dpos = sel.Position.AddCopy(0, 2, 0);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
					{
						positions.Add(dpos);
					}
					dpos = sel.Position.AddCopy(1, 3, 0);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos)))
					{
						positions.Add(dpos);
					}
				}
				return positions;
			}
			if (this.curMode == 6)
			{
				switch (sel.Face.Axis)
				{
				case (EnumAxis)0:
				{
					BlockPos dpos2 = sel.Position.AddCopy(0, -1, 1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(0, -1, -1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(0, 1, 1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(0, 1, -1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					break;
				}
				case (EnumAxis)1:
				{
					BlockPos dpos2 = sel.Position.AddCopy(-1, 0, 1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(-1, 0, -1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(1, 0, 1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(1, 0, -1);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					break;
				}
				case (EnumAxis)2:
				{
					BlockPos dpos2 = sel.Position.AddCopy(-1, 1, 0);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(-1, -1, 0);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(1, 1, 0);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
					{
						positions.Add(dpos2);
					}
					dpos2 = sel.Position.AddCopy(1, -1, 0);
					if (this.CanMultiBreak(world.BlockAccessor.GetBlock(dpos2)))
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
							if (this.CanMultiBreak(world.BlockAccessor.GetBlock(pos)))
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
				this.minWidth = 0;
				this.maxWidth = 0;
				this.minHeight = 0;
				this.maxHeight = 0;
				this.minDepth = 0;
				this.maxDepth = 0;
				this.numBlocks = 1;
				return;
			case 1:
				this.minWidth = 0;
				this.maxWidth = 0;
				this.minHeight = -1;
				this.maxHeight = 0;
				this.minDepth = 0;
				this.maxDepth = 0;
				this.numBlocks = 2;
				return;
			case 2:
				this.minWidth = 0;
				this.maxWidth = 1;
				this.minHeight = -1;
				this.maxHeight = 0;
				this.minDepth = 0;
				this.maxDepth = 0;
				this.numBlocks = 4;
				return;
			case 3:
				this.minWidth = -1;
				this.maxWidth = 1;
				this.minHeight = -1;
				this.maxHeight = 1;
				this.minDepth = 0;
				this.maxDepth = 0;
				this.numBlocks = 9;
				return;
			case 4:
				this.minWidth = -1;
				this.maxWidth = 1;
				this.minHeight = -1;
				this.maxHeight = 0;
				this.minDepth = 0;
				this.maxDepth = 0;
				this.numBlocks = 6;
				return;
			case 5:
				this.minWidth = 0;
				this.maxWidth = 0;
				this.minHeight = -1;
				this.maxHeight = 1;
				this.minDepth = 0;
				this.maxDepth = 2;
				this.numBlocks = 4;
				return;
			default:
				return;
			}
		}

		public string GetDirectionForUpDown(IPlayer player)
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
			return this.toolModes;
		}

		public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
		{
			return slot.Itemstack.Attributes.GetInt("toolMode", 0);
		}

		public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
		{
			slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
		}

		private string[] allowedPrefixes;

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

		private SkillItem[] toolModes;

		private List<BlockPos> blockPositions;

		private int curMode = 3;

		private int numBlocks = 1;
	}
}

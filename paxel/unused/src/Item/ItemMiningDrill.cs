using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Paxel
{
	internal class ItemMiningDrill : Item
	{
		private float SoundLevel
		{
			get
			{
				return this.soundlevel;
			}
		}

		public override void OnHeldIdle(ItemSlot slot, EntityAgent byEntity)
		{
			base.OnHeldIdle(slot, byEntity);
			EntityPlayer entityPlayer = byEntity as EntityPlayer;
			IPlayer player = entityPlayer?.Player;
            ClearHighlights(this.api.World, player);
			BlockSelection currentBlockSelection = player.CurrentBlockSelection;
			bool flag = currentBlockSelection == null;
			if (!flag)
			{
				List<BlockPos> currentBlockList = this.GetCurrentBlockList(currentBlockSelection, slot);
				bool flag2 = currentBlockList == null || currentBlockList.Count == 0;
				if (!flag2)
				{
					List<int> list = new();
					for (int i = 0; i < currentBlockList.Count; i++)
					{
						bool flag3 = this.CanMine(this.api, currentBlockList[i]);
						if (flag3)
						{
							list.Add(ColorUtil.ColorFromRgba(255, 255, 0, 32));
						}
						else
						{
							list.Add(ColorUtil.ColorFromRgba(255, 0, 0, 32));
						}
					}
					this.api.World.HighlightBlocks(player, ItemMiningDrill.HighlightSlotId, currentBlockList, list, 0, 0, 1f);
				}
			}
		}

		public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
		{
			this.nextactionat = this.actionspeed;
			handling = (EnumHandHandling)1;
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

		public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
		{
			bool flag = blockSel == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
                IPlayer player = (byEntity is EntityPlayer entityPlayer) ? entityPlayer.Player : null;
                bool flag2 = player != null && player.WorldData.CurrentGameMode == (EnumGameMode)2;
				float num = slot.Itemstack.Attributes.GetFloat("fuelintank", 0f);
				float num2 = slot.Itemstack.Attributes.GetFloat("drillhead", 100f);
				bool flag3 = num2 <= 0f && !flag2;
				if (flag3)
				{
					ItemMiningDrill.PlaySound(this.api, "paxel:sounds/error", player.Entity.Pos.AsBlockPos);
					result = false;
				}
				else
				{
					bool flag4 = this.capi == null && num < this.tankcapacity;
					if (flag4)
					{
						BlockEntityContainer blockEntityContainer = this.api.World.BlockAccessor.GetBlockEntity(blockSel.Position) as BlockEntityContainer;
						bool flag5 = blockEntityContainer != null;
						if (flag5)
						{
							this.TryFuel(blockEntityContainer, slot);
						}
					}
					bool flag6 = num <= 0f && !flag2;
					if (flag6)
					{
						ItemMiningDrill.PlaySound(this.api, "paxel:sounds/error", player.Entity.Pos.AsBlockPos);
						result = false;
					}
					else
					{
						bool flag7 = secondsUsed > this.startdelay && !this.soundplayed;
						if (flag7)
						{
							this.soundplayed = true;
						}
						ItemMiningDrill.myParticles.MinPos = byEntity.Pos.XYZ.Add(-0.25, 1.0, 0.0).Ahead(1.0, byEntity.Pos.Pitch, byEntity.Pos.Yaw);
						ItemMiningDrill.myParticles.AddPos = new Vec3d(0.5, 0.5, 0.5);
						ItemMiningDrill.myParticles.MinVelocity = new Vec3f(0f, -0.1f, 0f);
						ItemMiningDrill.myParticles.AddVelocity = new Vec3f(0f, 0.5f, 0f);
						ItemMiningDrill.myParticles.LifeLength = 1f;
						ItemMiningDrill.myParticles.addLifeLength = 0.5f;
						ItemMiningDrill.myParticles.MinQuantity = 3f;
						ItemMiningDrill.myParticles.AddQuantity = 10f;
						ItemMiningDrill.myParticles.GravityEffect = 0f;
						ItemMiningDrill.myParticles.SizeEvolve = new EvolvingNatFloat((EnumTransformFunction)1, 2f);
						ItemMiningDrill.myParticles.ParticleModel = 0;
						byEntity.World.SpawnParticles(ItemMiningDrill.myParticles, null);
						ItemMiningDrill.myParticles.Color = ColorUtil.ToRgba(16, 16, 0, 32);
						ItemMiningDrill.myParticles.OpacityEvolve = new EvolvingNatFloat((EnumTransformFunction)1, -255f);
						this.ToggleAmbientSounds(true, blockSel.Position);
						bool flag8 = secondsUsed > this.nextactionat;
						if (flag8)
						{
							List<BlockPos> currentBlockList = this.GetCurrentBlockList(blockSel, slot);
							foreach (BlockPos blockPos in currentBlockList)
							{
								Block block = this.api.World.BlockAccessor.GetBlock(blockPos);
								bool flag9 = block == null;
								if (!flag9)
								{
									bool flag10 = block.MatterState != EnumMatterState.Solid;
									if (!flag10)
									{
										bool flag11 = block.RequiredMiningTier > 5;
										if (!flag11)
										{
											bool flag12 = !this.CanMine(block);
											if (!flag12)
											{
												bool flag13 = !this.api.World.Claims.TryAccess(player, blockPos, (EnumBlockAccessFlags)1);
												if (!flag13)
												{
													this.api.World.BlockAccessor.BreakBlock(blockPos, player, 1f);
													bool flag14 = !flag2;
													if (flag14)
													{
														num2 -= this.drillheadusepertick;
														bool flag15 = num2 <= 0f;
														if (flag15)
														{
															break;
														}
													}
												}
											}
										}
									}
								}
							}
							this.nextactionat += this.actionspeed;
							bool flag16 = !flag2;
							if (flag16)
							{
								num -= this.fuelusepertick;
							}
							bool flag17 = this.api is not ICoreClientAPI;
							if (flag17)
							{
								slot.Itemstack.Attributes.SetFloat("fuelintank", num);
								slot.Itemstack.Attributes.SetFloat("drillhead", num2);
								slot.MarkDirty();
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
		{
			this.soundplayed = false;
			this.nextactionat = 0f;
            IPlayer player = (byEntity is EntityPlayer entityPlayer) ? entityPlayer.Player : null;
            ClearHighlights(this.api.World, player);
			this.CleanSound();
			base.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel);
		}

		public override void OnHeldDropped(IWorldAccessor world, IPlayer byPlayer, ItemSlot slot, int quantity, ref EnumHandling handling)
		{
			base.OnHeldDropped(world, byPlayer, slot, quantity, ref handling);
            ClearHighlights(world, byPlayer);
			this.CleanSound();
		}

		public virtual bool CanMine(ICoreAPI api, BlockPos atpos)
		{
			Block block = api.World.BlockAccessor.GetBlock(atpos);
			return this.CanMine(block);
		}

		public virtual bool CanMine(Block tryblock)
		{
			bool flag = tryblock.BlockMaterial is not (EnumBlockMaterial)6 and not (EnumBlockMaterial)7;
			return !flag;
		}

		public override void OnUnloaded(ICoreAPI api)
		{
			base.OnUnloaded(api);
			this.CleanSound();
		}

		private void CleanSound()
		{
			ILoadedSound loadedSound = this.ambientSound;
			loadedSound?.Stop();
			ILoadedSound loadedSound2 = this.ambientSound;
			loadedSound2?.Dispose();
			this.ambientSound = null;
		}

		public void ToggleAmbientSounds(bool on, BlockPos Pos)
		{
			bool flag = api.Side != (EnumAppSide)2;
			if (!flag)
			{
				bool flag2 = this.runsound == "" || this.SoundLevel == 0f;
				if (!flag2)
				{
					if (on)
					{
						bool flag3 = this.ambientSound == null || (!this.ambientSound.IsPlaying && (!this.alreadyPlayedSound || this.loopsound));
						if (flag3)
						{
							this.ambientSound = ((IClientWorldAccessor)this.api.World).LoadSound(new SoundParams
							{
								Location = new AssetLocation(this.runsound),
								ShouldLoop = this.loopsound,
								Position = Pos.ToVec3f().Add(0.5f, 0.25f, 0.5f),
								DisposeOnFinish = false,
								Volume = this.SoundLevel,
								Range = 10f
							});
							this.soundoffdelaycounter = 0;
							this.ambientSound.Start();
							this.alreadyPlayedSound = true;
						}
					}
					else
					{
						bool flag4 = this.loopsound && this.soundoffdelaycounter < 10;
						if (flag4)
						{
							this.soundoffdelaycounter++;
						}
						else
						{
							ILoadedSound loadedSound = this.ambientSound;
							loadedSound?.Stop();
							ILoadedSound loadedSound2 = this.ambientSound;
							loadedSound2?.Dispose();
							this.ambientSound = null;
							this.alreadyPlayedSound = false;
						}
					}
				}
			}
		}

		private void TryFuel(BlockEntityContainer bec, ItemSlot myslot)
		{
			bool flag = bec == null || this.capi != null;
			if (!flag)
			{
				bool flag2 = bec.Inventory == null || bec.Inventory.Empty;
				if (!flag2)
				{
					foreach (ItemSlot itemSlot in bec.Inventory)
					{
						bool flag3 = itemSlot == null || itemSlot.Empty || itemSlot.StackSize <= 0;
						if (!flag3)
						{
							bool flag4 = itemSlot.Itemstack.Item == null;
							if (!flag4)
							{
								bool flag5 = !this.IsFuel(itemSlot.Itemstack.Item);
								if (!flag5)
								{
									float num = itemSlot.Itemstack.Attributes.GetFloat("fuelintank", 0f);
									float val = this.tankcapacity - num;
									float num2 = Math.Min(val, (float)itemSlot.StackSize);
									bool flag6 = (int)num2 > 1;
									if (flag6)
									{
										itemSlot.Itemstack.StackSize -= (int)Math.Ceiling((double)num2);
										bool flag7 = itemSlot.Itemstack.StackSize <= 0;
										if (flag7)
										{
											itemSlot.Itemstack = null;
										}
										num += num2;
										itemSlot.MarkDirty();
										bec.MarkDirty(true, null);
										myslot.Itemstack.Attributes.SetFloat("fuelintank", num);
										myslot.MarkDirty();
									}
								}
							}
						}
					}
				}
			}
		}

		public virtual bool IsFuel(Item checkitem)
		{
			return checkitem.Code.ToString().Contains("spiritportion");
		}

		public override void OnLoaded(ICoreAPI api)
		{
			base.OnLoaded(api);
			bool flag = api is ICoreClientAPI;
			if (flag)
			{
				this.capi = (api as ICoreClientAPI);
			}
			this.toolModes = ObjectCacheUtil.GetOrCreate<SkillItem[]>(api, "DrillToolModes", delegate()
			{
				SkillItem[] array = new SkillItem[Enum.GetNames(typeof(EnumDrillModes)).Length];
				array[0] = new SkillItem
				{
					Code = new AssetLocation(EnumDrillModes.Drill1x1.ToString()),
					Name = Lang.Get("Drill 1x1", Array.Empty<object>())
				};
				array[1] = new SkillItem
				{
					Code = new AssetLocation(EnumDrillModes.Drill2x1.ToString()),
					Name = Lang.Get("Drill 2x1", Array.Empty<object>())
				};
				array[2] = new SkillItem
				{
					Code = new AssetLocation(EnumDrillModes.Drill3x1.ToString()),
					Name = Lang.Get("Drill 3x1", Array.Empty<object>())
				};
				array[3] = new SkillItem
				{
					Code = new AssetLocation(EnumDrillModes.Drill3x3.ToString()),
					Name = Lang.Get("Drill 3x3", Array.Empty<object>())
				};
				array[4] = new SkillItem
				{
					Code = new AssetLocation(EnumDrillModes.DrillX.ToString()),
					Name = Lang.Get("Drill X", Array.Empty<object>())
				};
				array[5] = new SkillItem
				{
					Code = new AssetLocation(EnumDrillModes.DrillPLUS.ToString()),
					Name = Lang.Get("Drill +", Array.Empty<object>())
				};
				bool flag2 = this.capi != null;
				if (flag2)
				{
					array[0].WithIcon(this.capi, this.capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/drill1x1.svg"), 48, 48, 5, new int?(-1)));
					array[0].TexturePremultipliedAlpha = false;
					array[1].WithIcon(this.capi, this.capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/drill2x1.svg"), 48, 48, 5, new int?(-1)));
					array[1].TexturePremultipliedAlpha = false;
					array[2].WithIcon(this.capi, this.capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/drill3x1.svg"), 48, 48, 5, new int?(-1)));
					array[2].TexturePremultipliedAlpha = false;
					array[3].WithIcon(this.capi, this.capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/drill3x3.svg"), 48, 48, 5, new int?(-1)));
					array[3].TexturePremultipliedAlpha = false;
					array[4].WithIcon(this.capi, this.capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/drillx.svg"), 48, 48, 5, new int?(-1)));
					array[4].TexturePremultipliedAlpha = false;
					array[5].WithIcon(this.capi, this.capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/drillplus.svg"), 48, 48, 5, new int?(-1)));
					array[5].TexturePremultipliedAlpha = false;
				}
				return array;
			});
			this.interactions = ObjectCacheUtil.GetOrCreate<WorldInteraction[]>(api, "DrillInteractions", () => new WorldInteraction[]
			{
				new() {
					ActionLangCode = "Use Drill",
					MouseButton = (EnumMouseButton)2
				}
			});
		}

		public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
		{
			return this.toolModes;
		}

		public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
		{
			return Math.Min(this.toolModes.Length - 1, slot.Itemstack.Attributes.GetInt("toolMode", 0));
		}

		public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
		{
			ItemSlot mouseItemSlot = byPlayer.InventoryManager.MouseItemSlot;
			bool flag = !mouseItemSlot.Empty && mouseItemSlot.Itemstack.Item != null;
			if (flag)
			{
				bool flag2 = mouseItemSlot.Itemstack.Item == this.api.World.GetItem(new AssetLocation("paxel:drillhead-steel"));
				if (flag2)
				{
					ItemStack itemStack = mouseItemSlot.TakeOut(1);
					mouseItemSlot.MarkDirty();
					slot.Itemstack.Attributes.SetFloat("drillhead", 100f);
					slot.MarkDirty();
					ItemMiningDrill.PlaySound(this.api, "paxel:sounds/mechhammer", byPlayer.Entity.Pos.AsBlockPos);
					return;
				}
			}
			else
			{
				bool flag3 = !mouseItemSlot.Empty && mouseItemSlot.Itemstack.Block != null;
				if (flag3)
				{
				}
			}
			slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
		}

		public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
		{
			base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
			float @float = inSlot.Itemstack.Attributes.GetFloat("fuelintank", 0f);
			float float2 = inSlot.Itemstack.Attributes.GetFloat("drillhead", 100f);
			bool flag = @float <= 0f;
			if (flag)
			{
				dsc.Append("[NO FUEL!]");
			}
			else
			{
				dsc.Append("[FUEL " + Math.Ceiling((double)(@float / this.tankcapacity * 100f)).ToString() + "%]");
			}
			bool flag2 = float2 <= 0f;
			if (flag2)
			{
				dsc.Append("[MISSING/BROKEN DRILLHEAD!]");
			}
			else
			{
				dsc.Append("[DRILLHEAD " + Math.Ceiling((double)float2).ToString() + "%]");
			}
		}

		public static void ClearHighlights(IWorldAccessor world, IPlayer player)
		{
			world.HighlightBlocks(player, ItemMiningDrill.HighlightSlotId, new List<BlockPos>(), new List<int>(), 0, 0, 1f);
		}

		public static void PlaySound(ICoreAPI Api, string soundname, BlockPos pos)
		{
			bool flag = Api is ICoreClientAPI;
			if (flag)
			{
				ILoadedSound loadedSound = ((IClientWorldAccessor)Api.World).LoadSound(new SoundParams
				{
					Location = new AssetLocation(soundname),
					ShouldLoop = false,
					Position = pos.ToVec3f().Add(0.5f, 0.25f, 0.5f),
					DisposeOnFinish = true,
					Volume = 2f,
					Range = 15f
				});
				loadedSound.Start();
			}
		}

		private float nextactionat = 0f;

		private bool soundplayed = false;

		private readonly float actionspeed = 0.5f;

		private readonly float soundlevel = 1f;

		private bool alreadyPlayedSound = false;

		private readonly bool loopsound = true;

		private int soundoffdelaycounter = 0;

		private readonly float tankcapacity = 100f;

		private readonly float drillheadusepertick = 0.1f;

		private readonly float fuelusepertick = 1f;

		private readonly float startdelay = 0.1f;

		public static SimpleParticleProperties myParticles = new(1f, 1f, ColorUtil.ColorFromRgba(0, 0, 0, 75), new Vec3d(), new Vec3d(), new Vec3f(), new Vec3f(), 1f, 1f, 1f, 1f, (EnumParticleModel)1);

		public const string fuelattribute = "fuelintank";

		public const string drillheadattribute = "drillhead";

		public HUDMiningDrill hud;

		private ILoadedSound ambientSound;

		private readonly string runsound = "paxel:sounds/drillloop";

		private ICoreClientAPI capi;

		private SkillItem[] toolModes;

		private WorldInteraction[] interactions;

		public static int HighlightSlotId = 23;

		public enum EnumDrillModes
		{
			Drill1x1,
			Drill2x1,
			Drill3x1,
			Drill3x3,
			DrillX,
			DrillPLUS
		}
	}
}

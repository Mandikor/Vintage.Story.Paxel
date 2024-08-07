using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using XLib.XLeveling;

namespace XSkills
{
	// Token: 0x02000046 RID: 70
	public class XSkillsItemPickaxe : Item
	{
		// Token: 0x06000148 RID: 328 RVA: 0x0000B5F0 File Offset: 0x000097F0
		public override void OnLoaded(ICoreAPI api)
		{
			base.OnLoaded(api);
			ICoreClientAPI capi = api as ICoreClientAPI;
			if (capi != null)
			{
				this.toolModes = ObjectCacheUtil.GetOrCreate<SkillItem[]>(api, "pickaxeToolModes", () => new SkillItem[]
				{
					new SkillItem
					{
						Code = new AssetLocation("1size"),
						Name = Lang.Get("1x1", Array.Empty<object>())
					}.WithIcon(capi, new DrawSkillIconDelegate(ItemClay.Drawcreate1_svg)),
					new SkillItem
					{
						Code = new AssetLocation("3size"),
						Name = Lang.Get("3x3", Array.Empty<object>())
					}.WithIcon(capi, new DrawSkillIconDelegate(new ItemClay().Drawcreate9_svg)),
					new SkillItem
					{
						Code = new AssetLocation("vein"),
						Name = Lang.Get("vein", Array.Empty<object>())
					}.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("textures/icons/heatmap.svg"), 48, 48, 5, new int?(-1)))
				});
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000B63C File Offset: 0x0000983C
		public override void OnUnloaded(ICoreAPI api)
		{
			if (this.toolModes == null)
			{
				return;
			}
			for (int i = 0; i < this.toolModes.Length; i++)
			{
				SkillItem skillItem = this.toolModes[i];
				if (skillItem != null)
				{
					skillItem.Dispose();
				}
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000B678 File Offset: 0x00009878
		public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
		{
			XLeveling xleveling = XLeveling.Instance(this.api);
			Mining mining = ((xleveling != null) ? xleveling.GetSkill("mining", false) : null) as Mining;
			if (mining == null)
			{
				return null;
			}
			PlayerSkill playerSkill;
			if (forPlayer == null)
			{
				playerSkill = null;
			}
			else
			{
				EntityPlayer entity = forPlayer.Entity;
				if (entity == null)
				{
					playerSkill = null;
				}
				else
				{
					PlayerSkillSet behavior = entity.GetBehavior<PlayerSkillSet>();
					playerSkill = ((behavior != null) ? behavior[mining.Id] : null);
				}
			}
			PlayerSkill playerSkill2 = playerSkill;
			if (playerSkill2 == null)
			{
				return null;
			}
			PlayerAbility playerAbility = playerSkill2[mining.TunnelDiggerId];
			PlayerAbility playerAbility2 = playerSkill2[mining.VeinMinerId];
			int num = 1 + ((playerAbility != null && playerAbility.Tier > 0) ? 1 : 0) + ((playerAbility2 != null && playerAbility2.Tier > 0) ? 1 : 0);
			if (num == 1)
			{
				return null;
			}
			SkillItem[] array = new SkillItem[num];
			array[0] = this.toolModes[0];
			num = 1;
			if (playerAbility.Tier > 0)
			{
				array[num] = this.toolModes[1];
				num++;
			}
			if (playerAbility2.Tier > 0)
			{
				array[num] = this.toolModes[2];
			}
			return array;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000B770 File Offset: 0x00009970
		public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
		{
			XLeveling xleveling = XLeveling.Instance(this.api);
			Mining mining = ((xleveling != null) ? xleveling.GetSkill("mining", false) : null) as Mining;
			if (mining == null)
			{
				return 0;
			}
			PlayerSkill playerSkill;
			if (byPlayer == null)
			{
				playerSkill = null;
			}
			else
			{
				EntityPlayer entity = byPlayer.Entity;
				if (entity == null)
				{
					playerSkill = null;
				}
				else
				{
					PlayerSkillSet behavior = entity.GetBehavior<PlayerSkillSet>();
					playerSkill = ((behavior != null) ? behavior[mining.Id] : null);
				}
			}
			PlayerSkill playerSkill2 = playerSkill;
			if (playerSkill2 == null)
			{
				return 0;
			}
			PlayerAbility playerAbility = playerSkill2[mining.TunnelDiggerId];
			PlayerAbility playerAbility2 = playerSkill2[mining.VeinMinerId];
			int num = 1 + ((playerAbility != null && playerAbility.Tier > 0) ? 1 : 0) + ((playerAbility2 != null && playerAbility2.Tier > 0) ? 1 : 0);
			return GameMath.Min(new int[]
			{
				slot.Itemstack.Attributes.GetInt("toolMode", 0),
				num
			});
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00009B04 File Offset: 0x00007D04
		public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
		{
			slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000B840 File Offset: 0x00009A40
		public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
		{
			WorldInteraction[] heldInteractionHelp = base.GetHeldInteractionHelp(inSlot);
			InventoryBasePlayer inventoryBasePlayer = inSlot.Inventory as InventoryBasePlayer;
			IPlayer player = (inventoryBasePlayer != null) ? inventoryBasePlayer.Player : null;
			XLeveling xleveling = XLeveling.Instance(this.api);
			Mining mining = ((xleveling != null) ? xleveling.GetSkill("mining", false) : null) as Mining;
			if (mining == null)
			{
				return heldInteractionHelp;
			}
			PlayerSkill playerSkill;
			if (player == null)
			{
				playerSkill = null;
			}
			else
			{
				EntityPlayer entity = player.Entity;
				if (entity == null)
				{
					playerSkill = null;
				}
				else
				{
					PlayerSkillSet behavior = entity.GetBehavior<PlayerSkillSet>();
					playerSkill = ((behavior != null) ? behavior[mining.Id] : null);
				}
			}
			PlayerSkill playerSkill2 = playerSkill;
			if (playerSkill2 == null)
			{
				return heldInteractionHelp;
			}
			PlayerAbility playerAbility = playerSkill2[mining.TunnelDiggerId];
			PlayerAbility playerAbility2 = playerSkill2[mining.VeinMinerId];
			if (playerAbility != null && playerAbility.Tier == 0 && playerAbility2 != null && playerAbility2.Tier == 0)
			{
				return heldInteractionHelp;
			}
			return ArrayExtensions.Append<WorldInteraction>(new WorldInteraction[]
			{
				new WorldInteraction
				{
					ActionLangCode = "blockhelp-selecttoolmode",
					HotKeyCode = "toolmodeselect",
					MouseButton = 255
				}
			}, heldInteractionHelp);
		}

		// Token: 0x0400003C RID: 60
		private SkillItem[] toolModes;
	}
}

using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Paxel
{
	// Token: 0x0200007F RID: 127
	internal class HUDMiningDrill : HudElement
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x000243CF File Offset: 0x000225CF
		public override bool Focusable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x000243D2 File Offset: 0x000225D2
		public override bool UnregisterOnClose
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x000243D8 File Offset: 0x000225D8
		public override bool CaptureAllInputs()
		{
			return false;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x000243EC File Offset: 0x000225EC
		public override bool ShouldReceiveKeyboardEvents()
		{
			return false;
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x000243FF File Offset: 0x000225FF
		public override EnumDialogType DialogType
		{
			get
			{
				return this.dialogType;
			}
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00024407 File Offset: 0x00022607
		public HUDMiningDrill(ICoreClientAPI capi, ItemStack stack) : base(capi)
		{
			this.capi = capi;
			this.stack = stack;
			this.SetupDialog();
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00024430 File Offset: 0x00022630
		private void SetupDialog()
		{
			ElementBounds elementBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment((EnumDialogArea)6);
			ElementBounds elementBounds2 = ElementBounds.Fixed(0.0, 0.0, 300.0, 300.0);
			base.SingleComposer = GuiElementDynamicTextHelper.AddDynamicText(this.capi.Gui.CreateCompo("myAwesomeDialog", elementBounds), "STATUS", CairoFont.WhiteDetailText(), elementBounds2, "status").Compose(true);
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x000244AC File Offset: 0x000226AC
		public override void OnRenderGUI(float deltaTime)
		{
			bool flag = this.capi == null || this.stack == null || this.stack.Item == null || this.stack.StackSize == 0;
			if (flag)
			{
				GuiElementDynamicTextHelper.GetDynamicText(base.SingleComposer, "status").SetNewText("", false, false, false);
				base.OnRenderGUI(deltaTime);
			}
			float @float = this.stack.Attributes.GetFloat("fuelintank", -1f);
			float float2 = this.stack.Attributes.GetFloat("drillhead", -1f);
			bool flag2 = @float == -1f || float2 == -1f;
			if (flag2)
			{
				GuiElementDynamicTextHelper.GetDynamicText(base.SingleComposer, "status").SetNewText("", false, false, false);
				base.OnRenderGUI(deltaTime);
			}
			GuiElementDynamicTextHelper.GetDynamicText(base.SingleComposer, "status").SetNewText("Drill:" + Math.Ceiling((double)float2).ToString() + "% Fuel:" + @float.ToString(), false, false, false);
			base.OnRenderGUI(deltaTime);
		}

		// Token: 0x0400022B RID: 555
		public ItemStack stack;

		// Token: 0x0400022C RID: 556
		private readonly EnumDialogType dialogType = (EnumDialogType)1;
	}
}

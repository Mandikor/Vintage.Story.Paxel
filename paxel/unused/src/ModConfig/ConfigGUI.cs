using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Paxel.Configuration
{

    public class ConfigGui : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "configgui";

        public ConfigGui(ICoreClientAPI capi) : base(capi)
        {
            SetupDialog();
            OnTogglePause(true);
        }

        private void SetupDialog()
        {
            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            // Just a simple 300x300 pixel box
            ElementBounds textBounds = ElementBounds.Fixed(0, 40, 400, 600);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(textBounds);

            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("ConfigGuiDialog", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Paxel Mod Configuration", OnTitleBarCloseClicked)
                .AddIf(capi.IsSinglePlayer && !capi.OpenedToLan)
                .AddToggleButton("Pause game", CairoFont.WhiteDetailText(), OnTogglePause, ElementBounds.Fixed(300.0, -15.0, 100.0, 22.0), "pausegame")
                .EndIf()
                .AddStaticText("This is a piece of text at the center of your screen - Enjoy!", CairoFont.WhiteDetailText(), textBounds)
                .Compose()
            ;
        }

        private void OnTogglePause(bool on)
        {
            capi.PauseGame(on);
            capi.Settings.Bool["noHandbookPause"] = !on;
        }

        private void OnTitleBarCloseClicked()
        {
            TryClose();
        }
    }

    public class AnnoyingTextSystem : ModSystem
    {
        ICoreClientAPI capi;
        GuiDialog dialog;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);

            dialog = new ConfigGui(api);

            capi = api;
            capi.Input.RegisterHotKey("configgui", "Annoys you with annoyingly centered text", GlKeys.P, HotkeyType.GUIOrOtherControls);
            capi.Input.SetHotKeyHandler("configgui", ToggleGui);
        }

        private bool ToggleGui(KeyCombination comb)
        {
            if (dialog.IsOpened()) dialog.TryClose();
            else dialog.TryOpen();

            return true;
        }
    }
}

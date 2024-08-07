using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace Paxel.Configuration
{

    public class API
        {
            public static ICoreAPI api;
            public static ClientCoreAPI capi;
        }

    public class SinglePauseMod : ModSystem
    {
        private ICoreServerAPI api;

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            this.api = api;

            api.RegisterCommand("pause", "Pauses the game", "", PauseCommand);
        }

        public void PauseCommand(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(GlobalConstants.GeneralChatGroup, "Your game is paused, open & close the escape menu to resume game", EnumChatType.Notification);
            if (API.api != null && !API.capi.IsGamePaused && API.capi.IsSinglePlayer)
            {
                ClientMain clientMain = (ClientMain)API.api.World;
                clientMain.PauseGame(true);
                //API.api.Logger.Debug("should pause");
            }
        }

    }

    [HarmonyPatch(typeof(GuiDialogHandbook), "OnGuiOpened")]
    public class Patch_GuiDialogHandbook_OnGuiOpened
    {
        static void Postfix()
        {
            if (API.api != null && !API.capi.IsGamePaused && API.capi.IsSinglePlayer)
            {
                ClientMain clientMain = (ClientMain)API.api.World;
                clientMain.PauseGame(true);
                //API.api.Logger.Debug("should pause");
            }
        }
    }

    [HarmonyPatch(typeof(GuiDialogHandbook), "OnGuiClosed")]
    public class Patch_GuiDialogHandbook_OnGuiClosed
    {
        static void Postfix()
        {
            if (API.api != null && API.capi.IsGamePaused && API.capi.IsSinglePlayer)
            {
                ClientMain clientMain = (ClientMain)API.api.World;
                clientMain.PauseGame(false);
                //API.capi.Logger.Debug("should unpause");
            }
        }
    }

}

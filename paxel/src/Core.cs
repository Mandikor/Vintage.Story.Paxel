global using HarmonyLib;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using Vintagestory.API.Client;
global using Vintagestory.API.Common;
global using Vintagestory.API.Common.Entities;
global using Vintagestory.API.Config;
global using Vintagestory.API.Datastructures;
global using Vintagestory.API.MathTools;
global using Vintagestory.API.Util;
global using Vintagestory.Client.NoObf;
global using Vintagestory.GameContent;


namespace Paxel
{
    class Core : ModSystem
    {
        public static Config Config { get; set; }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            Config = ModConfig.ReadConfig(api);
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterItemClass("ItemPaxel", typeof(ItemPaxel));
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);
        }
    }
}

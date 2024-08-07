using System;
using Vintagestory.API.Common;

namespace Paxel.old.Configuration
{
    public class ModConfigFile
    {
        public static ModConfigFile Current { get; set; }

        public string StoneAgeEnabledDesc { get; set; } = "Enable/Disable stone based paxels";
        public bool StoneAgeEnabled { get; set; } = true;
        public string CopperAgeEnabledDesc { get; set; } = "Enable/Disable copper/bronze based paxels";
        public bool CopperAgeEnabled { get; set; } = true;
        public string IronAgeEnabledDesc { get; set; } = "Enable/Disable iron/steel based paxels";
        public bool IronAgeEnabled { get; set; } = true;
        public string DecoEnabledDesc { get; set; } = "Enable/Disable gold and silver paxel";
        public bool DecoEnabled { get; set; } = true;
        public bool LootBoxEnabled { get; set; } = true;

    }

    public static class ModConfig
    {
        public static TConfig LoadOrCreateConfig<TConfig>(this ICoreAPI api, string filename) where TConfig : new()
        {
            try
            {
                var loadedConfig = api.LoadModConfig<TConfig>(filename);
                if (loadedConfig != null)
                {
                    api.StoreModConfig(loadedConfig, filename);
                    return loadedConfig;
                }
            }
            catch (Exception e)
            {
                api.World.Logger.Error("{0}", $"Failed loading file ({filename}), error {e}. Will initialize new one");
            }

            var newConfig = new TConfig();
            api.StoreModConfig(newConfig, filename);
            return newConfig;
        }

    }
}
namespace Paxel
{
    public static class ModConfig
    {
        private const string jsonConfig = "MandikorsMods/PaxelConfig.json";

        public static Config ReadConfig(ICoreAPI api)
        {
            Config config;

            try
            {
                config = LoadConfig(api);

                if (config == null)
                {
                    GenerateConfig(api);
                    config = LoadConfig(api);
                }
                else
                {
                    GenerateConfig(api, config);
                }
            }
            catch
            {
                GenerateConfig(api);
                config = LoadConfig(api);
            }

            #region Mods
            api.World.Config.SetBool("PaxelSharpeningEnabled", config.Paxels.ModDependence["sharpening"]);
            api.World.Config.SetBool("PaxelHelveSmithingEnabled", config.Paxels.ModDependence["helvesmithing"]);
            api.World.Config.SetBool("PaxelJewelryEnabled", config.Paxels.ModDependence["jewelry"]);
            api.World.Config.SetBool("PaxelInTreeHollowEnabled", config.Paxels.ModDependence["intreehollow"]);
            #endregion

            #region Loot
            api.World.Config.SetBool("PaxelLootFromCrackedVesselEnabled", config.Paxels.LootEnabled["crackedvessel"]);
            api.World.Config.SetBool("PaxelLootFromRuinsEnabled", config.Paxels.LootEnabled["ruins"]);
            #endregion

            #region Traders
            api.World.Config.SetBool("PaxelTraderSurvivalGoodsEnabled", config.Paxels.TraderEnabled["survivalgoods"]);
            api.World.Config.SetBool("PaxelTraderBuildMaterialsEnabled", config.Paxels.TraderEnabled["buildmaterials"]);
            api.World.Config.SetBool("PaxelTraderTreasureHunterEnabled", config.Paxels.TraderEnabled["treasurehunter"]);
            #endregion

            #region Paxels
            api.World.Config.SetBool("PaxelAndesiteRecipeEnabled", config.Paxels.RecipeEnabled["andesite"]);
            api.World.Config.SetBool("PaxelBasaltRecipeEnabled", config.Paxels.RecipeEnabled["basalt"]);
            api.World.Config.SetBool("PaxelChertRecipeEnabled", config.Paxels.RecipeEnabled["chert"]);
            api.World.Config.SetBool("PaxelGraniteRecipeEnabled", config.Paxels.RecipeEnabled["granite"]);
            api.World.Config.SetBool("PaxelPeridotiteRecipeEnabled", config.Paxels.RecipeEnabled["peridotite"]);
            api.World.Config.SetBool("PaxelFlintRecipeEnabled", config.Paxels.RecipeEnabled["flint"]);
            api.World.Config.SetBool("PaxelObsidianRecipeEnabled", config.Paxels.RecipeEnabled["obsidian"]);

            api.World.Config.SetBool("PaxelCopperRecipeEnabled", config.Paxels.RecipeEnabled["copper"]);
            api.World.Config.SetBool("PaxelTinbronzeRecipeEnabled", config.Paxels.RecipeEnabled["tinbronze"]);
            api.World.Config.SetBool("PaxelBismuthbronzeRecipeEnabled", config.Paxels.RecipeEnabled["bismuthbronze"]);
            api.World.Config.SetBool("PaxelBlackbronzeRecipeEnabled", config.Paxels.RecipeEnabled["blackbronze"]);

            api.World.Config.SetBool("PaxelSilverRecipeEnabled", config.Paxels.RecipeEnabled["silver"]);
            api.World.Config.SetBool("PaxelGoldRecipeEnabled", config.Paxels.RecipeEnabled["gold"]);

            api.World.Config.SetBool("PaxelIronRecipeEnabled", config.Paxels.RecipeEnabled["iron"]);
            api.World.Config.SetBool("PaxelMeteoricironRecipeEnabled", config.Paxels.RecipeEnabled["meteoriciron"]);
            api.World.Config.SetBool("PaxelSteelRecipeEnabled", config.Paxels.RecipeEnabled["steel"]);

            api.World.Config.SetBool("PaxelElectrumRecipeEnabled", config.Paxels.RecipeEnabled["electrum"]);
            api.World.Config.SetBool("PaxelPlatinumRecipeEnabled", config.Paxels.RecipeEnabled["platinum"]);
            api.World.Config.SetBool("PaxelCupronickelRecipeEnabled", config.Paxels.RecipeEnabled["cupronickel"]);
            api.World.Config.SetBool("PaxelTitaniumRecipeEnabled", config.Paxels.RecipeEnabled["titanium"]);

            api.World.Config.SetBool("PaxelDioriteRecipeEnabled", config.Paxels.RecipeEnabled["diorite"]);
            api.World.Config.SetBool("PaxelGabbroRecipeEnabled", config.Paxels.RecipeEnabled["gabbro"]);
            api.World.Config.SetBool("PaxelQuartziteRecipeEnabled", config.Paxels.RecipeEnabled["quartzite"]);
            #endregion

            return config;

        }
        private static Config LoadConfig(ICoreAPI api)
        {
            return api.LoadModConfig<Config>(jsonConfig);
        }

        private static void GenerateConfig(ICoreAPI api)
        {
            api.StoreModConfig(new Config(), jsonConfig);
        }

        private static void GenerateConfig(ICoreAPI api, Config previousConfig)
        {
            api.StoreModConfig(new Config(previousConfig), jsonConfig);
        }
    }
}

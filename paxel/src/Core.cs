using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Paxel.Configuration;


namespace Paxel;

public class Core : ModSystem
{
    public Config Config { get; set; }

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);

        Config = ModConfig.ReadConfig<Config>(api, "MandikorsMods/PaxelConfig.json");

        #region Mods
        api.World.Config.SetBool("PaxelSharpeningEnabled", Config.ModDependence["sharpening"]);
        api.World.Config.SetBool("PaxelHelveSmithingEnabled", Config.ModDependence["helvesmithing"]);
        api.World.Config.SetBool("PaxelJewelryEnabled", Config.ModDependence["jewelry"]);
        api.World.Config.SetBool("PaxelInTreeHollowEnabled", Config.ModDependence["intreehollow"]);
        #endregion

        #region Loot
        api.World.Config.SetBool("PaxelLootFromCrackedVesselEnabled", Config.LootEnabled["crackedvessel"]);
        api.World.Config.SetBool("PaxelLootFromRuinsEnabled", Config.LootEnabled["ruins"]);
        #endregion

        #region Traders
        api.World.Config.SetBool("PaxelTraderSurvivalGoodsEnabled", Config.TraderEnabled["survivalgoods"]);
        api.World.Config.SetBool("PaxelTraderBuildMaterialsEnabled", Config.TraderEnabled["buildmaterials"]);
        api.World.Config.SetBool("PaxelTraderTreasureHunterEnabled", Config.TraderEnabled["treasurehunter"]);
        #endregion

        #region Paxels
        api.World.Config.SetBool("PaxelAndesiteRecipeEnabled", Config.RecipeEnabled["andesite"]);
        api.World.Config.SetBool("PaxelBasaltRecipeEnabled", Config.RecipeEnabled["basalt"]);
        api.World.Config.SetBool("PaxelChertRecipeEnabled", Config.RecipeEnabled["chert"]);
        api.World.Config.SetBool("PaxelGraniteRecipeEnabled", Config.RecipeEnabled["granite"]);
        api.World.Config.SetBool("PaxelPeridotiteRecipeEnabled", Config.RecipeEnabled["peridotite"]);
        api.World.Config.SetBool("PaxelFlintRecipeEnabled", Config.RecipeEnabled["flint"]);
        api.World.Config.SetBool("PaxelObsidianRecipeEnabled", Config.RecipeEnabled["obsidian"]);

        api.World.Config.SetBool("PaxelCopperRecipeEnabled", Config.RecipeEnabled["copper"]);
        api.World.Config.SetBool("PaxelTinbronzeRecipeEnabled", Config.RecipeEnabled["tinbronze"]);
        api.World.Config.SetBool("PaxelBismuthbronzeRecipeEnabled", Config.RecipeEnabled["bismuthbronze"]);
        api.World.Config.SetBool("PaxelBlackbronzeRecipeEnabled", Config.RecipeEnabled["blackbronze"]);

        api.World.Config.SetBool("PaxelSilverRecipeEnabled", Config.RecipeEnabled["silver"]);
        api.World.Config.SetBool("PaxelGoldRecipeEnabled", Config.RecipeEnabled["gold"]);

        api.World.Config.SetBool("PaxelIronRecipeEnabled", Config.RecipeEnabled["iron"]);
        api.World.Config.SetBool("PaxelMeteoricironRecipeEnabled", Config.RecipeEnabled["meteoriciron"]);
        api.World.Config.SetBool("PaxelSteelRecipeEnabled", Config.RecipeEnabled["steel"]);

        api.World.Config.SetBool("PaxelElectrumRecipeEnabled", Config.RecipeEnabled["electrum"]);
        api.World.Config.SetBool("PaxelPlatinumRecipeEnabled", Config.RecipeEnabled["platinum"]);
        api.World.Config.SetBool("PaxelCupronickelRecipeEnabled", Config.RecipeEnabled["cupronickel"]);
        api.World.Config.SetBool("PaxelTitaniumRecipeEnabled", Config.RecipeEnabled["titanium"]);

        api.World.Config.SetBool("PaxelDioriteRecipeEnabled", Config.RecipeEnabled["diorite"]);
        api.World.Config.SetBool("PaxelGabbroRecipeEnabled", Config.RecipeEnabled["gabbro"]);
        api.World.Config.SetBool("PaxelQuartziteRecipeEnabled", Config.RecipeEnabled["quartzite"]);
        #endregion

        //#region Mods
        //api.World.Config.SetBool("PaxelSharpeningEnabled", Config.Paxels.ModDependence["sharpening"]);
        //api.World.Config.SetBool("PaxelHelveSmithingEnabled", Config.Paxels.ModDependence["helvesmithing"]);
        //api.World.Config.SetBool("PaxelJewelryEnabled", Config.Paxels.ModDependence["jewelry"]);
        //api.World.Config.SetBool("PaxelInTreeHollowEnabled", Config.Paxels.ModDependence["intreehollow"]);
        //#endregion

        //#region Loot
        //api.World.Config.SetBool("PaxelLootFromCrackedVesselEnabled", Config.Paxels.LootEnabled["crackedvessel"]);
        //api.World.Config.SetBool("PaxelLootFromRuinsEnabled", Config.Paxels.LootEnabled["ruins"]);
        //#endregion

        //#region Traders
        //api.World.Config.SetBool("PaxelTraderSurvivalGoodsEnabled", Config.Paxels.TraderEnabled["survivalgoods"]);
        //api.World.Config.SetBool("PaxelTraderBuildMaterialsEnabled", Config.Paxels.TraderEnabled["buildmaterials"]);
        //api.World.Config.SetBool("PaxelTraderTreasureHunterEnabled", Config.Paxels.TraderEnabled["treasurehunter"]);
        //#endregion

        //#region Paxels
        //api.World.Config.SetBool("PaxelAndesiteRecipeEnabled", Config.Paxels.RecipeEnabled["andesite"]);
        //api.World.Config.SetBool("PaxelBasaltRecipeEnabled", Config.Paxels.RecipeEnabled["basalt"]);
        //api.World.Config.SetBool("PaxelChertRecipeEnabled", Config.Paxels.RecipeEnabled["chert"]);
        //api.World.Config.SetBool("PaxelGraniteRecipeEnabled", Config.Paxels.RecipeEnabled["granite"]);
        //api.World.Config.SetBool("PaxelPeridotiteRecipeEnabled", Config.Paxels.RecipeEnabled["peridotite"]);
        //api.World.Config.SetBool("PaxelFlintRecipeEnabled", Config.Paxels.RecipeEnabled["flint"]);
        //api.World.Config.SetBool("PaxelObsidianRecipeEnabled", Config.Paxels.RecipeEnabled["obsidian"]);

        //api.World.Config.SetBool("PaxelCopperRecipeEnabled", Config.Paxels.RecipeEnabled["copper"]);
        //api.World.Config.SetBool("PaxelTinbronzeRecipeEnabled", Config.Paxels.RecipeEnabled["tinbronze"]);
        //api.World.Config.SetBool("PaxelBismuthbronzeRecipeEnabled", Config.Paxels.RecipeEnabled["bismuthbronze"]);
        //api.World.Config.SetBool("PaxelBlackbronzeRecipeEnabled", Config.Paxels.RecipeEnabled["blackbronze"]);

        //api.World.Config.SetBool("PaxelSilverRecipeEnabled", Config.Paxels.RecipeEnabled["silver"]);
        //api.World.Config.SetBool("PaxelGoldRecipeEnabled", Config.Paxels.RecipeEnabled["gold"]);

        //api.World.Config.SetBool("PaxelIronRecipeEnabled", Config.Paxels.RecipeEnabled["iron"]);
        //api.World.Config.SetBool("PaxelMeteoricironRecipeEnabled", Config.Paxels.RecipeEnabled["meteoriciron"]);
        //api.World.Config.SetBool("PaxelSteelRecipeEnabled", Config.Paxels.RecipeEnabled["steel"]);

        //api.World.Config.SetBool("PaxelElectrumRecipeEnabled", Config.Paxels.RecipeEnabled["electrum"]);
        //api.World.Config.SetBool("PaxelPlatinumRecipeEnabled", Config.Paxels.RecipeEnabled["platinum"]);
        //api.World.Config.SetBool("PaxelCupronickelRecipeEnabled", Config.Paxels.RecipeEnabled["cupronickel"]);
        //api.World.Config.SetBool("PaxelTitaniumRecipeEnabled", Config.Paxels.RecipeEnabled["titanium"]);

        //api.World.Config.SetBool("PaxelDioriteRecipeEnabled", Config.Paxels.RecipeEnabled["diorite"]);
        //api.World.Config.SetBool("PaxelGabbroRecipeEnabled", Config.Paxels.RecipeEnabled["gabbro"]);
        //api.World.Config.SetBool("PaxelQuartziteRecipeEnabled", Config.Paxels.RecipeEnabled["quartzite"]);
        //#endregion

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

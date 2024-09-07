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

        if (api.ModLoader.IsModEnabled("necessaries") || api.ModLoader.IsModEnabled("necessariesfix") || api.ModLoader.IsModEnabled("stillnecessaries"))
        {
            api.World.Config.SetBool("Paxel.Sharpening.Enabled", Config.ModDependence["sharpening"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.Sharpening.Enabled", false );
        }

        api.World.Config.SetBool("Paxel.HelveSmithing.Enabled", Config.ModDependence["helvesmithing"]);
        api.World.Config.SetBool("Paxel.Jewelry.Enabled", Config.ModDependence["jewelry"]);
        api.World.Config.SetBool("Paxel.InTreeHollow.Enabled", Config.ModDependence["intreehollow"]);
        api.World.Config.SetBool("Paxel.KRPGEnchantment.Enabled", Config.ModDependence["krpgenchantment"]);
        api.World.Config.SetBool("Paxel.ToolsExtended.Enabled", Config.ModDependence["toolsextended"]);
        #endregion

        #region Loot
        api.World.Config.SetBool("Paxel.Loot.CrackedVessel.Enabled", Config.LootEnabled["crackedvessel"]);
        api.World.Config.SetBool("Paxel.Loot.Ruins.Enabled", Config.LootEnabled["ruins"]);
        #endregion

        #region Traders
        api.World.Config.SetBool("Paxel.Trader.Sell.SurvivalGoods.Enabled", Config.TraderEnabled["survivalgoods"]);
        api.World.Config.SetBool("Paxel.Trader.Buy.BuildMaterials.Enabled", Config.TraderEnabled["buildmaterials"]);
        api.World.Config.SetBool("Paxel.Trader.Buy.TreasureHunter.Enabled", Config.TraderEnabled["treasurehunter"]);
        #endregion

        #region Paxels
        api.World.Config.SetBool("Paxel.Andesite.Recipe.Enabled", Config.RecipeEnabled["andesite"]);
        api.World.Config.SetBool("Paxel.Basalt.Recipe.Enabled", Config.RecipeEnabled["basalt"]);
        api.World.Config.SetBool("Paxel.Chert.Recipe.Enabled", Config.RecipeEnabled["chert"]);
        api.World.Config.SetBool("Paxel.Flint.Recipe.Enabled", Config.RecipeEnabled["flint"]);
        api.World.Config.SetBool("Paxel.Granite.Recipe.Enabled", Config.RecipeEnabled["granite"]);
        api.World.Config.SetBool("Paxel.Obsidian.Recipe.Enabled", Config.RecipeEnabled["obsidian"]);
        api.World.Config.SetBool("Paxel.Peridotite.Recipe.Enabled", Config.RecipeEnabled["peridotite"]);

        api.World.Config.SetBool("Paxel.Copper.Recipe.Enabled", Config.RecipeEnabled["copper"]);
        api.World.Config.SetBool("Paxel.Tinbronze.Recipe.Enabled", Config.RecipeEnabled["tinbronze"]);
        api.World.Config.SetBool("Paxel.Bismuthbronze.Recipe.Enabled", Config.RecipeEnabled["bismuthbronze"]);
        api.World.Config.SetBool("Paxel.Blackbronze.Recipe.Enabled", Config.RecipeEnabled["blackbronze"]);

        api.World.Config.SetBool("Paxel.Silver.Recipe.Enabled", Config.RecipeEnabled["silver"]);
        api.World.Config.SetBool("Paxel.Gold.Recipe.Enabled", Config.RecipeEnabled["gold"]);

        api.World.Config.SetBool("Paxel.Iron.Recipe.Enabled", Config.RecipeEnabled["iron"]);
        api.World.Config.SetBool("Paxel.Meteoriciron.Recipe.Enabled", Config.RecipeEnabled["meteoriciron"]);
        api.World.Config.SetBool("Paxel.Steel.Recipe.Enabled", Config.RecipeEnabled["steel"]);

        api.World.Config.SetBool("Paxel.Electrum.Recipe.Enabled", Config.RecipeEnabled["electrum"]);
        api.World.Config.SetBool("Paxel.Platinum.Recipe.Enabled", Config.RecipeEnabled["platinum"]);
        api.World.Config.SetBool("Paxel.Cupronickel.Recipe.Enabled", Config.RecipeEnabled["cupronickel"]);
        api.World.Config.SetBool("Paxel.Uranium.Recipe.Enabled", Config.RecipeEnabled["uranium"]);

        if (api.ModLoader.IsModEnabled("geoaddons"))
        {
            api.World.Config.SetBool("Paxel.Diorite.Recipe.Enabled", Config.RecipeEnabled["diorite"]);
            api.World.Config.SetBool("Paxel.Gabbro.Recipe.Enabled", Config.RecipeEnabled["gabbro"]);
            api.World.Config.SetBool("Paxel.Quartzite.Recipe.Enabled", Config.RecipeEnabled["quartzite"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.Diorite.Recipe.Enabled", false);
            api.World.Config.SetBool("Paxel.Gabbro.Recipe.Enabled", false);
            api.World.Config.SetBool("Paxel.Quartzite.Recipe.Enabled", false);
        }

        if (api.ModLoader.IsModEnabled("toolsextended"))
        {
            api.World.Config.SetBool("Paxel.StainlessSteel.Recipe.Enabled", Config.RecipeEnabled["stainlesssteel"]);
            api.World.Config.SetBool("Paxel.Titanium.Recipe.Enabled", Config.RecipeEnabled["titanium"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.StainlessSteel.Recipe.Enabled", false);
            api.World.Config.SetBool("Paxel.Titanium.Recipe.Enabled", false);
        }

        if (api.ModLoader.IsModEnabled("legendarymobs"))
        {
            api.World.Config.SetBool("Paxel.Legendary.Recipe.Enabled", Config.RecipeEnabled["legendary"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.Legendary.Recipe.Enabled", false);
        }
        #endregion

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

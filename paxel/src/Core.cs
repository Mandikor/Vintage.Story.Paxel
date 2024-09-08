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

        // MOD [Still]Necessaries[fix] - sharpening the paxel on the grindstone
        if (api.ModLoader.IsModEnabled("necessaries") || api.ModLoader.IsModEnabled("necessariesfix") || api.ModLoader.IsModEnabled("stillnecessaries"))
        {
            api.World.Config.SetBool("Paxel.Mod.Enabled.Necessaries", Config.ModDependence["sharpening"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.Mod.Enabled.Necessaries", false );
        }

        // MOD Helve Hammer Extensions - automatic smithing of the paxelhead
        api.World.Config.SetBool("Paxel.Mod.Enabled.HelveHammerExtensions", Config.ModDependence["helvesmithing"]);

        // MOD Tools Extended - recipes for titanium and stainless steel paxel
        api.World.Config.SetBool("Paxel.Mod.Enabled.ToolsExtended", Config.ModDependence["toolsextended"]);

        // MOD CAN Jewlry - improve paxels with buffs through gemstones
        api.World.Config.SetBool("Paxel.Mod.Enabled.CANJewelry", Config.ModDependence["jewelry"]);

        // MOD KRPG Enchantment - improve paxels with enchantments
        api.World.Config.SetBool("Paxel.Mod.Enabled.KRPGEnchantment", Config.ModDependence["krpgenchantment"]);

        #endregion

        #region Loot

        // Vanilla - flint, obsidian and copper paxel in cracked tool vessel
        api.World.Config.SetBool("Paxel.Loot.Enabled.CrackedVessel", Config.LootEnabled["crackedvessel"]);

        // Vanilla - obsidian and copper paxel in old chest
        api.World.Config.SetBool("Paxel.Loot.Enabled.Ruins", Config.LootEnabled["ruins"]);

        // MOD Primitive Survival - flint, obsidian and copper paxel in tree hollows
        api.World.Config.SetBool("Paxel.Mod.Enabled.PrimitiveSurvival", Config.ModDependence["intreehollow"]);

        // MOD Battle Towers - paxel for each loot tier
        api.World.Config.SetBool("Paxel.Loot.Enabled.BattleTowers", Config.LootEnabled["battletowers"]);

        #endregion

        #region Traders

        // Vanilla - selling copper, tinbronze, bismuthbronze and blackbronze paxel to trader
        api.World.Config.SetBool("Paxel.Trader.Enabled.SurvivalGoods", Config.TraderEnabled["survivalgoods"]);

        // Vanilla - buying copper, tinbronze, bismuthbronze and blackbronze paxel from trader
        api.World.Config.SetBool("Paxel.Trader.Enabled.BuildMaterials", Config.TraderEnabled["buildmaterials"]);

        // Vanilla - buying copper, tinbronze, bismuthbronze and blackbronze paxel from trader
        api.World.Config.SetBool("Paxel.Trader.Enabled.TreasureHunter", Config.TraderEnabled["treasurehunter"]);

        #endregion

        #region Paxels

        // Vanilla - recipes for all standard paxels
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Andesite", Config.RecipeEnabled["andesite"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Basalt", Config.RecipeEnabled["basalt"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Chert", Config.RecipeEnabled["chert"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Flint", Config.RecipeEnabled["flint"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Granite", Config.RecipeEnabled["granite"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Obsidian", Config.RecipeEnabled["obsidian"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Peridotite", Config.RecipeEnabled["peridotite"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Copper", Config.RecipeEnabled["copper"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Tinbronze", Config.RecipeEnabled["tinbronze"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Bismuthbronze", Config.RecipeEnabled["bismuthbronze"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Blackbronze", Config.RecipeEnabled["blackbronze"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Silver", Config.RecipeEnabled["silver"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Gold", Config.RecipeEnabled["gold"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Electrum", Config.RecipeEnabled["electrum"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Iron", Config.RecipeEnabled["iron"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Meteoriciron", Config.RecipeEnabled["meteoriciron"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Steel", Config.RecipeEnabled["steel"]);
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Cupronickel", Config.RecipeEnabled["cupronickel"]);

        // Vanilla - recipes for future standard paxels, currently not yet producible
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Platinum", Config.RecipeEnabled["platinum"]); // Anvil tier 5
        api.World.Config.SetBool("Paxel.Recipe.Enabled.Uranium", Config.RecipeEnabled["uranium"]); // Anvil tier 7

        // MOD Geology Additions - recipes for diorite, gabbro and quartzite paxel
        if (api.ModLoader.IsModEnabled("geoaddons"))
        {
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Diorite", Config.RecipeEnabled["diorite"]);
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Gabbro", Config.RecipeEnabled["gabbro"]);
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Quartzite", Config.RecipeEnabled["quartzite"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Diorite", false);
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Gabbro", false);
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Quartzite", false);
        }

        // MOD Tools Extended - recipes for titanium and stainless steel paxel
        if (api.ModLoader.IsModEnabled("toolsextended"))
        {
            api.World.Config.SetBool("Paxel.Recipe.Enabled.StainlessSteel", Config.RecipeEnabled["stainlesssteel"]);
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Titanium", Config.RecipeEnabled["titanium"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.Recipe.Enabled.StainlessSteel", false);
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Titanium", false);
        }

        // MOD Legendary Mobs - recipes for legendary paxel
        if (api.ModLoader.IsModEnabled("legendarymobs"))
        {
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Legendary", Config.RecipeEnabled["legendary"]);
        }
        else
        {
            api.World.Config.SetBool("Paxel.Recipe.Enabled.Legendary", false);
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

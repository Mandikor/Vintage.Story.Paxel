using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Paxel.Configuration;

public class Config : IModConfig
{

    public Dictionary<string, bool> RecipeEnabled = new()
    {
        { "andesite", true },
        { "basalt", true },
        { "chert", true },
        { "granite", true },
        { "peridotite", true },
        { "flint", true },
        { "obsidian", true },
        { "copper", true },
        { "tinbronze", true },
        { "bismuthbronze", true },
        { "blackbronze", true },
        { "silver", true },
        { "gold", true },
        { "iron", true },
        { "meteoriciron", true },
        { "steel", true },
        { "electrum", true },
        { "platinum", true },
        { "uranium", true },
        { "cupronickel", true },
        { "diorite", true },
        { "gabbro", true },
        { "quartzite", true },
        { "stainlesssteel", true },
        { "titanium", true },
        { "legendary", false }
    };

    public Dictionary<string, bool> LootEnabled = new()
    {
        { "crackedvessel", true },
        { "ruins", true },
        { "primitivesurvival", true },
        { "battletowers", true }
    };

    public Dictionary<string, bool> TraderEnabled = new()
    {
        { "buildmaterials", true },
        { "survivalgoods", true },
        { "treasurehunter", true }
    };

    public Dictionary<string, bool> ModDependence = new()
    {
        { "necessaries", true },
        { "helvehammerextensions", true },
        { "canjewelry", true },
        { "krpgenchantment", true },
        { "toolsextended", true }
    };

    public Config(ICoreAPI api, Config previousConfig = null)
    {
        if (previousConfig == null) { return; }

        ModDependence["necessaries"] = previousConfig.ModDependence["necessaries"] ? previousConfig.ModDependence["necessaries"] : true;
        ModDependence["helvehammerextensions"] = previousConfig.ModDependence["helvehammerextensions"] ? previousConfig.ModDependence["helvehammerextensions"] : true;
        ModDependence["canjewelry"] = previousConfig.ModDependence["canjewelry"] ? previousConfig.ModDependence["canjewelry"] : true;
        ModDependence["krpgenchantment"] = previousConfig.ModDependence["krpgenchantment"] ? previousConfig.ModDependence["krpgenchantment"] : true;
        ModDependence["toolsextended"] = previousConfig.ModDependence["toolsextended"] ? previousConfig.ModDependence["toolsextended"] : true;

        LootEnabled["crackedvessel"] = previousConfig.LootEnabled["crackedvessel"] ? previousConfig.LootEnabled["crackedvessel"] : true;
        LootEnabled["ruins"] = previousConfig.LootEnabled["ruins"] ? previousConfig.LootEnabled["ruins"] : true;
        LootEnabled["primitivesurvival"] = previousConfig.LootEnabled["primitivesurvival"] ? previousConfig.LootEnabled["primitivesurvival"] : true;
        LootEnabled["battletowers"] = previousConfig.LootEnabled["battletowers"] ? previousConfig.LootEnabled["battletowers"] : true;

        TraderEnabled["survivalgoods"] = previousConfig.TraderEnabled["survivalgoods"] ? previousConfig.TraderEnabled["survivalgoods"] : true;
        TraderEnabled["buildmaterials"] = previousConfig.TraderEnabled["buildmaterials"] ? previousConfig.TraderEnabled["buildmaterials"] : true;
        TraderEnabled["treasurehunter"] = previousConfig.TraderEnabled["treasurehunter"] ? previousConfig.TraderEnabled["treasurehunter"] : true;

        RecipeEnabled["andesite"] = previousConfig.RecipeEnabled["andesite"] ? previousConfig.RecipeEnabled["andesite"] : true;
        RecipeEnabled["basalt"] = previousConfig.RecipeEnabled["basalt"] ? previousConfig.RecipeEnabled["basalt"] : true;
        RecipeEnabled["chert"] = previousConfig.RecipeEnabled["chert"] ? previousConfig.RecipeEnabled["chert"] : true;
        RecipeEnabled["granite"] = previousConfig.RecipeEnabled["granite"] ? previousConfig.RecipeEnabled["granite"] : true;
        RecipeEnabled["peridotite"] = previousConfig.RecipeEnabled["peridotite"] ? previousConfig.RecipeEnabled["peridotite"] : true;
        RecipeEnabled["flint"] = previousConfig.RecipeEnabled["flint"] ? previousConfig.RecipeEnabled["flint"] : true;
        RecipeEnabled["obsidian"] = previousConfig.RecipeEnabled["obsidian"] ? previousConfig.RecipeEnabled["obsidian"] : true;
        RecipeEnabled["copper"] = previousConfig.RecipeEnabled["copper"] ? previousConfig.RecipeEnabled["copper"] : true;
        RecipeEnabled["tinbronze"] = previousConfig.RecipeEnabled["tinbronze"] ? previousConfig.RecipeEnabled["tinbronze"] : true;
        RecipeEnabled["bismuthbronze"] = previousConfig.RecipeEnabled["bismuthbronze"] ? previousConfig.RecipeEnabled["bismuthbronze"] : true;
        RecipeEnabled["blackbronze"] = previousConfig.RecipeEnabled["blackbronze"] ? previousConfig.RecipeEnabled["blackbronze"] : true;
        RecipeEnabled["silver"] = previousConfig.RecipeEnabled["silver"] ? previousConfig.RecipeEnabled["silver"] : true;
        RecipeEnabled["gold"] = previousConfig.RecipeEnabled["gold"] ? previousConfig.RecipeEnabled["gold"] : true;
        RecipeEnabled["iron"] = previousConfig.RecipeEnabled["iron"] ? previousConfig.RecipeEnabled["iron"] : true;
        RecipeEnabled["meteoriciron"] = previousConfig.RecipeEnabled["meteoriciron"] ? previousConfig.RecipeEnabled["meteoriciron"] : true;
        RecipeEnabled["steel"] = previousConfig.RecipeEnabled["steel"] ? previousConfig.RecipeEnabled["steel"] : true;
        RecipeEnabled["electrum"] = previousConfig.RecipeEnabled["electrum"] ? previousConfig.RecipeEnabled["electrum"] : true;
        RecipeEnabled["platinum"] = previousConfig.RecipeEnabled["platinum"] ? previousConfig.RecipeEnabled["platinum"] : true;
        RecipeEnabled["cupronickel"] = previousConfig.RecipeEnabled["cupronickel"] ? previousConfig.RecipeEnabled["cupronickel"] : true;
        RecipeEnabled["uranium"] = previousConfig.RecipeEnabled["uranium"] ? previousConfig.RecipeEnabled["uranium"] : true;
        RecipeEnabled["diorite"] = previousConfig.RecipeEnabled["diorite"] ? previousConfig.RecipeEnabled["diorite"] : true;
        RecipeEnabled["gabbro"] = previousConfig.RecipeEnabled["gabbro"] ? previousConfig.RecipeEnabled["gabbro"] : true;
        RecipeEnabled["quartzite"] = previousConfig.RecipeEnabled["quartzite"] ? previousConfig.RecipeEnabled["quartzite"] : true;
        RecipeEnabled["stainlesssteel"] = previousConfig.RecipeEnabled["stainlesssteel"] ? previousConfig.RecipeEnabled["stainlesssteel"] : true;
        RecipeEnabled["titanium"] = previousConfig.RecipeEnabled["titanium"] ? previousConfig.RecipeEnabled["titanium"] : true;
        RecipeEnabled["legendary"] = previousConfig.RecipeEnabled["legendary"] ? previousConfig.RecipeEnabled["legendary"] : false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using static HarmonyLib.Code;

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
        { "titanium", true }//,
        //{ "legendary", false }
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
        if (previousConfig == null)
        {
            return;
        }
        
        ModDependence["necessaries"] = previousConfig.ModDependence["necessaries"];
        ModDependence["helvehammerextensions"] = previousConfig.ModDependence["helvehammerextensions"];
        ModDependence["canjewelry"] = previousConfig.ModDependence["canjewelry"];
        ModDependence["krpgenchantment"] = previousConfig.ModDependence["krpgenchantment"];
        ModDependence["toolsextended"] = previousConfig.ModDependence["toolsextended"];

        LootEnabled["crackedvessel"] = previousConfig.LootEnabled["crackedvessel"];
        LootEnabled["ruins"] = previousConfig.LootEnabled["ruins"];
        LootEnabled["primitivesurvival"] = previousConfig.LootEnabled["primitivesurvival"];
        LootEnabled["battletowers"] = previousConfig.LootEnabled["battletowers"];

        TraderEnabled["survivalgoods"] = previousConfig.TraderEnabled["survivalgoods"];
        TraderEnabled["buildmaterials"] = previousConfig.TraderEnabled["buildmaterials"];
        TraderEnabled["treasurehunter"] = previousConfig.TraderEnabled["treasurehunter"];

        RecipeEnabled["andesite"] = previousConfig.RecipeEnabled["andesite"];
        RecipeEnabled["basalt"] = previousConfig.RecipeEnabled["basalt"];
        RecipeEnabled["chert"] = previousConfig.RecipeEnabled["chert"];
        RecipeEnabled["granite"] = previousConfig.RecipeEnabled["granite"];
        RecipeEnabled["peridotite"] = previousConfig.RecipeEnabled["peridotite"];
        RecipeEnabled["flint"] = previousConfig.RecipeEnabled["flint"];
        RecipeEnabled["obsidian"] = previousConfig.RecipeEnabled["obsidian"];
        RecipeEnabled["copper"] = previousConfig.RecipeEnabled["copper"];
        RecipeEnabled["tinbronze"] = previousConfig.RecipeEnabled["tinbronze"];
        RecipeEnabled["bismuthbronze"] = previousConfig.RecipeEnabled["bismuthbronze"];
        RecipeEnabled["blackbronze"] = previousConfig.RecipeEnabled["blackbronze"];
        RecipeEnabled["silver"] = previousConfig.RecipeEnabled["silver"];
        RecipeEnabled["gold"] = previousConfig.RecipeEnabled["gold"];
        RecipeEnabled["iron"] = previousConfig.RecipeEnabled["iron"];
        RecipeEnabled["meteoriciron"] = previousConfig.RecipeEnabled["meteoriciron"];
        RecipeEnabled["steel"] = previousConfig.RecipeEnabled["steel"];
        RecipeEnabled["electrum"] = previousConfig.RecipeEnabled["electrum"];
        RecipeEnabled["platinum"] = previousConfig.RecipeEnabled["platinum"];
        RecipeEnabled["cupronickel"] = previousConfig.RecipeEnabled["cupronickel"];
        RecipeEnabled["uranium"] = previousConfig.RecipeEnabled["uranium"];
        RecipeEnabled["diorite"] = previousConfig.RecipeEnabled["diorite"];
        RecipeEnabled["gabbro"] = previousConfig.RecipeEnabled["gabbro"];
        RecipeEnabled["quartzite"] = previousConfig.RecipeEnabled["quartzite"];
        RecipeEnabled["stainlesssteel"] = previousConfig.RecipeEnabled["stainlesssteel"];
        RecipeEnabled["titanium"] = previousConfig.RecipeEnabled["titanium"];
        //RecipeEnabled["legendary"] = previousConfig.RecipeEnabled["legendary"];
    }

}

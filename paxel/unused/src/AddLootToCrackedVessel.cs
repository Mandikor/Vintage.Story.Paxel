using HarmonyLib;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

[HarmonyPatch(typeof(BlockLootVessel))]
[HarmonyPatch("GetLootItems")]
public class BlockLootVesselPatch
{
    static void Postfix(BlockLootVessel __instance, ref ItemStack[] __result, ref float __state)
    {
        // Erstelle eine neue Liste von ItemStacks
        List<ItemStack> newLootList = new List<ItemStack>();

        // Füge die vorhandenen Items zur neuen Liste hinzu
        newLootList.AddRange(__result);

        // Füge hier zusätzliche LootItems zur Liste hinzu
        ItemStack myItemStack = new ItemStack(__instance.Api.World.GetItem(new AssetLocation("myitemcode")), 1);
        newLootList.Add(myItemStack);

        // Setze das Ergebnis auf die neue Liste
        __result = newLootList.ToArray();
    }
}


lootLists["tool"] = LootList.Create(2.2f, LootItem.Item(1f, 1f, 1f, "axe-flint"), LootItem.Item(1f, 1f, 1f, "shovel-flint"), LootItem.Item(1f, 1f, 1f, "knife-generic-flint"), LootItem.Item(0.1f, 1f, 1f, "axe-felling-copper", "axe-felling-copper", "axe-felling-tinbronze"), LootItem.Item(0.1f, 1f, 1f, "shovel-copper", "shovel-copper", "shovel-tinbronze"), LootItem.Item(0.1f, 1f, 1f, "pickaxe-copper"), LootItem.Item(0.1f, 1f, 1f, "scythe-copper"), LootItem.Item(0.1f, 1f, 1f, "knife-generic-copper", "knife-generic-copper", "knife-generic-tinbronze"), LootItem.Item(0.1f, 1f, 1f, "blade-falx-copper", "blade-falx-copper", "blade-falx-tinbronze"), LootItem.Item(0.1f, 2f, 4f, "gear-rusty"), LootItem.Item(0.8f, 1f, 1f, "paxel:paxel-flint"), LootItem.Item(0.4f, 1f, 1f, "paxel:paxel-obsidian"), LootItem.Item(0.1f, 1f, 1f, "paxel:paxel-copper", "paxel:paxel-copper", "paxel:paxel-tinbronze"));

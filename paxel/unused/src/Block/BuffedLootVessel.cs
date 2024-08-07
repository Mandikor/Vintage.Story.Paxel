using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Paxel.Patches
{
    public class BuffedBlockLootVessel : BlockLootVessel
    {
        new static readonly Dictionary<string, LootList> lootLists = new();
        static BuffedBlockLootVessel()
        {
            lootLists["seed"] = LootList.Create(1f,
                                                LootItem.Item(1f, 5f, 7f, "seeds-carrot", "seeds-onion", "seeds-spelt", "seeds-turnip", "seeds-rice", "seeds-rye", "seeds-soybean", "seeds-pumpkin", "seeds-cabbage"));
            lootLists["food"] = LootList.Create(1f,
                                                LootItem.Item(3f, 8f, 15f, "grain-spelt", "grain-rice", "grain-flax", "grain-rye"),
                                                LootItem.Item(0.1f, 1f, 1f, "tuningcylinder-1", "tuningcylinder-2", "tuningcylinder-3", "tuningcylinder-4", "tuningcylinder-5", "tuningcylinder-6", "tuningcylinder-7", "tuningcylinder-8", "tuningcylinder-9"));
            lootLists["forage"] = LootList.Create(2.5f,
                                                  LootItem.Item(1f, 2f, 6f, "flint"),
                                                  LootItem.Item(1f, 3f, 9f, "stick"),
                                                  LootItem.Item(1f, 3f, 16f, "drygrass"),
                                                  LootItem.Item(1f, 3f, 24f, "stone-andesite", "stone-chalk", "stone-claystone", "stone-granite", "stone-sandstone", "stone-shale"),
                                                  LootItem.Item(1f, 3f, 24f, "clay-blue", "clay-fire"),
                                                  LootItem.Item(1f, 3f, 24f, "cattailtops"),
                                                  LootItem.Item(1f, 1f, 4f, "poultice-linen-horsetail"),
                                                  LootItem.Item(0.5f, 1f, 12f, "flaxfibers"),
                                                  LootItem.Item(0.3f, 1f, 3f, "honeycomb"),
                                                  LootItem.Item(0.3f, 2f, 6f, "bamboostakes"),
                                                  LootItem.Item(0.3f, 2f, 6f, "beenade-closed"));
            lootLists["ore"] = LootList.Create(1.35f,
                                               LootItem.Item(1f, 2f, 12f, "ore-lignite", "ore-bituminouscoal"),
                                               LootItem.Item(1f, 2f, 8f, "nugget-nativecopper", "ore-quartz", "nugget-galena"),
                                               LootItem.Item(0.2f, 4f, 12f, "nugget-galena", "nugget-cassiterite", "nugget-sphalerite", "nugget-bismuthinite"),
                                               LootItem.Item(0.1f, 4f, 12f, "nugget-limonite", "nugget-nativegold", "nugget-chromite", "nugget-ilmenite", "nugget-nativesilver", "nugget-magnetite"));
            lootLists["tool"] = LootList.Create(2.2f,
                                                LootItem.Item(1f, 1f, 1f, "axe-flint"),
                                                LootItem.Item(1f, 1f, 1f, "shovel-flint"),
                                                LootItem.Item(1f, 1f, 1f, "knife-generic-flint"),
                                                LootItem.Item(0.8f, 1f, 1f, "paxel:paxel-flint"),
                                                LootItem.Item(0.4f, 1f, 1f, "paxel:paxel-obsidian"),
                                                LootItem.Item(0.1f, 1f, 1f, "axe-felling-copper", "axe-felling-copper", "axe-felling-tinbronze"),
                                                LootItem.Item(0.1f, 1f, 1f, "shovel-copper", "shovel-copper", "shovel-tinbronze"),
                                                LootItem.Item(0.1f, 1f, 1f, "pickaxe-copper"),
                                                LootItem.Item(0.1f, 1f, 1f, "scythe-copper"),
                                                LootItem.Item(0.1f, 1f, 1f, "knife-generic-copper", "knife-generic-copper", "knife-generic-tinbronze"),
                                                LootItem.Item(0.1f, 1f, 1f, "blade-falx-copper", "blade-falx-copper", "blade-falx-tinbronze"),
                                                LootItem.Item(0.1f, 1f, 1f, "paxel:paxel-copper", "paxel:paxel-copper", "paxel:paxel-tinbronze"),
                                                LootItem.Item(0.1f, 2f, 4f, "gear-rusty"));
            lootLists["farming"] = LootList.Create(2.5f,
                                                   LootItem.Item(0.1f, 1f, 1f, "linensack"),
                                                   LootItem.Item(0.5f, 1f, 1f, "basket"),
                                                   LootItem.Item(0.75f, 3f, 10f, "feather"),
                                                   LootItem.Item(0.75f, 2f, 10f, "flaxfibers"),
                                                   LootItem.Item(0.35f, 2f, 10f, "flaxtwine"),
                                                   LootItem.Item(0.75f, 2f, 4f, "seeds-cabbage"),
                                                   LootItem.Item(0.75f, 5f, 10f, "cattailtops"),
                                                   LootItem.Item(0.1f, 1f, 1f, "scythe-copper", "scythe-tinbronze"));
            lootLists["arcticsupplies"] = LootList.Create(2.5f,
                                                          LootItem.Item(1.5f, 6f, 24f, "cattailtops"),
                                                          LootItem.Item(1.5f, 6f, 16f, "drygrass"),
                                                          LootItem.Item(1f, 4f, 7f, "flint"),
                                                          LootItem.Item(1f, 6f, 12f, "stick"),
                                                          LootItem.Item(1f, 10f, 24f, "stone-andesite", "stone-chalk", "stone-granite", "stone-basalt"),
                                                          LootItem.Block(1f, 4f, 24f, "soil-medium-none"),
                                                          LootItem.Item(1f, 4f, 24f, "clay-blue", "clay-fire"),
                                                          LootItem.Item(0.5f, 1f, 4f, "poultice-linen-horsetail"),
                                                          LootItem.Item(0.5f, 1f, 12f, "flaxfibers"),
                                                          LootItem.Item(0.3f, 1f, 3f, "honeycomb"),
                                                          LootItem.Item(0.1f, 2f, 4f, "gear-rusty"));
        }
        public override float OnGettingBroken(IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter)
        {
            return base.OnGettingBroken(player, blockSel, itemslot, remainingResistance, dt, counter);
        }
        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
        {
            base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
        }
        public override BlockDropItemStack[] GetDropsForHandbook(ItemStack handbookStack, IPlayer forPlayer)
        {
            LootList list = lootLists[LastCodePart()];
            List<BlockDropItemStack> drops = new();
            foreach (LootItem val in list.lootItems)
            {
                for (int i = 0; i < val.codes.Length; i++)
                {
                    ItemStack itemStack = val.GetItemStack(api.World, i, 1f);
                    if (itemStack == null)
                    {
                        AssetLocation assetLocation = val.codes[i % val.codes.Length];
                        api.World.Logger.Warning("Unable to resolve loot vessel drop {0}, item wont drop.", assetLocation);
                        continue;
                    }

                    BlockDropItemStack stack = new(val.GetItemStack(this.api.World, i, 1f), 1f);
                    if (stack != null)
                    {
                        stack.Quantity.avg = val.chance / list.TotalChance / (float)val.codes.Length;
                        if (drops.FirstOrDefault((BlockDropItemStack dstack) => dstack.ResolvedItemstack.Equals(api.World, stack.ResolvedItemstack, GlobalConstants.IgnoredStackAttributes)) == null)
                        {
                            drops.Add(stack);
                        }
                    }
                }
            }
            return drops.ToArray();
        }
        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
        {
            float selfdropRate = ((byPlayer != null) ? byPlayer.Entity.Stats.GetBlended("wholeVesselLootChance") : 0f) - 1f;
            if (this.api.World.Rand.NextDouble() < (double)selfdropRate)
            {
                return new ItemStack[]
                {
                    new ItemStack(this, 1)
                };
            }
            LootList list = lootLists[base.LastCodePart(0)];
            if (list == null)
            {
                return System.Array.Empty<ItemStack>();
            }
            return list.GenerateLoot(world, byPlayer);
        }

    }

}

namespace Paxel
{
    public class Config
    {

        public PaxelData Paxels = new();

        public Config() { }

        public Config(Config previousConfig)
        {
            Paxels = previousConfig.Paxels;
        }

    }
    public class PaxelData
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
            { "cupronickel", true },
            { "titanium", true },
            { "diorite", true },
            { "gabbro", true },
            { "quartzite", true }
        };

        public Dictionary<string, bool> LootEnabled = new()
        {
            { "crackedvessel", true },
            { "ruins", true }
        };

        public Dictionary<string, bool> TraderEnabled = new()
        {
            { "buildmaterials", true },
            { "survivalgoods", true },
            { "treasurehunter", true }
        };

        public Dictionary<string, bool> ModDependence = new()
        {
            { "sharpening", true },
            { "helvesmithing", true },
            { "jewelry", true },
            { "intreehollow", true }
        };
    }
}
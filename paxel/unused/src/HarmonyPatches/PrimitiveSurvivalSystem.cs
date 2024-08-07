namespace Paxel
{
    public class PrimitiveSurvivalSystem : ModSystem
    {
        private ICoreClientAPI capi;


        private readonly Harmony harmony = new("Mandikor.Paxel");

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);

            var PSBlockGetPlacedBlockInfoOriginal = typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo));
            var PSBlockGetPlacedBlockInfoPostfix = typeof(PS_BlockGetPlacedBlockInfo_Patch).GetMethod(nameof(PS_BlockGetPlacedBlockInfo_Patch.PSBlockGetPlacedBlockInfoPostfix));
            this.harmony.Patch(PSBlockGetPlacedBlockInfoOriginal, postfix: new HarmonyMethod(PSBlockGetPlacedBlockInfoPostfix));

            var PSCollectibleGetHeldItemInfoOriginal = typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo));
            var PSCollectibleGetHeldItemInfoPostfix = typeof(PS_CollectibleGetHeldItemInfo_Patch).GetMethod(nameof(PS_CollectibleGetHeldItemInfo_Patch.PSCollectibleGetHeldItemInfoPostfix));
            this.harmony.Patch(PSCollectibleGetHeldItemInfoOriginal, postfix: new HarmonyMethod(PSCollectibleGetHeldItemInfoPostfix));

            this.capi = api;
        }

        public override void Dispose()
        {
            var PSBlockGetPlacedBlockInfoOriginal = typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo));
            this.harmony.Unpatch(PSBlockGetPlacedBlockInfoOriginal, HarmonyPatchType.Postfix, "*");

            var PSCollectibleGetHeldItemInfoOriginal = typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo));
            this.harmony.Unpatch(PSCollectibleGetHeldItemInfoOriginal, HarmonyPatchType.Postfix, "*");

            base.Dispose();
        }

        // display mod name in the hud for blocks
        public class PS_BlockGetPlacedBlockInfo_Patch
        {
            [HarmonyPostfix]
            public static void PSBlockGetPlacedBlockInfoPostfix(ref string __result, IPlayer forPlayer)
            {
                var domain = forPlayer.Entity?.BlockSelection?.Block?.Code?.Domain;
                if (domain != null)
                {
                    if (domain == "paxel")
                    {
                        __result += "\n\n<font color=\"#D8EAA3\"><i>Paxel</i></font>\n\n";
                    }
                }
            }
        }

        public class PS_CollectibleGetHeldItemInfo_Patch
        {
            [HarmonyPostfix]
            public static void PSCollectibleGetHeldItemInfoPostfix(ItemSlot inSlot, StringBuilder dsc)
            {
                var domain = inSlot.Itemstack?.Collectible?.Code?.Domain;
                if (domain != null)
                {
                    if (domain == "paxel")
                    {
                        dsc.AppendLine("\n<font color=\"#D8EAA3\"><i>Paxel</i></font>");
                    }
                }
            }
        }
    }
}







using HarmonyLib;

namespace Paxel
{
    class VanillaPatches : ModSystem
    {
        public const string patchCode = "Mandikor.Paxel.VanillaPatches";
        public Harmony harmonyInstance = new(patchCode);

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            harmonyInstance.Patch(original: typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo)), postfix: typeof(GetPlacedBlockInfoPatch).GetMethod(nameof(GetPlacedBlockInfoPatch.Postfix)));
            harmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo)), postfix: typeof(GetHeldItemInfoPatch).GetMethod(nameof(GetHeldItemInfoPatch.Postfix)));
        }

        public override void Dispose()
        {
            harmonyInstance.Unpatch(original: typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo)), type: HarmonyPatchType.All, patchCode);
            harmonyInstance.Unpatch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo)), type: HarmonyPatchType.All, patchCode);
            base.Dispose();
        }
    }
}
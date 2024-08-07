using HarmonyLib;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Common;

namespace Paxel
{
    [HarmonyPatch(typeof(CollectibleObject))]
    [HarmonyPatch("GetHeldTpHitAnimation")]
    public class GetHeldTpHitAnimationPatches
    {

        [HarmonyPostfix]
        public static string SetTpHitAnim(ItemSlot slot, Entity byEntity, ref string __result)
        {
            
            Block block = byEntity.World.BlockAccessor.GetBlock();

            __result = "breakhand";

            if (block.BlockMaterial != null)
            {
                switch (block.BlockMaterial)
                {
                    case (block.BlockMaterial == EnumBlockMaterial.Stone):
                        __result = "smithing";
                        break;
                    case (block.BlockMaterial == EnumBlockMaterial.Wood):
                        __result = "axechop";
                        break;

                    case (block.BlockMaterial == EnumBlockMaterial.Soil):
                        __result = "shoveldig";
                        break;

                }
            }

            return __result;
        }
    }
}
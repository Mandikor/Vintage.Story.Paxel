using HarmonyLib;
using Vintagestory.GameContent;

namespace Paxel
{
    [HarmonyPatch(typeof(ItemWorkItem))]
    [HarmonyPatch("GetHelveWorkableMode")]
    public class GetHelveWorkableModePatch
    {
        public static bool Prefix( ref EnumHelveWorkableMode __result, ref BlockEntityAnvil beAnvil)
        {
            bool workable = true;
            var attr = beAnvil.SelectedRecipe.Output.Attributes;
            if (attr != null)
            {
                workable = attr["helvehammersmithable"].AsBool();
            }

            if (workable && beAnvil.OwnMetalTier >= 2)
            {
                __result = EnumHelveWorkableMode.TestSufficientVoxelsWorkable;
                return false;
            }

            return true;
        }
    }
}
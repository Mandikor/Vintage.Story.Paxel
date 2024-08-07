namespace Paxel
{
    [HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
    public class GetHeldItemInfoPatch
    {
        public static void Postfix(ItemSlot inSlot, StringBuilder dsc)
        {
            var domain = inSlot.Itemstack?.Collectible?.Code?.Domain;
            if (domain != null)
            {
                if (domain == "paxel")
                {
                    dsc.AppendLine("\n<font color=\"#84ef8a\"><i>Paxel</i></font>");
                }
            }
        }
    }
}

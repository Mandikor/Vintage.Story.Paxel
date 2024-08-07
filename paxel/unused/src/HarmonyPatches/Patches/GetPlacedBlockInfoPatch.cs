namespace Paxel
{
    [HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
    public class GetPlacedBlockInfoPatch
    {
        public static void Postfix(ref string __result, IPlayer forPlayer)
        {
            var domain = forPlayer.Entity?.BlockSelection?.Block?.Code?.Domain;
            if (domain != null)
            {
                if (domain == "paxel")
                {
                    __result += "\n\n<font color=\"#84ef8a\"><i>Paxel</i></font>\n";
                }
            }
        }
    }
}

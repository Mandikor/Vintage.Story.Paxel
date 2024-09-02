using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Paxel.Configuration;
using Vintagestory.GameContent;

namespace Paxel;

public class ItemPaxel : ItemAxe
{

    //public override string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity forEntity)
    //{
    //    if ((api as ICoreClientAPI)?.World.Player.WorldData.CurrentGameMode == EnumGameMode.Creative) return null;

    //    return base.GetHeldTpUseAnimation(activeHotbarSlot, forEntity);
    //}

    //public override string GetHeldTpHitAnimation(ItemSlot slot, Entity byEntity)
    //{
    //    if ((api as ICoreClientAPI)?.World.Player.WorldData.CurrentGameMode == EnumGameMode.Creative) return null;

    //    return base.GetHeldTpHitAnimation(slot, byEntity);
    //}
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Game.Entity;
using Scott.GameContent;

namespace Scott.Game.Graphics
{
    public enum Layer
    {
        BottomMost = 0,
        ActorBody,
        ActorClothes,
        ActorEquipment,
        ActorWeapon,
        Default,
        Count
    }
}

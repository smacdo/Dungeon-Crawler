using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Dungeon.ComponentModel;
using Scott.Dungeon.Data;

namespace Scott.Dungeon.Graphics
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

    /// <summary>
    /// The sprite class is the base class used for rendering animated 2d images on
    /// screen. It uses a flywheel pattern to make sprites lightweight, where the sprite
    /// class itself stores current animation information, and references all the static
    /// sprite data from the SpriteData class.
    /// </summary>
    public class Sprite
    {
    }
}

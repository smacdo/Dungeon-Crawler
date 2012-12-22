using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scott.dungeon.gameobject
{
    /// <summary>
    /// Represents a physical object in the game world
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Location of the game object 
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Get the direciton this object is facing
        /// </summary>
        public Direction Direction { get; set; }

        public Actor Actor { get; private set; }
        public Movement Movement { get; private set; }
        public Sprite Sprite { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject( Sprite sprite )
        {
            Position = new Vector2( 0, 0 );
            Direction = Direction.South;

            Actor = new Actor( this );
            Movement = new Movement( this );
            Sprite = sprite;
        }
    }
}

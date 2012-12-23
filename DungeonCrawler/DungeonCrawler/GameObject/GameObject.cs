using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Scott.Dungeon.Actor;
using Scott.Dungeon.AI;
using Scott.Dungeon.Graphics;

namespace Scott.Dungeon.ComponentModel
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

        public ActorController Actor { get; set; }
        public AiController AI { get; set; }
        public Movement Movement { get; set; }
        public Sprite Sprite { get; set; }

        public int Width { get; set; }      // should we also adjust sprite width, or at least warn?
        public int Height { get; set; }     // same question

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject( Sprite sprite )
        {
            Position = new Vector2( 0, 0 );
            Direction = Direction.South;

            Actor = new ActorController( this );
            Movement = new Movement( this );
            Sprite = sprite;

            Width = sprite.Width;
            Height = sprite.Height;     // whoops, probably find a better way
        }
    }
}

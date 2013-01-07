using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Scott.Dungeon.Actor;
using Scott.Dungeon.AI;
using Scott.Dungeon.Graphics;
using Scott.Dungeon.Data;

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
        public Movement Movement { get; set; }
        public CharacterSprite CharacterSprite { get; set; }

        public BoundingRect Bounds;

        public int Width { get; set; }      // should we also adjust sprite width, or at least warn?
        public int Height { get; set; }     // same question

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject( int width, int height )
            : this( new Vector2( 0, 0 ), Direction.South, 0, 0 )
        {
        }

        /// <summary>
        /// Game object constructor
        /// </summary>
        /// <param name="position">Position of the game object</param>
        /// <param name="direction">Direction of the game object</param>
        /// <param name="width">Game object width</param>
        /// <param name="height">Game object height</param>
        public GameObject( Vector2 position, Direction direction, int width, int height )
        {
            Position = position;
            Direction = direction;

            Actor = new ActorController( this );

            Width = width;
            Height = height;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException( "Need to implement this sucker" );
            return base.GetHashCode();
        }
    }
}

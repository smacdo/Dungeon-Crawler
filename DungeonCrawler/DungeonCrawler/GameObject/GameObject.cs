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
        /// This game object's name
        /// </summary>
        public string Name { get; private set; }

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
        public GameObject( string name )
            : this( name, new Vector2( 0, 0 ), Direction.South, 0, 0 )
        {
        }

        /// <summary>
        /// Game object constructor
        /// </summary>
        /// <param name="name">This game object's name</param>
        /// <param name="position">Position of the game object</param>
        /// <param name="direction">Direction of the game object</param>
        /// <param name="width">Game object width</param>
        /// <param name="height">Game object height</param>
        public GameObject( string name, Vector2 position, Direction direction, int width, int height )
        {
            Name = name;
            Position = position;
            Direction = direction;

            Actor = new ActorController( this );

            Width = width;
            Height = height;
        }

        public string DumpDebugInfoToString()
        {
            return String.Format( "{{ name: \"{0}\", pos: {1}, dir: \"{2}\", w: {3}, h: {4} }}",
                                  Name,
                                  Position,
                                  Direction.ToString(),
                                  Width,
                                  Height );
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException( "Need to implement this sucker" );
            return base.GetHashCode();
        }
    }
}

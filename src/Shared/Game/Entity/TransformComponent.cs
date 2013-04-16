using Microsoft.Xna.Framework;
using Scott.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    ///  Contains the position, rotation and direction of an object.
    /// </summary>
    public class TransformComponent : Component
    {
        private Vector2 mPosition = Vector2.Zero;
        private Direction mDirection = Direction.East;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public TransformComponent()
        {
            // Empty
        }

        /// <summary>
        ///  Position relative to parent.
        /// </summary>
        public Vector2 Position { get { return mPosition; } set { mPosition = value; } }
        public Direction Direction { get { return mDirection; } set { mDirection = value; } }

        public override string ToString()
        {
            return "Position <{0},{1}>, Direction: {2}".With( Position.X, Position.Y, Direction );
        }

        public override void Update( GameTime time )
        {
        }
    }
}

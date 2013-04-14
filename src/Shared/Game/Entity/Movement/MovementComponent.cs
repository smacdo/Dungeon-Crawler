using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Scott.GameContent;
using Scott.Geometry;
using Scott.Game.Entity.Graphics;
using Scott.Game.Graphics;

namespace Scott.Game.Entity.Movement
{
    /// <summary>
    /// Tracks movement information
    /// </summary>
    public class MovementComponent : Component
    {
        /// <summary>
        /// The speed (in units/sec) that the movement is occurring
        /// </summary>
        public float Speed { get; set; }
        public float LastSpeed { get; set; }

        /// <summary>
        ///  Rectangle defining which part of the game object is used for movement related
        ///  collision detection.
        /// </summary>
        public RectangleF MoveBox { get; set; }

        /// <summary>
        /// The direction that the movement is occurring
        /// </summary>
        public Direction Direction { get; set; }

        public bool IsMoving
        {
            get
            {
                return ( Speed < 1 );
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MovementComponent()
        {
            Speed = 0.0f;
            LastSpeed = 0.0f;
            Direction = Direction.South;
        }

        /// <summary>
        /// Requests the actor to move in a specified direction for the duration of this update
        /// cycle
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        /// <param name="speed">Speed at which to move</param>
        public void Move( Direction direction, int speed )
        {
            Direction = direction;
            Speed = speed;
        }

        /// <summary>
        /// Stops a queued move
        /// </summary>
        public void CancelMove()
        {
            Speed = 0.0f;
        }

        /// <summary>
        /// Makes the actor face a different direction. If they are moving, then they will move in
        /// this direction
        /// </summary>
        /// <param name="direction">Direction to face</param>
        public void ChangeDirection( Direction direction )
        {
            Direction = direction;
        }

        /// <summary>
        /// Update movement game components
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
        }

    }
}

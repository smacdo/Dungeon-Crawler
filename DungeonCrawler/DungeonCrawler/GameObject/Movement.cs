using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Tracks movement information
    /// </summary>
    public class Movement : AbstractGameObjectComponent
    {
        /// <summary>
        /// The speed (in units/sec) that the movement is occurring
        /// </summary>
        public int Speed { get; set; }

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
        public Movement()
        {
            Speed = 0;
            Direction = Direction.South;
        }

        /// <summary>
        /// Update movement game components
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            // Only perform the movement math and logic if we were actually requested
            // to move.
            if ( Speed > 0 )
            {
                float timeDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;

                // Determine axis of motion
                Vector2 movementAxis = Vector2.Zero;

                switch ( Direction )
                {
                    case Direction.North:
                        movementAxis = new Vector2( 0, -1 );
                        break;

                    case Direction.South:
                        movementAxis = new Vector2( 0, 1 );
                        break;

                    case Direction.West:
                        movementAxis = new Vector2( -1, 0 );
                        break;

                    case Direction.East:
                        movementAxis = new Vector2( 1, 0 );
                        break;
                }

                // Calculate the new position
                Vector2 position = Owner.Position + ( movementAxis * Speed * timeDelta );

                // Don't let the object go out of bounds (TODO: Do this smarter when we get real levels)
                BoundingArea bounds = Owner.Bounds;
                bool isOutOfBounds  = false;

                if ( bounds != null )
                {
                    int screenWidth  = GameRoot.ViewportWidth;
                    int screenHeight = GameRoot.ViewportHeight;

                    isOutOfBounds = position.X + bounds.UpperLeft.X  < 0           ||
                                    position.X + bounds.UpperRight.X > screenWidth ||
                                    position.Y + bounds.UpperLeft.Y  < 0           ||
                                    position.Y + bounds.LowerRight.Y > screenHeight;
                }

                // Update the movement information
                if ( !isOutOfBounds )
                {
                    Owner.Direction = Direction;
                    Owner.Position = position;
                }

            }

            // Reset movement back to zero for the next update cycle.
            Speed = 0;
        }
    }
}

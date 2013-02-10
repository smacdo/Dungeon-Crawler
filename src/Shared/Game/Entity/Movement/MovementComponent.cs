using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Scott.GameContent;
using Scott.Geometry;
using Scott.Game.Entity.Graphics;

namespace Scott.Game.Entity.Movement
{
    /// <summary>
    /// Tracks movement information
    /// </summary>
    public class MovementComponent : Component
    {
        /// <summary>
        /// Direction that the player (or another controller) is requesting us to move in.
        /// </summary>
        /// <remarks>
        /// This is maintained seperately from the Movement component because we are treating
        /// it as a per frame movement action request. This is also not maintained as an
        /// action type because the player will hold the move key down, causing a ton of action
        /// spam.
        /// </remarks>
    //    private Direction mRequestedMoveDirection;
        
        /// <summary>
        /// Speed that we are moving at
        /// </summary>
        /// <remarks>
        /// This is maintained seperately from the Movement component because we are treating
        /// it as a per frame movement action request. This is also not maintained as an
        /// action type because the player will hold the move key down, causing a ton of action
        /// spam.
        /// </remarks>
   //     private int mRequestedMoveSpeed;

        /// <summary>
        /// The speed (in units/sec) that the movement is occurring
        /// </summary>
        public float Speed { get; set; }
        public float LastSpeed { get; set; }

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
            UpdateMovementAndPosition( gameTime );
            UpdateAnimation( gameTime );

            // Reset movement back to zero for the next update cycle.
            LastSpeed = Speed;
            Speed = 0;
        }

        /// <summary>
        /// Movement
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateMovementAndPosition( GameTime gameTime )
        {
            // Only perform the movement math and logic if we were actually requested
            // to move.
            if ( Speed > 0 )
            {
                float timeDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;

                // Determine axis of motion and then calculate the new position
                Vector2 movementAxis = GetMovementVector( Direction );
                Vector2 position     = Owner.Position + ( movementAxis * Speed * timeDelta );

                // Don't let the object go out of bounds (TODO: Do this smarter when we get real levels)
                BoundingArea bounds = Owner.Bounds;
                bool isOutOfBounds  = false;

                if ( bounds != null )
                {
                    int screenWidth  = 800; //  GameRoot.ViewportWidth;
                    int screenHeight = 600; //  GameRoot.ViewportHeight;

                    isOutOfBounds = position.X + bounds.UpperLeft.X < 0 ||
                                    position.X + bounds.UpperRight.X > screenWidth ||
                                    position.Y + bounds.UpperLeft.Y < 0 ||
                                    position.Y + bounds.LowerRight.Y > screenHeight;
                }

                // Update our position so long as the game object is still in bounds
                if ( !isOutOfBounds )
                {
                    Owner.Direction = Direction;
                    Owner.Position = position;
                }

            }
        }

        /// <summary>
        /// Updates the walk cycle, in lieu of another action to perform
        /// </summary>
        public void UpdateAnimation( GameTime gameTime )
        {
            SpriteComponent sprite = Owner.GetComponent<SpriteComponent>();

            if ( Speed > 0.0f )
            {
                // Animation time! Are we starting an walk animation cycle, are we switching
                // directions mid-walk, or should we just continue animating the current cycle?
                if ( !sprite.IsPlayingAnimation( "Walk", Direction ) )
                {
                    sprite.PlayAnimationLooping( "Walk", Direction );
                }
            }
            else if ( LastSpeed > 0.0f )
            {
                // Looks like we've stopped walking. Update our sprite so that we're facing the right direction
                // and being idle.
                if ( sprite.IsPlayingAnimation( "Walk", Direction ) )
                {
                    sprite.PlayAnimationLooping( "Idle", Direction );
                }
            }
        }


        /// <summary>
        /// Returns a unit vector that is oriented in the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Vector2 GetMovementVector( Direction direction )
        {
            switch ( direction )
            {
                case Direction.North:
                    return new Vector2( 0, -1 );

                case Direction.South:
                    return new Vector2( 0, 1 );

                case Direction.West:
                    return new Vector2( -1, 0 );

                case Direction.East:
                    return new Vector2( 1, 0 );

                default:
                    return Vector2.Zero;
            }
        }

    }
}

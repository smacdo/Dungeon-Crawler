using Microsoft.Xna.Framework;
using Scott.Game.Entity.Graphics;
using Scott.Game.Graphics;
using Scott.GameContent;
using Scott.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity.Movement
{
    /// <summary>
    ///  Provides movement components, and manages their high level collision logic.
    ///  
    ///  TODO: Make the collision detection algorithm not suck nearly as much. Right now I am using
    ///        an N^2 algorithm.
    /// </summary>
    public class MovementProvider : ComponentCollection<MovementComponent>
    {
        /// <summary>
        ///  Updates all movement components using collision detection and resolution.
        /// </summary>
        /// <param name="simulationTime">The current simulation time.</param>
        public override void Update( GameTime time )
        {
            // Perform movement logic for each movement component.
            foreach ( MovementComponent movement in mComponentPool )
            {
                // Only update when component is enabled.
                if ( movement.Enabled )
                {
                    // Update the component
                    IGameObject owner = movement.Owner;

                    UpdateMovement( time, owner, movement );
                    UpdateAnimation( owner, movement );

                    // Reset movement back to zero for the next update cycle.
                    movement.LastSpeed = movement.Speed;
                    movement.Speed = 0;
                }
            }
        }

        /// <summary>
        ///  Process the movement's request to move somewhere.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="movement"></param>
        private void UpdateMovement( GameTime time,
                                      IGameObject owner,
                                      MovementComponent movement )
        {
            float timeDelta = (float) time.ElapsedGameTime.TotalSeconds;
            float speed     = movement.Speed;

            if ( speed > 0.0f )
            {
                // Determine axis of motion and then calculate the new position
                Vector2 movementAxis = GameUtil.GetMovementVector( movement.Direction );
                Vector2 position     = owner.Position + ( movementAxis * speed * timeDelta );

                // Don't let the object go out of bounds
                //  (TODO: Do this smarter when we get real levels)
                RectangleF moveBox = movement.MoveBox;
                moveBox.Offset( position );

                GameRoot.Debug.DrawRect( moveBox, Color.Yellow );

                // Update our position so long as the game object is still in bounds
                if ( IsInLevelBounds( moveBox ) && !IsCollidingWithSomething( movement, moveBox ) )
                {
                    owner.Direction = movement.Direction;
                    owner.Position = position;
                }
            }
        }


        /// <summary>
        ///  Updates the movement component's animation.
        /// </summary>
        public void UpdateAnimation( IGameObject owner, MovementComponent movement )
        {
            SpriteComponent sprite = owner.GetComponent<SpriteComponent>();
            Direction direction    = movement.Direction;
            float speed            = movement.Speed;
            float lastSpeed        = movement.LastSpeed;

            if ( speed > 0.0f )
            {
                // Animation time! Are we starting an walk animation cycle, are we switching
                // directions mid-walk, or should we just continue animating the current cycle?
                if ( !sprite.IsPlayingAnimation( "Walk", direction ) )
                {
                    sprite.PlayAnimationLooping( "Walk", direction );
                }
            }
            else if ( lastSpeed > 0.0f )
            {
                // Looks like we've stopped walking. Update our sprite so that we're facing the right direction
                // and being idle.
                if ( sprite.IsPlayingAnimation( "Walk", direction ) )
                {
                    sprite.PlayAnimationLooping( "Idle", direction );
                }
            }
        }

        /// <summary>
        ///  Check if the given area is inside of the level bounds.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsInLevelBounds( RectangleF bounds )
        {
            return bounds.TopLeft.X > 0 &&
                   bounds.TopRight.X < Screen.Width &&
                   bounds.TopLeft.Y > 0 &&
                   bounds.BottomLeft.Y < Screen.Height;
        }

        /// <summary>
        ///  Check if the given area is colliding with a movement component's movebox.
        ///  TODO: This is a terrible N^2 algorithm... do something much better!
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        private bool IsCollidingWithSomething( MovementComponent self, RectangleF bounds )
        {
            foreach ( MovementComponent movement in mComponentPool )
            {
                if ( !ReferenceEquals( self, movement ) )
                {
                    IGameObject owner  = movement.Owner;
                    RectangleF moveBox = movement.MoveBox;

                    moveBox.Offset( owner.Position );

                    if ( bounds.Intersects( moveBox ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  Called after the movement component is created. We use this opportunity to start
        ///  tracking the component for later collision detection.
        /// </summary>
        /// <param name="Component">Component instance that was created.</param>
        protected override void OnComponentCreated( MovementComponent Component )
        {

            Console.Out.WriteLine( "Movement component created" );
            base.OnComponentCreated( Component );
        }

        /// <summary>
        ///  Called after the movement component is destroyed, and this provider should stop
        ///  tracking the movement component.
        /// </summary>
        /// <param name="Component">Component instance that was destroyed.</param>
        protected override void OnComponentDestroyed( MovementComponent Component )
        {
            base.OnComponentDestroyed( Component );
        }
    }
}

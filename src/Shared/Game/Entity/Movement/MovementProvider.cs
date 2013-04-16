using Microsoft.Xna.Framework;
using Scott.Common;
using Scott.Game.Entity.Graphics;
using Scott.Game.Graphics;
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
                movement.Update( time );

                // Only update when component is enabled.
                if ( movement.Enabled )
                {
                    // Update the component
                    IGameObject owner = movement.Owner;

                    UpdateAnimation( owner, movement );
                    UpdateMovement( time, owner, movement );
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
            // Is the new position OK? If not, revert back to the old position.
            Vector2 position = movement.Position;
            RectangleF moveBox = movement.MoveBox;

            moveBox.Offset( position );

            if ( IsInLevelBounds( moveBox ) && !IsCollidingWithSomething( movement, moveBox ) )
            {
                owner.Transform.Direction = movement.Direction;
                owner.Transform.Position  = position;
            }

            // Debugging aid to help visualize the movebox.
            GameRoot.Debug.DrawRect( moveBox, Color.Yellow );
        }


        /// <summary>
        ///  Updates the movement component's animation.
        /// </summary>
        public void UpdateAnimation( IGameObject owner, MovementComponent movement )
        {
            SpriteComponent sprite = owner.GetComponent<SpriteComponent>();
            Direction direction    = movement.Direction;
            float speed            = movement.Velocity;

            if ( movement.StartedMovingThisFrame )
            {
                sprite.PlayAnimationLooping( "Walk", direction );
            }
            else if ( movement.IsMoving && movement.ChangedDirectionThisFrame )
            {
                sprite.PlayAnimationLooping( "Walk", direction );
            }
            else if ( movement.StoppedMovingThisFrame )
            {
                sprite.PlayAnimationLooping( "Idle", direction );
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

                    moveBox.Offset( owner.Transform.Position );

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

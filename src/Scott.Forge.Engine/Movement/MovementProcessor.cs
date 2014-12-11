/*
 * Copyright 2012-2014 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Microsoft.Xna.Framework;
using Scott.Forge.Engine.Graphics;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Movement
{
    /// <summary>
    ///  Provides movement components, and manages their high level collision logic.
    ///  
    ///  TODO: Make the collision detection algorithm not suck nearly as much. Right now I am using
    ///        an N^2 algorithm.
    /// </summary>
    public class MovementProcessor : ComponentProcessor<MovementComponent>
    {
        /// <summary>
        ///  Updates all movement components using collision detection and resolution.
        /// </summary>
        /// <param name="simulationTime">The current simulation time.</param>
        public override void UpdateGameObject(MovementComponent movement, double currentTime, double deltaTime)
        {
            IGameObject gameObject = movement.Owner;

            // Calculate our new position.
            var position = gameObject.Transform.Position;
            var movementAxis = movement.Direction.ToVector();
            var newPosition  = position + (movementAxis * movement.Velocity * (float)deltaTime);

            movement.Update(position, movement.Velocity, movement.Direction);

            // Update game object position.
            gameObject.Transform.Position = newPosition;

            // Update animations and movement.
            UpdateAnimation(gameObject, movement );
            UpdateMovement(movement.Owner, movement, currentTime, deltaTime);
        }

        /// <summary>
        ///  Process the movement's request to move somewhere.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="movement"></param>
        private void UpdateMovement(
            IGameObject owner,
            MovementComponent movement,
            double currentTime,
            double deltaTime)
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
            var sprite = owner.GetComponent<SpriteComponent>();
            DirectionName direction = movement.Direction;
            float speed = movement.Velocity;

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
 /*           foreach ( MovementComponent movement in mComponentPool )
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
            }*/

            return false;
        }
    }
}

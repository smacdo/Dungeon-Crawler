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
            var transform = gameObject.Transform;

            if (!movement.IsMoving)
            {
                return;
            }

            // Check if we are moving before updating movement information.
            var wasMoving = movement.IsMoving;
            bool isMovingNow = false;
            var oldDirection = transform.Direction;

            // Calculate our new potential position.
            movement.Velocity += (movement.Acceleration * (float)deltaTime);
            movement.Velocity += (movement.Velocity.Normalized().Negated()*0.5f);       // bad way to do friction.
            var newPosition = transform.Position + (movement.Velocity * (float) deltaTime);

            // Check for collisions and other situations that can lead to an invalid position.
            RectangleF moveBox = movement.MoveBox;
            moveBox.Offset(newPosition);

            bool isValidPosition = (IsInLevelBounds(moveBox) && !IsCollidingWithSomething(movement, moveBox));

            if (isValidPosition)
            {
                // Movement is valid, we can update the movement component with its newest position.
                isMovingNow = movement.IsMoving;

                // Calculate movement component's new position and direction.
                transform.Position = newPosition;
                transform.Direction = DirectionNameHelper.FromVector(movement.Velocity);
            }
            else
            {
                // TODO: Find a way to put us in a valid positiohn.
            }

            // Debugging aid to help visualize movement boundary box.
            GameRoot.Debug.DrawRect(
                moveBox, 
                (isValidPosition ? Microsoft.Xna.Framework.Color.Yellow : Microsoft.Xna.Framework.Color.Red));

            // Update movement state flags.
            movement.CollisionThisFrame = !isValidPosition;
            movement.StartedMovingThisFrame = (!wasMoving && isMovingNow);
            movement.StoppedMovingThisFrame = (wasMoving && !isMovingNow);
            movement.ChangedDirectionThisFrame = (oldDirection != transform.Direction);

            // Temporary hack : stop movement
            movement.Acceleration = Vector2.Zero;

            // Update animations and movement.
            UpdateAnimation(gameObject, movement );
        }

        /// <summary>
        ///  Updates the movement component's animation.
        /// </summary>
        public void UpdateAnimation( IGameObject owner, MovementComponent movement )
        {
            var sprite = owner.GetComponent<SpriteComponent>();
            var direction = owner.Transform.Direction;

            if ( movement.StartedMovingThisFrame )
            {
                sprite.PlayAnimationLooping("Walk", direction);
            }
            else if ( movement.IsMoving && movement.ChangedDirectionThisFrame )
            {
                sprite.PlayAnimationLooping("Walk", direction);
            }
            else if ( movement.StoppedMovingThisFrame )
            {
                sprite.PlayAnimationLooping("Idle", direction);
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
            foreach (var movement in mComponents)
            {
                if ( !ReferenceEquals( self, movement ) )
                {
                    var owner  = movement.Owner;
                    var moveBox = movement.MoveBox;

                    moveBox.Offset( owner.Transform.Position );

                    if ( bounds.Intersects( moveBox ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

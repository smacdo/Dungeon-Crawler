/*
 * Copyright 2012-2017 Scott MacDonald
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
using Scott.Forge.Engine.Sprites;
using System;
using Scott.Forge.Engine.Physics;

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
        protected override void UpdateComponent(MovementComponent movement, double currentTime, double deltaTime)
        {
            IGameObject gameObject = movement.Owner;
            var transform = gameObject.Transform;

            // Skip processing if movement component is not moving.
            if (!movement.IsMoving)
            {
                return;
            }

            // TODO: Rewrite all this and use force rather than friction. Right now the actor system skips all this
            //       because it uses velocity directly.

            // Apply acceleration and friction.
            movement.Velocity += (movement.Acceleration * (float)deltaTime);
            movement.Velocity += (movement.Velocity.Normalized().Negated()*0.95f);       // bad way to do friction.

            // Limit velocity to a maximum value.
            //  TODO: Do this proper: http://answers.unity3d.com/questions/9985/limiting-rigidbody-velocity.html
            if (movement.Velocity.LengthSquared > movement.MaxSpeed * movement.MaxSpeed)
            {
                movement.Velocity = movement.Velocity.Normalized() * movement.MaxSpeed;
            }

            // Calculate new position.
            var newPosition = transform.Position + (movement.Velocity * (float) deltaTime);

            // Check for collisions and other situations that can lead to an invalid position.
            RectF moveBox = movement.MoveBox;
            moveBox.Offset(newPosition);

            if (IsInLevelBounds(moveBox))
            {
                // Calculate movement component's new position and direction.
                transform.Position = newPosition;
//                transform.Direction = DirectionNameHelper.FromVector(movement.Velocity);
            }

            DrawDebugVisualization(movement, transform);

            // Temporary hack : stop movement
            movement.Acceleration = Vector2.Zero;
        }

        private void DrawDebugVisualization(MovementComponent movement, TransformComponent transform)
        {
            /*GameRoot.Debug.DrawText(
                string.Format(
                    "x: {0:0.0}, y: {1:0.0}, vx: {2:0.0}, vy: {3:0.0} ({4})",
                    transform.Position.X,
                    transform.Position.Y,
                    movement.Velocity.X,
                    movement.Velocity.Y,
                    transform.Direction.ToString()),
                transform.Position,
                Microsoft.Xna.Framework.Color.Yellow);*/
        }

        /// <summary>
        ///  Check if the given area is inside of the level bounds.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsInLevelBounds(RectF bounds)
        {
            return bounds.TopLeft.X > 0 &&
                   bounds.TopRight.X < Screen.Width &&
                   bounds.TopLeft.Y > 0 &&
                   bounds.BottomLeft.Y < Screen.Height;
        }
    }
}

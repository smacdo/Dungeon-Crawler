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
using Forge.Physics;
using Forge.Sprites;
using Forge.GameObjects;
using System;

namespace Forge.Physics
{
    /// <summary>
    ///  Process locomotion components in the world.
    /// </summary>
    public class LocomotionComponentProcessor : ComponentProcessor<LocomotionComponent>
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public LocomotionComponentProcessor(GameScene scene)
            : base(scene)
        {
        }

        /// <summary>
        ///  Update single locomotion component.
        /// </summary>
        protected override void UpdateComponent(LocomotionComponent actor, TimeSpan currentTime, TimeSpan deltaTime)
        {
            ProcessMovementRequest(actor, currentTime, deltaTime);

            UpdateCurrentAction(actor, currentTime, deltaTime);
        }

        /// <summary>
        ///  Update the state of the actor component's current action.
        /// </summary>
        private void UpdateCurrentAction(LocomotionComponent actor, TimeSpan currentTime, TimeSpan deltaTime)
        {
            /*if (actor.CurrentAction != null)
            {
                if (actor.CurrentAction.IsFinished)
                {
                    actor.CurrentAction = null;
                }
                else
                {
                    actor.CurrentAction.Update(actor.Owner, currentTime, deltaTime);
                }
            }*/
        }

        /// <summary>
        ///  Process an actor component's movement request.
        /// </summary>
        private void ProcessMovementRequest(LocomotionComponent actor, TimeSpan currentTime, TimeSpan deltaTime)
        {
            // Skip movement if the actor is performing an action that prevents movement.
            bool movementAllowed = true;

            /*if (actor.CurrentAction != null && !actor.CurrentAction.CanMove)
            {
                movementAllowed = false;
            }*/

            // Calculate the a movement vector from the request.
            var physics = actor.Owner.Get<PhysicsComponent>();
            var transform = actor.Owner.Transform;

            var requestedMovement = actor.RequestedMovement;
            actor.RequestedMovement = Vector2.Zero;

            if (movementAllowed && requestedMovement != Vector2.Zero)
            {
                // Linearly interpolate speed from zero up to requested speed to simulate acceleration.
                var interpFactor = MathHelper.Clamp(actor.WalkAccelerationTime.TotalSeconds, 0.0f, 0.1f) * 10.0f;
                var requestedSpeed = requestedMovement.Length;
                var requestedDirection = requestedMovement.Normalized();
                var actualSpeed = Interpolation.Lerp(0.0f, requestedSpeed, interpFactor);
                
                physics.Velocity = requestedDirection * (float)actualSpeed;

                // Update actor state.
                transform.WorldRotation = requestedDirection.AngleRadians_Normalized;
                actor.WalkAccelerationTime += deltaTime;     // TODO: Move to locomotion.

                // Walk animation.
                var sprite = actor.Owner.Get<SpriteComponent>();

                if (sprite.IsAnimating)
                {
                    if (sprite.CurrentAnimation.Name != Constants.WalkAnimationName)
                    {
                        sprite.PlayAnimationLooping(Constants.WalkAnimationName);
                    }
                }
                else
                {
                    sprite.PlayAnimationLooping(Constants.WalkAnimationName);
                }
            }
            else
            {
                // The actor is not walking. Simulate deceleration (rather than hard stopping) by lerping from the
                // current speed down to zero with a quick deceleration window.s
                // TODO: Actually do deceleration.
                physics.Velocity = Vector2.Zero;
                actor.WalkAccelerationTime = TimeSpan.Zero;

                // Actor is not walking - return them to the idle animation.
                var sprite = actor.Owner.Get<SpriteComponent>();

                if (sprite.IsAnimating && sprite.CurrentAnimation.Name == "Walk")
                {
                    sprite.PlayAnimationLooping("Idle");
                }
            }
        }
    }
}

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
using Scott.Forge.Engine.Movement;
using Scott.Forge.Engine.Sprites;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Actors
{
    /// <summary>
    ///  Processes character agents in the world.
    /// </summary>
    public class ActorProcessor : ComponentProcessor<ActorComponent>
    {
        protected override void UpdateComponent(ActorComponent component, double currentTime, double deltaTime)
        {
            ProcessMovementRequest(component, currentTime, deltaTime);
            
            ProcessActionRequest(component, currentTime, deltaTime);
            UpdateCurrentAction(component, currentTime, deltaTime);
        }

        /// <summary>
        ///  Check if there was an action requested, and if so attempt to perform it.
        /// </summary>
        private void ProcessActionRequest(ActorComponent component, double currentTime, double deltaTime)
        {
            var currentAction = component.CurrentAction;
            var requestedAction = component.RequestedAction;

            // Check if the actor is idle and there's a requested action to perform. If so, schedule it for
            // execution.
            if (currentAction == null && requestedAction != null)
            {
                var movement = component.Owner.Get<MovementComponent>();
                movement.RequestStopMovement();

                component.CurrentAction = requestedAction;
                component.RequestedAction = null;

                currentAction = requestedAction;
                requestedAction = null;
            }
        }

        /// <summary>
        ///  Update the state of the actor component's current action.
        /// </summary>
        private void UpdateCurrentAction(ActorComponent component, double currentTime, double deltaTime)
        {
            var currentAction = component.CurrentAction;

            if (currentAction != null)
            {
                if (currentAction.IsFinished)
                {
                    component.CurrentAction = null;
                    currentAction = null;
                }
                else
                {
                    currentAction.Update(component.Owner, currentTime, deltaTime);
                }
            }
        }

        /// <summary>
        ///  Process an actor component's movement request.
        /// </summary>
        private void ProcessMovementRequest(ActorComponent component, double currentTime, double deltaTime)
        {
            var mover = component.Owner.Get<MovementComponent>();

            var requestedMovement = component.RequestedMovement;
            component.RequestedMovement = Vector2.Zero;

            var requestedDirection = DirectionNameHelper.FromVector(requestedMovement);

            // TODO: Don't start walking if movemnt speed is too high.
            //const float MaxSpeed = 64.0f * 64.0f;

            if (requestedMovement != Vector2.Zero)
            {
                // Linearly interpolate speed from zero up to requested speed to simulate acceleration.
                var interpFactor = MathHelper.Clamp(component.WalkAccelerationSeconds, 0.0f, 0.1f) * 10.0f;
                var requestedSpeed = requestedMovement.Length;
                var actualSpeed = Interpolation.Lerp(0.0f, requestedSpeed, interpFactor);

                mover.Velocity = DirectionNameHelper.ToVector(requestedDirection) * (float)actualSpeed;

                component.WalkAccelerationSeconds += deltaTime;

                // Walk animation.
                var sprite = component.Owner.Get<SpriteComponent>();

                if (sprite.IsAnimating)
                {
                    if (sprite.CurrentAnimation.Name == Constants.WalkAnimationName &&
                        sprite.Direction != requestedDirection)
                    {
                        sprite.PlayAnimationLooping(Constants.WalkAnimationName, requestedDirection);
                    }
                }
                else
                {
                    sprite.PlayAnimationLooping(Constants.WalkAnimationName, requestedDirection);
                }
            }
            else
            {
                // The actor is not walking. Simulate deceleration (rather than hard stopping) by lerping from the
                // current speed down to zero with a quick deceleration window.s
                // TODO: Actually do deceleration.
                mover.Velocity = Vector2.Zero;
                component.WalkAccelerationSeconds = 0.0f;

                // Actor is not walking, terminate any walking animation.
                var sprite = component.Owner.Get<SpriteComponent>();

                if (sprite.IsAnimating && sprite.CurrentAnimation.Name == "Walk")
                {
                    sprite.AbortCurrentAnimation();
                }
            }
        }
    }
}

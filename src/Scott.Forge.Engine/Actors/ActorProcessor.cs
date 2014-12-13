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
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Actors
{
    /// <summary>
    ///  Processes character agents in the world.
    /// </summary>
    public class ActorProcessor : ComponentProcessor<ActorComponent>
    {
        public override void UpdateGameObject(ActorComponent component, double currentTime, double deltaTime)
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

            // Check if the actor has a currently schedule action. Either update the action, or remove it once it
            // has finished execution.
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
    }
}

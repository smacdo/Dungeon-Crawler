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
using System;
using Scott.Forge.Engine.Actors;
using Scott.Forge.Engine.Movement;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Ai
{
    /// <summary>
    /// Does AI logic
    /// </summary>
    public class AiProcessor : ComponentProcessor<AiComponent>
    {
        private const double CHANGE_DIRECTION_CHANCE = 0.05;
        private const double START_WALKING_CHANCE = 0.15;
        private const double STOP_WALKING_CHANCE = 0.05;

        public override void UpdateGameObject(AiComponent component, double currentTime, double deltaTime)
        {
            var decisionTimeDelta = TimeSpan.FromSeconds(0.25);
            var gameTime = TimeSpan.FromSeconds(currentTime);

            var gameObject = component.Owner;

            var movement = gameObject.GetComponent<MovementComponent>();

            // Is it time for us to make a decision?
            if (component.LastDecisionTime.Add(decisionTimeDelta) <= gameTime)
            {
                PerformIdleUpdate(gameObject, currentTime, deltaTime);
                component.LastDecisionTime = gameTime;
            }
            else if (component.LastDecisionTime == TimeSpan.MinValue)
            {
                // First update call. Schedule an AI decision next update.
                component.LastDecisionTime = gameTime.Subtract(decisionTimeDelta);
            }
            else
            {
                // Keep doing whatever the heck we we are doing.
                if (movement.IsMoving)
                {
                    movement.RequestMovement(gameObject.Transform.Direction, 50);
                }
            }
        }

        /// <summary>
        /// Idle AI logic
        /// </summary>
        /// <param name="gameTime"></param>
        private void PerformIdleUpdate(IGameObject gameObject, double currentTime, double deltaTime)
        {
            var actor = gameObject.GetComponent<ActorComponent>();
            var movement = gameObject.GetComponent<MovementComponent>();

            // Are we walking around or just standing?
            if ( movement.IsMoving )
            {
                // Character is moving around... should they stop moving? Change direction mid walk?
                if ( GameRoot.Random.NextDouble() <= STOP_WALKING_CHANCE )
                {
                    // nothing to do!
                }
                else
                {
                    // Should we change direction?
                    DirectionName direction = gameObject.Transform.Direction;

                    if ( GameRoot.Random.NextDouble() <= CHANGE_DIRECTION_CHANCE )
                    {
                        direction = (DirectionName) GameRoot.Random.Next( 0, 3 );
                    }

                    movement.RequestMovement( direction, 50 );
                }
            }
            else
            {
                // Character is standing around. Should they start moving? Maybe change
                // direction
                if ( GameRoot.Random.NextDouble() <= START_WALKING_CHANCE )
                {
                    // Should we change direction when we start walking?
                    DirectionName direction = gameObject.Transform.Direction;

                    if ( GameRoot.Random.NextDouble() <= CHANGE_DIRECTION_CHANCE )
                    {
                        direction = (DirectionName) GameRoot.Random.Next( 0, 3 );
                        Console.WriteLine( "Starting to walk" );
                    }

                    // Start walking now
                    movement.RequestMovement(direction, 50);
                }
                else
                {
                    // Maybe we should look around?
                    if ( GameRoot.Random.NextDouble() <= CHANGE_DIRECTION_CHANCE )
                    {
                        // Pick a new direction
                        gameObject.Transform.Direction = (DirectionName) GameRoot.Random.Next(0, 3);
                    }
                }
            }

            // Should we change direction? (5% chance each frame)

        }
    }
}

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
using System;
using Scott.Forge.Engine.Actors;
using Scott.Forge.Engine.Movement;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Ai
{
    /// <summary>
    /// Does AI logic.
    ///  TODO: This belongs in DungeonCrawler project not in engine.
    /// </summary>
    public class AiProcessor : ComponentProcessor<AiComponent>
    {
        private const double DecisionWindowInSeconds = 1.0f;
        private const double ChangeWalkDirectionChance = 0.05;
        private const double ChangeIdleDirectionChange = 0.20;
        private const double StartWalkingChance = 0.15;
        private const double StopWalkingChance = 0.05;
        private const double MovementSpeedPerSecond = 50;

        /// <summary>
        ///  Update an AI component.
        /// </summary>
        protected override void UpdateComponent(AiComponent ai, double currentTime, double deltaTime)
        {
            var decisionTimeDelta = TimeSpan.FromSeconds(0.25);
            var gameTime = TimeSpan.FromSeconds(currentTime);

            var gameObject = ai.Owner;

            var actor = gameObject.Get<ActorComponent>();
            var movement = gameObject.Get<MovementComponent>();

            switch (ai.CurrentState)
            {
                case AiState.Idle:
                    PerformIdleUpdate(ai, gameObject, currentTime, deltaTime);
                    break;

                case AiState.Walking:
                    PerformMovingUpdate(ai, gameObject, currentTime, deltaTime);
                    break;
            }
        }

        /// <summary>
        /// Idle AI logic
        /// </summary>
        private void PerformIdleUpdate(
            AiComponent ai,
            IGameObject gameObject,
            double currentTime,
            double deltaTime)
        {
            // Consider switching to movement.
            var actor = gameObject.Get<ActorComponent>();

            if (ai.SecondsSinceLastStateChange >= DecisionWindowInSeconds)
            {
                // Shift to walking?
                if (GameRoot.Random.NextDouble() <= StartWalkingChance)
                {
                    ai.CurrentState = AiState.Walking;
                    ai.SecondsSinceLastStateChange = 0.0f;
                }
                // Change direction?
                else if (GameRoot.Random.NextDouble() <= ChangeWalkDirectionChance)
                {
                    actor.Direction = (DirectionName) GameRoot.Random.Next(0, Constants.DirectionCount);
                    ai.SecondsSinceLastStateChange = 0.0f;
                }
            }

            ai.SecondsSinceLastStateChange += deltaTime;
        }

        /// <summary>
        ///  Perform movement logic.
        /// </summary>
        private void PerformMovingUpdate(
            AiComponent ai,
            IGameObject gameObject,
            double currentTime,
            double deltaTime)
        {
            var actor = gameObject.Get<ActorComponent>();

            // Consider changing movement direction or state every second.
            if (ai.SecondsSinceLastStateChange >= DecisionWindowInSeconds)
            {
                // Start walking?
                if (GameRoot.Random.NextDouble() <= StopWalkingChance)
                {
                    ai.CurrentState = AiState.Walking;
                    ai.SecondsSinceLastStateChange = 0.0f;
                }
                // Change direction?
                else if (GameRoot.Random.NextDouble() <= ChangeWalkDirectionChance)
                {
                    actor.Direction = (DirectionName) GameRoot.Random.Next(0, Constants.DirectionCount);
                    ai.SecondsSinceLastStateChange = 0.0f;
                }
            }

            // Move!
            if (ai.CurrentState == AiState.Walking)
            {
                actor.MoveForward((float)(MovementSpeedPerSecond));
            }

            ai.SecondsSinceLastStateChange += deltaTime;
        }
    }
}

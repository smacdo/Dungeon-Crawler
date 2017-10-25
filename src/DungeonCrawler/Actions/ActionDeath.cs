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
using System.Collections.Generic;
using Forge;
using Forge.Actors;
using Forge.Sprites;
using Forge.GameObjects;

namespace DungeonCrawler.Actions
{
    /// <summary>
    ///  Death animation state.
    /// </summary>
    public enum DeathAnimationState
    {
        NotStarted,
        Performing,
        Finished
    }

    /// <summary>
    ///  Action performed when an actor dies.
    /// </summary>
    public class ActionDeath : IActorAction
    {
        private const float AnimationSeconds = 0.2f;

        private double mAnimationSecondsPlayed = 0.0f;
        private DeathAnimationState mState = DeathAnimationState.NotStarted;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionDeath()
        {
        }

        /// <summary>
        ///  If the action has completed.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return mState == DeathAnimationState.Finished;
            }
        }

        /// <summary>
        ///  If the action allows actor movement.
        /// </summary>
        public bool CanMove
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///  Update the actor with the current state of our action.
        /// </summary>
        /// <param name="gameTime">Current simulation time</param>
        public void Update(IGameObject actorGameObject, double currentTime, double deltaTime)
        {
            var actor = actorGameObject.Get<ActorComponent>();
            var direction = actorGameObject.Transform.Forward;

            var sprite = actorGameObject.Get<SpriteComponent>();
            var waitTimeSpan = TimeSpan.FromSeconds(AnimationSeconds);

            switch (mState)
            {
                case DeathAnimationState.NotStarted:
                    mAnimationSecondsPlayed = 0.0f;
                    mState = DeathAnimationState.Performing;
                    sprite.PlayAnimation("Hurt");
                    break;

                case DeathAnimationState.Performing:
                    if (mAnimationSecondsPlayed < AnimationSeconds)
                    {
                        mState = DeathAnimationState.Finished;
                    }
                    break;

                case DeathAnimationState.Finished:
                    break;
            }
        }
    }
}
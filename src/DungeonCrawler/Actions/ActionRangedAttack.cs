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
using Forge.Actors;
using Forge.Sprites;
using Forge.GameObjects;

namespace DungeonCrawler.Actions
{
    public enum RangedAttackState
    {
        NotStarted,
        Performing,
        Finished
    }

    public class ActionRangedAttack : IActorAction
    {
        private const double AnimationSeconds = 0.2;

        private double mAnimationSecondsPlayed = 0.0f;
        private RangedAttackState mState = RangedAttackState.NotStarted;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionRangedAttack()
        {
        }

        /// <summary>
        ///  If the action has completed.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return mState == RangedAttackState.Finished;
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
            var direction = actor.Direction;        // TODO: Use Transform.Direction.

            var sprite = actorGameObject.Get<SpriteComponent>();
            var waitTimeSpan = TimeSpan.FromSeconds(AnimationSeconds);

            switch (mState)
            {
                case RangedAttackState.NotStarted:
                    mAnimationSecondsPlayed = 0.0f;
                    mState = RangedAttackState.Performing;
                    sprite.PlayAnimation("Bow", direction);
                    break;

                case RangedAttackState.Performing:
                    if (mAnimationSecondsPlayed < AnimationSeconds)
                    {
                        mState = RangedAttackState.Finished;
                    }
                    break;

                case RangedAttackState.Finished:
                    break;
            }
        }
    }
}
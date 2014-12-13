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
using Scott.Forge.Engine.Actors;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Client
{
    public enum DeathAnimationState
    {
        NotStarted,
        Performing,
        Finished
    }

    public class ActionDeath : IActorAction
    {
        private const float WAIT_TIME = 0.2f;
        private const float ACTION_TIME = 0.6f;     // how long the attack lasts, sync to animation

        private TimeSpan mTimeStarted = TimeSpan.MinValue;
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
        public void Update(IGameObject actor, double currentTime, double deltaTime)
        {
            /*IGameObject owner = actor.Owner;
            Direction direction = owner.Transform.Direction;
            SpriteComponent sprite = owner.Get<SpriteComponent>();
            TimeSpan waitTimeSpan = TimeSpan.FromSeconds( WAIT_TIME );
            TimeSpan actionTimeSpan = TimeSpan.FromSeconds( ACTION_TIME );

            switch ( mState )
            {
                case DeathAnimationState.NotStarted:
                    mTimeStarted = gameTime.TotalGameTime;
                    mState = DeathAnimationState.Performing;

                    // Enable the weapon sprite, and animate the attack
                    sprite.PlayAnimation( "Hurt", direction );
                    break;

                case DeathAnimationState.Performing:
                    // Have we finished the attack?
                    if ( mTimeStarted.Add( actionTimeSpan ) <= gameTime.TotalGameTime )
                    {
                        // Disable the weapon sprite now that the attack has finished
                        mState = DeathAnimationState.Finished;
                    }
                    break;

                case DeathAnimationState.Finished:
                    break;
            }*/
        }
    }
}
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
using Forge;
using Forge.Gameplay;
using Forge.Sprites;
using Forge.GameObjects;
using Forge.Physics;

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
    ///  Action to die.
    /// </summary>
    public class ActionDeath : IGameplayAction
    {
        private static readonly TimeSpan DeathAnimationTime = TimeSpan.FromSeconds(0.2);

        private TimeSpan _timeElapsed = TimeSpan.Zero;
        private DeathAnimationState _state = DeathAnimationState.NotStarted;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionDeath()
        {
        }

        /// <inheritdoc />
        public bool IsFinished { get => _state == DeathAnimationState.Finished; }

        /// <inheritdoc />
        public bool CanMove { get => false; }

        /// <inheritdoc />
        public void Start(GameObject self)
        {
        }

        /// <inheritdoc />
        public void Update(GameObject self, TimeSpan currentTime, TimeSpan deltaTime)
        {
            var actor = self.Get<LocomotionComponent>();
            var direction = self.Transform.Forward;

            var sprite = self.Get<SpriteComponent>();

            switch (_state)
            {
                case DeathAnimationState.NotStarted:
                    _timeElapsed = TimeSpan.Zero;
                    _state = DeathAnimationState.Performing;
                    sprite.PlayAnimation("Hurt");
                    break;

                case DeathAnimationState.Performing:
                    if (_timeElapsed < DeathAnimationTime)
                    {
                        _state = DeathAnimationState.Finished;
                    }
                    break;

                case DeathAnimationState.Finished:
                    break;
            }
        }
    }
}
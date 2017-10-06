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
using Forge.Sprites;
using Forge.GameObjects;
using Forge.Physics;
using Forge.Gameplay;

namespace DungeonCrawler.Actions
{
    /// <summary>
    ///  Spell cast animation states.
    /// </summary>
    public enum CastSpellState
    {
        NotStarted,
        Performing,
        Finished
    }

    /// <summary>
    ///  Action to cast a magic spell.
    /// </summary>
    public class ActionCastSpell : IGameplayAction
    {
        private static readonly TimeSpan AnimationSpellCastTime = TimeSpan.FromSeconds(0.2);

        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private CastSpellState _state = CastSpellState.NotStarted;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionCastSpell()
        {
        }

        /// <inheritdoc />
        public bool IsFinished { get => _state == CastSpellState.Finished; }

        /// <inheritdoc />
        public bool CanMove { get => false; }

        /// <inheritdoc />
        public void Start(GameObject self)
        {
        }

        /// <inheritdoc />
        public void Update(GameObject self, TimeSpan currentTime, TimeSpan deltaTime)
        {
            var locomotion = self.Get<LocomotionComponent>();
            var direction = self.Transform.Forward;

            var sprite = self.Get<SpriteComponent>();

            switch (_state)
            {
                case CastSpellState.NotStarted:
                    _elapsedTime = TimeSpan.Zero;
                    _state = CastSpellState.Performing;
                    sprite.PlayAnimation("Spell");
                    break;

                case CastSpellState.Performing:
                    if (_elapsedTime < AnimationSpellCastTime)
                    {
                        _state = CastSpellState.Finished;
                    }
                    break;

                case CastSpellState.Finished:
                    break;
            }
        }
    }
}
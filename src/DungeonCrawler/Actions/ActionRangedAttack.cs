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
using Forge.Gameplay;
using Forge.Sprites;
using Forge.GameObjects;
using Forge.Physics;
using DungeonCrawler.Components;

namespace DungeonCrawler.Actions
{
    public enum RangedAttackState
    {
        NotStarted,
        Performing,
        Finished
    }

    /// <summary>
    ///  Action to perform a ranged attack.
    /// </summary>
    public class ActionRangedAttack : IGameplayAction
    {
        private static readonly TimeSpan AnimationRangedAttackTime = TimeSpan.FromSeconds(1.0);
        private const string AttackAnimationName = "Attack";

        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private RangedAttackState _state = RangedAttackState.NotStarted;

        private GameObject _weapon = null;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionRangedAttack()
        {
        }

        /// <inheritdoc />
        public bool IsFinished { get => _state == RangedAttackState.Finished; }

        /// <inheritdoc />
        public bool CanMove { get => false; }

        /// <inheritdoc />
        public void Start(GameObject self)
        {
            // Get the melee weapon from the game actor's primary weapon slot.
            //  TODO: Handle offhand weapons.
            //  TODO: Handle failures.
            //  TODO: Get weapon info from a weapon component.
            var equipment = self.Get<EquipmentComponent>();
            _weapon = equipment.PrimaryHand ?? throw new InvalidOperationException("Weapon not found");
        }

        /// <inheritdoc />
        public void Update(GameObject self, TimeSpan currentTime, TimeSpan deltaTime)
        {
            var actor = self.Get<LocomotionComponent>();
            var direction = self.Transform.Forward;

            var sprite = self.Get<SpriteComponent>();
            var weaponSprite = _weapon.Get<SpriteComponent>();

            switch (_state)
            {
                case RangedAttackState.NotStarted:
                    _elapsedTime = TimeSpan.Zero;
                    _state = RangedAttackState.Performing;

                    _weapon.Active = true;
                    weaponSprite.PlayAnimation(AttackAnimationName);

                    sprite.PlayAnimation("Bow");
                    break;

                case RangedAttackState.Performing:
                    if (_elapsedTime >= AnimationRangedAttackTime)
                    {
                        _weapon.Active = false;
                        _state = RangedAttackState.Finished;
                    }
                    else
                    {
                        _elapsedTime += deltaTime;
                    }
                    break;

                case RangedAttackState.Finished:
                    
                    break;
            }
        }
    }
}
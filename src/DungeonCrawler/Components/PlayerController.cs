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
using System.Collections.Generic;
using System.Text;
using DungeonCrawler.Actions;
using Forge.GameObjects;
using Forge.Gameplay;
using Forge.Input;
using Forge.Physics;

namespace DungeonCrawler.Components
{
    /// <summary>
    ///  Allows a player to control a game object.
    /// </summary>
    public class PlayerController : SelfUpdatingComponent
    {
        private InputManager<InputAction> _input;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="input">Reference to global input manager for game.</param>
        public PlayerController(InputManager<InputAction> input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }
        
        /// <inheritdoc />
        public override void OnUpdate(TimeSpan currentTime, TimeSpan deltaTime)
        {
            // Player movement.
            var locomotor = Owner.Get<LocomotionComponent>();
            var playerMovement = _input.GetAxis(InputAction.Move);

            if (playerMovement.LengthSquared > 0.01)
            {
                locomotor.Move(playerMovement, 125.0f);
            }
            
            // Player actions.
            //  TODO: Stop looking up item by hardcoded name. stop auto-equipping things.
            var actions = Owner.Get<ActionComponent>();
            var equipment = Owner.Get<EquipmentComponent>();

            if (!actions.IsBusy && _input.WasTriggered(InputAction.MeleeAttack))
            {
                var weapon = Owner.FindChildByName("Sword") ?? throw new InvalidOperationException();
                equipment.PrimaryHand = weapon;

                actions.TryPerform(new ActionSlashAttack());
            }
            else if (!actions.IsBusy && _input.WasTriggered(InputAction.RangedAttack))
            {
                var weapon = Owner.FindChildByName("Bow") ?? throw new InvalidOperationException();
                equipment.PrimaryHand = weapon;

                actions.TryPerform(new ActionRangedAttack());
            }
            else if (!actions.IsBusy && _input.WasTriggered(InputAction.CastSpell))
            {
                actions.TryPerform(new ActionCastSpell());
            }
        }
    }
}

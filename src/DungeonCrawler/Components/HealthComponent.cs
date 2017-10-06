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
using Forge.GameObjects;

namespace DungeonCrawler.Components
{
    /// <summary>
    ///  Holds health information.
    /// </summary>
    public class HealthComponent : Component
    {

        /// <summary>
        ///  Constructor.
        /// </summary>
        public HealthComponent()
            : this(100, 0, 100)
        {
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="currentHealth">Current health.</param>
        /// <param name="minHealth">Minimum health.</param>
        /// <param name="maxHealth">Maximum health.</param>
        public HealthComponent(float currentHealth, float minHealth, float maxHealth)
        {
            CurrentHealth = currentHealth;
            MinHealth = minHealth;
            MaxHealth = maxHealth;
        }

        public void ModifyHealth(float v)
        {
            var newAmount = CurrentHealth + v;

            if (newAmount <= MinHealth)
            {
                // DEATH.
                // TODO: This should be notified by event, and another component (like death) should handle
                //       the logic of animating the death, post-death logic and killing the game object. Maybe
                //       the prefab instead of a component?
                CurrentHealth = MinHealth;

                // TODO: Huge hack for giggles to simulate death until we can do it properly. Again THIS IS A HACK
                //       AND SHOULD BE CLEANED UP.
                if (Owner.Contains<Forge.Ai.AiComponent>())
                {
                    Owner.Get<Forge.Ai.AiComponent>().CurrentState = Forge.Ai.AiState.Dead;
                    // TODO: Play death.
                }

                if (Owner.Contains<Forge.Sprites.SpriteComponent>())
                {
                    Owner.Get<Forge.Sprites.SpriteComponent>().PlayAnimation("Hurt");
                }
            }
            else if (newAmount > MaxHealth)
            {
                // Just set to maximum.
                CurrentHealth = MaxHealth;
            }
            else
            {
                CurrentHealth = newAmount;
            }
        }

        /// <summary>
        ///  Get or set current health value.
        /// </summary>
        public float CurrentHealth { get; set; }

        /// <summary>
        ///  Get or set minimum health.
        /// </summary>
        public float MinHealth { get; set; }

        /// <summary>
        ///  Get or set maximum health.
        /// </summary>
        public float MaxHealth { get; set; }

        /// <summary>
        ///  Get or set if invincible.
        /// </summary>
        public float Invincible { get; set; }
    }
}

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
    ///  Allows a game object to take damage.
    /// </summary>
    public class DamageComponent : Component
    {

        /// <summary>
        ///  Constructor.
        /// </summary>
        public DamageComponent()
        {
        }
        
        public float TakeIncomingDamage(float damage)
        {
            // TODO: Handle invincibility frames.
            // TODO: Animate invincbility frames.

            // Apply damage to object.
            var health = Owner.TryGet<HealthComponent>();

            if (health != null)
            {
                health.ModifyHealth(-damage);
            }

            return damage;
        }
    }
}

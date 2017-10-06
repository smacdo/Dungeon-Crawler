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
    ///  Slots that equipment can be placed in.
    /// </summary>
    public enum EquipmentSlot
    {
        PrimaryHand,
        Count_DoNotUseAsSlot
    }

    /// <summary>
    ///  Stores equipment for a character.
    /// </summary>
    public class EquipmentComponent : Component
    {
        private GameObject[] _slots = new GameObject[(int)EquipmentSlot.Count_DoNotUseAsSlot];

        /// <summary>
        ///  Get or set the game object in the primary hand equipment slot.
        /// </summary>
        public GameObject PrimaryHand
        {
            get { return Get(EquipmentSlot.PrimaryHand); }
            set { Set(value, EquipmentSlot.PrimaryHand); }
        }
        
        /// <summary>
        ///  Try to place an object in an equipment slot.
        /// </summary>
        /// <param name="slot">Equipment slot to get object from.</param>
        /// <returns>GameObject occupying requested slot, otherwise null.</returns>
        public GameObject Get(EquipmentSlot slot)
        {
            var slotIndex = (int)slot;

            // Check that slot is valid.
            if (slotIndex < 0 || slotIndex >= (int)EquipmentSlot.Count_DoNotUseAsSlot)
            {
                throw new ArgumentException("Equipment slot index is not valid");
            }

            return _slots[slotIndex];
        }

        /// <summary>
        ///  Check if an object can be put in an equipment slot.
        /// </summary>
        /// <param name="object">Object to place.</param>
        /// <param name="slot">Equipment slot to place object into.</param>
        /// <returns>True if the placement succeeded, false otherwise.</returns>
        public bool CanSet(GameObject @object, EquipmentSlot slot)
        {
            var slotIndex = (int)slot;

            // Check that slot is valid.
            if (slotIndex < 0 || slotIndex >= (int)EquipmentSlot.Count_DoNotUseAsSlot)
            {
                return false;
            }

            // TODO: More tests.
            return true;
        }

        /// <summary>
        ///  Try to place an object in an equipment slot.
        /// </summary>
        /// <param name="object">Object to place.</param>
        /// <param name="slot">Equipment slot to place object into.</param>
        /// <returns>True if the placement succeeded, false otherwise.</returns>
        public bool Set(GameObject @object, EquipmentSlot slot)
        {
            var slotIndex = (int)slot;
            
            // Check that slot is valid.
            if (slotIndex < 0 || slotIndex >= (int)EquipmentSlot.Count_DoNotUseAsSlot)
            {
                throw new ArgumentException("Equipment slot index is not valid");
            }

            // Makes sure equipment can be placed.
            if (!CanSet(@object, slot))
            {
                return false;
            }

            // Remove previous item from the slot.
            //  TODO: Send event notifcation.
            if (_slots[slotIndex] != null)
            {
                _slots[slotIndex] = null;
            }

            // Now equip.
            //  TODO: Add events. (Call ItemComponent.Equipped)
            _slots[slotIndex] = @object;

            return true;
        }
    }
}

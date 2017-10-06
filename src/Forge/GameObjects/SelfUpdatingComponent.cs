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

namespace Forge.GameObjects
{
    /// <summary>
    ///  Interface for components that update themselves (via their game object owners).
    /// </summary>
    public interface ISelfUpdatingComponent : IComponent
    {
        /// <summary>
        ///  The next component to update.
        /// </summary>
        ISelfUpdatingComponent NextComponentToUpdate { get; set; }

        /// <summary>
        ///  Update logic for component.
        /// </summary>
        /// <param name="currentTime">Current simulation time.</param>
        /// <param name="deltaTime">Amount of time since last simulation update.</param>
        void Update(TimeSpan currentTime, TimeSpan deltaTime);
    }

    /// <summary>
    ///  Base class for components that can update themselves without an associated processor.
    /// </summary>
    public abstract class SelfUpdatingComponent : Component, ISelfUpdatingComponent
    {
        /// <inheritdoc />
        ISelfUpdatingComponent ISelfUpdatingComponent.NextComponentToUpdate { get; set; }

        /// <inheritdoc />
        void ISelfUpdatingComponent.Update(TimeSpan currentTime, TimeSpan deltaTime)
        {
            OnUpdate(currentTime, deltaTime);
        }

        /// <summary>
        ///  Update logic for component.
        /// </summary>
        /// <param name="currentTime">Current simulation time.</param>
        /// <param name="deltaTime">Amount of time since last simulation update.</param>
        public abstract void OnUpdate(TimeSpan currentTime, TimeSpan deltaTime);
    }
}

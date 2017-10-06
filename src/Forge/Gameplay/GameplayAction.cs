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
using Forge.GameObjects;

namespace Forge.Gameplay
{
    /// <summary>
    ///  Interface for an actor action.
    /// </summary>
    public interface IGameplayAction
    {
        /// <summary>
        ///  Check if the action has completed.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        ///  Check if the game object can move while the action is in progress.
        /// </summary>
        bool CanMove { get; }

        /// <summary>
        ///  Called when action is about to start execution.
        /// </summary>
        /// <param name="self">GameObject performing  this action.</param>
        void Start(GameObject self);

        /// <summary>
        ///  Updates the action towards completion.
        /// </summary>
        /// <param name="actor">GameObject performing the action.</param>
        /// <param name="currentTime">The current simulation time.</param>
        /// <param name="deltaTime">The amount of time since the last simulation update.</param>
        void Update(GameObject self, TimeSpan currentTime, TimeSpan deltaTime);
    }
}

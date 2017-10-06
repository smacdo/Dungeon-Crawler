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

namespace Forge.Gameplay
{
    /// <summary>
    ///  The action component is responsible for holding an action that a game object charter would like to
    ///  perform, or is performing. These actions form the fundamental RPC nature of character driven interaction
    ///  in Forge's networked game system.
    /// </summary>
    public class ActionComponent : Component
    {
        /// <summary>
        ///  Get or set the current action being performed by the game object.
        /// </summary>
        public IGameplayAction CurrentAction { get; internal set; }

        /// <summary>
        ///  Get if action is being performed.
        /// </summary>
        public bool IsBusy
        {
            get { return CurrentAction != null || RequesetedAction != null; }
        }

        /// <summary>
        ///  Get or set the action that should be performed.
        /// </summary>
        public IGameplayAction RequesetedAction { get; internal set; }

        /// <summary>
        ///  Request an action to be performed by the gameplay object. This will return true only if the action can be
        ///  performed.
        /// </summary>
        /// <param name="action">Action to be performed.</param>
        /// <returns>True if the action is scheduled and can be performed, false otherwise.</returns>
        public bool TryPerform(IGameplayAction action)
        {
            // TODO: Add cancellation logic.
            if (CurrentAction == null)
            {
                RequesetedAction = action;
                return true;
            }

            return false;
        }
    }
}

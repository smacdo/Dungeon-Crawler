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
using Forge.GameObjects;

namespace Forge.Gameplay
{
    /// <summary>
    ///  The action component is responsible for coordinating and executing actions that game object actors can
    ///  take in the game world. These actions are thing like "attack", "talk to", "move to" or any other action
    ///  that can be performed by a charcter in a game world.
    /// </summary>
    public class ActionComponentProcessor : ComponentProcessor<ActionComponent>
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public ActionComponentProcessor(GameScene scene)
            : base(scene)
        {
        }

        /// <inheritdoc />
        protected override void UpdateComponent(ActionComponent action, TimeSpan currentTime, TimeSpan deltaTime)
        {
            // Check if there is an action requested to go and if so start it.
            if (action.RequesetedAction != null)
            {
                action.CurrentAction = action.RequesetedAction;
                action.RequesetedAction = null;

                // TODO: Remove cast when IGameObject is removed from project.
                action.CurrentAction.Start((GameObject)action.Owner);
            }

            // Is there an action that should be updated? If so update the action until completion at which point the
            // action should be removed.
            if (action.CurrentAction != null)
            {
                if (action.CurrentAction.IsFinished)
                {
                    action.CurrentAction = null;
                }
                else
                {
                    // TODO: Remove cast when IGameObject is removed from project.
                    action.CurrentAction.Update((GameObject)action.Owner, currentTime, deltaTime);
                }
            }
        }
    }
}

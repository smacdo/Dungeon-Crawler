/*
 * Copyright 2012-2014 Scott MacDonald
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
using Scott.Forge.GameObjects;

namespace Scott.Forge.Actors
{
    /// <summary>
    ///  Interface for an actor action.
    /// </summary>
    public interface IActorAction
    {
        /// <summary>
        ///  Check if the action is completed.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        ///  Check if the player can move while the action is in progress.
        /// </summary>
        bool CanMove { get; }

        /// <summary>
        ///  Updates the action towards completion.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="gameTime"></param>
        void Update(IGameObject actor, double currentTime, double deltaTime);
    }

    /// <summary>
    ///  An action that an actor can perform.
    /// </summary>
    public abstract class ActorAction : IActorAction
    {
        /// <summary>
        ///  Check if the action is completed.
        /// </summary>
        public abstract bool IsFinished { get; }

        public abstract bool CanMove { get; }

        /// <summary>
        ///  Updates the action towards completion.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="gameTime"></param>
        public abstract void Update(IGameObject actor, double currentTime, double deltaTime);
    }
}

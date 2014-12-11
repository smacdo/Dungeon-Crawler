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
using System;

namespace Scott.Forge.GameObjects
{
    /// <summary>
    /// Base class for all game object components
    /// 
    /// NOTE: DO NOT STORE REFERENCES TO GAME COMPONENTS FOR MORE THAN ONE FRAME!
    ///       Instead, re-request the reference from either the GameObjectCollection
    ///       or the specific game object. This is to ensure we don't go crazy trying
    ///       to figure out who is still holding references to deleted instances.
    /// 
    /// TODO: Explain this much better
    /// </summary>
    public abstract class Component : IComponent
    {
        private IGameObject mOwner;

        /// <summary>
        /// Component constructor
        /// </summary>
        protected Component()
        {
            Enabled = true;
        }

        /// <summary>
        /// The game object that owns this components
        /// </summary>
        public IGameObject Owner
        {
            get
            {
                return mOwner;
            }
            set
            {
                if ( mOwner != null )
                {
                    throw new NotSupportedException( "Reparenting game objects is not supported" );
                }

                mOwner = value;
            }
        }

        /// <summary>
        /// Enables or disable the game component instance. A disabld component will not
        /// be updated or displayed.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///  Writes the game component out to a stirng
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var ownerId = "null";

            if (Owner != null)
            {
                ownerId = Owner.Id.ToString();
            }

            return String.Format("Component {0}, owner: {1}", GetType().Name, ownerId);
        }
    }
}

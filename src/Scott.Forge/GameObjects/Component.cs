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

namespace Scott.Forge.GameObjects
{
    /// <summary>
    ///  Interface for components that can be attached to game objects.
    /// </summary>
    public interface IComponent : IDisposable
    {
        /// <summary>
        ///  Get the active status of this component.
        /// </summary>
        /// <remarks>
        ///  Inactive components are not updated by their respective component processor.
        /// </remarks>
        bool Active { get; set; }

        /// <summary>
        ///  Get or set the game object that owns this component.
        /// </summary>
        IGameObject Owner { get; set; }
    }

    /// <summary>
    /// Base class for all game object components
    /// </summary>
    /// <remarks>
    /// NOTE: DO NOT STORE REFERENCES TO GAME COMPONENTS FOR MORE THAN ONE FRAME!
    /// TODO: Explain this much better
    /// </remarks>
    public abstract class Component : IComponent
    {
        private bool mDisposed = false;
        private bool mActive = true;
        private IGameObject mOwner;
        private IComponentDestroyedCallback mDestroyCallback;

        /// <summary>
        ///  Component constructor
        /// </summary>
        protected Component()
            : this(null)
        {
        }

        /// <summary>
        ///  Component constructor
        /// </summary>
        protected Component(IComponentDestroyedCallback callback)
        {
            mDestroyCallback = callback;
        }

        /// <summary>
        ///  Destructor that will invoke Dispose upon destruction.
        /// </summary>
        ~Component()
        {
            Dispose(false);
        }

        /// <summary>
        ///  Get the active status of this component.
        /// </summary>
        /// <remarks>
        ///  Inactive components are not updated by their respective component processor.
        /// </remarks>
        public bool Active
        {
            get { return mActive && (Owner != null ? Owner.Active : true); }
            set { mActive = value; }
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
                if (mOwner != null)
                {
                    throw new CannotChangeComponentOwnerException(this, mOwner, value);
                }

                mOwner = value;
            }
        }

        /// <summary>
        ///  Dispose the component. This informs the optional creator factory that this component is about to be
        ///  removed.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        /// <summary>
        ///  Clean up the component before disposal. Inform the creator factory that the component is being destroyed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (mDestroyCallback != null)
                {
                    mDestroyCallback.Destroy(this);
                }

                mDestroyCallback = null;
                mDisposed = true;
            }
        }

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

            return String.Format("<Component name: {0}, owner: {1}>", GetType().Name, ownerId);
        }
    }
}

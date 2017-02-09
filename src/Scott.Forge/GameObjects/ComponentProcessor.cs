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

namespace Scott.Forge.GameObjects
{
    /// <summary>
    ///  Interface for systems that process groups of components attached to game objects.
    /// </summary>
    public interface IComponentProcessor<TComponent>
        where TComponent : class, IComponent, new()
    {
        void Add(TComponent component);
        TComponent Add(IGameObject gameObject);

        bool Remove(TComponent component);

        int ComponetnCount { get; }

        void Update(double currentTime, double deltaTime);
    }

    /// <summary>
    ///  Simple component processor that processes a batch of components. This component processor injects a typed
    ///  component into each game object and assumes that it is the exclusive owner of said component. Each time the
    ///  component processor gets an update call it will loop through all the components that it added and process them
    ///  in a batch.
    /// </summary>
    /// <remarks>
    ///  TODO: Switch mComponents to a fixed size array set at processor creation time, and use a list/linked list to
    ///  manage overflow components.
    ///  TODO: 
    /// </remarks>
    public abstract class ComponentProcessor<TComponent> : IComponentProcessor<TComponent>
        where TComponent : class, IComponent, new()
    {
        /// <summary>
        ///  The number of components that should be created initially for improved performance.
        /// </summary>
        public const int DefaultCapacity = 100;

        /// <summary>
        ///  List of components for processing.
        /// </summary>
        protected readonly List<TComponent> mComponents; 

        /// <summary>
        /// Constructor
        /// </summary>
        protected ComponentProcessor()
        {
            mComponents = new List<TComponent>(DefaultCapacity);
        }

        /// <summary>
        ///  Get the number of game objects registered in this component processor.
        /// </summary>
        public int ComponetnCount
        {
            get { return mComponents.Count; }
        }

        /// <summary>
        ///  Adds the game object to this object processor for future updates.
        /// </summary>
        /// <param name="gameObject">The game object to track.</param>
        public TComponent Add(IGameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }

            // Instantiate the component, and add it to the game object.
            var component = CreateComponent(gameObject);
            Add(component);

            gameObject.Add(component);
            return component;
        }

        /// <summary>
        ///  Add a component instance to the processor.
        /// </summary>
        /// <param name="component">Instance to add.</param>
        public void Add(TComponent component)
        {
            // Cannot add null.
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            // Add.
            mComponents.Add(component);
        }

        /// <summary>
        ///  Creates a new instance instance of the component for attaching to the game object.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        protected virtual TComponent CreateComponent(IGameObject gameObject)
        {
            return new TComponent {Owner = gameObject};
        }

        /// <summary>
        ///  Remove component instance from processor.
        /// </summary>
        /// <param name="component">Component to remove.</param>
        public bool Remove(TComponent component)
        {
            if (component != null)
            {
                // Remove from game object (if attached).
                if (component.Owner != null)
                {
                    component.Owner.Remove<TComponent>();
                }

                // TODO: Terrible for performance.
                return mComponents.Remove(component);
            }
            
            return false;
        }

        /// <summary>
        /// Updates all active game components managed by this component manager
        /// </summary>
        /// <remarks>
        ///  The update method uses a traditional for looop to reduce garbage generation.
        /// </remarks>
        /// <param name="gameTime">The current game time</param>
        public virtual void Update(double currentTime, double deltaTime)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < mComponents.Count; ++index)
            {
                if (mComponents[index].Active)
                {
                    UpdateComponent(mComponents[index], currentTime, deltaTime);
                }
            }
        }

        protected abstract void UpdateComponent(TComponent component, double currentTime, double deltaTime);

    }
}

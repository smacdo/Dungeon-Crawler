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
        TComponent Add(IGameObject gameObject);

        void Remove(IGameObject gameObject);

        int GameObjectCount { get; }

        void Update(double currentTime, double deltaTime);
    }

    /// <summary>
    ///  Simple component processor that processes a batch of components. This component processor injects a typed
    ///  component into each game object and assumes that it is the exclusive owner of said component. Each time the
    ///  component processor gets an update call it will loop through all the components that it added and process them
    ///  in a batch.
    /// </summary>
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
        public int GameObjectCount
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
            gameObject.Add(component);

            // Add the component to the processor's list of components for updating.
            mComponents.Add(component);

            return component;
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
        ///  Adds the game object to this object processor for future updates.
        /// </summary>
        /// <param name="gameObject">The game object to track.</param>
        public void Remove(IGameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }

            // Get the component from the game object, remove it from the processor and then delete the component from
            // the game object itself.
            var component = gameObject.Find<TComponent>();

            if (component == null)
            {
                throw new ComponentDoesNotExistException(gameObject, typeof(TComponent));
            }
            else
            {
                gameObject.Remove<TComponent>();
                mComponents.Remove(component);
            }
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
                if (mComponents[index].Enabled)
                {
                    UpdateComponent(mComponents[index], currentTime, deltaTime);
                }
            }
        }

        protected abstract void UpdateComponent(TComponent component, double currentTime, double deltaTime);

    }
}

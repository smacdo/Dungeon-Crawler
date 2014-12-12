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
using System.Collections.Generic;
using System.Text;

namespace Scott.Forge.GameObjects
{
    /// <summary>
    ///  Simple component processor that processes a batch of components. This component processor injects a typed
    ///  component into each game object and assumes that it is the exclusive owner of said component. Each time the
    ///  component processor gets an update call it will loop through all the components that it added and process them
    ///  in a batch.
    /// </summary>
    public abstract class ComponentProcessor<TComponent> : IComponentProcessor where TComponent : IComponent, new()
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
        ///  Adds the game object to this object processor for future updates.
        /// </summary>
        /// <param name="gameObject">The game object to track.</param>
        public TComponent Add(IGameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }

            var component = new TComponent {Owner = gameObject};
            gameObject.AddComponent(component);

            mComponents.Add(component);

            return component;
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
            var component = gameObject.GetComponent<TComponent>();
            gameObject.DeleteComponent<TComponent>();

            if (!mComponents.Remove(component))
            {
                // TODO: Replace with specific exception.
                throw new InvalidOperationException("Game object missing component required by this component processor");
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
                UpdateGameObject(mComponents[index], currentTime, deltaTime);
            }
        }

        public abstract void UpdateGameObject(TComponent component, double currentTime, double deltaTime);
    }
}

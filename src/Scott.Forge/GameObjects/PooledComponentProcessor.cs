/*
 * Copyright 2012-2015 Scott MacDonald
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
    /// Takes care of component life cycle management in addition to processing update
    /// cycles.
    /// TODO: Split up into ComponentProcessor and PooledComponentProcessor<T>. The pooled processor should contain
    ///       the instance pooling.
    /// </summary>
    public abstract class PooledComponentProcessor<TComponent> : IComponentProcessor<TComponent>
        where TComponent : class, IComponent, IRecyclable, new()
    {
        /// <summary>
        ///  The number of components that should be created initially for improved performance.
        /// </summary>
        public const int DefaultCapacity = 100;

        /// <summary>
        /// A pre-allocated pool of instances to speed up object creation and destruction
        /// </summary>
        private readonly InstancePool<TComponent> mComponentPool;

        /// <summary>
        /// Constructor
        /// </summary>
        public PooledComponentProcessor()
        {
            mComponentPool = new InstancePool<TComponent>(DefaultCapacity);
        }

        /// <summary>
        ///  Get the number of game objects registered in this component processor.
        /// </summary>
        public int GameObjectCount
        {
            get { return mComponentPool.ActiveCount; }
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

            // Take an unused component instance and add it to the game object.
            var component = mComponentPool.Take();

            gameObject.Add(component);
            component.Owner = gameObject;

            // Let derived classes have a chance to run code after the component is created and added.
            OnComponentCreated(component);
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
            var component = gameObject.Find<TComponent>();

            if (component == null)
            {
                throw new ComponentDoesNotExistException(gameObject, typeof(TComponent));
            }
            else
            {
                // Reset to component's default state, and then return it to the component pool.
                OnComponentDestroyed(component);
                mComponentPool.Return(component);

                // Remove the component from the game object.
                gameObject.Remove<TComponent>();
            }
        }

        /// <summary>
        /// Updates all active game components managed by this component manager
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public virtual void Update(double currentTime, double deltaTime)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            foreach (var instance in mComponentPool)
            {
                UpdateComponent(instance, currentTime, deltaTime);
            }
        }

        protected abstract void UpdateComponent(TComponent component, double currentTime, double deltaTime);
        
        /// <summary>
        ///  Called after a component is created.
        /// </summary>
        /// <param name="Component"></param>
        protected virtual void OnComponentCreated(TComponent component)
        {
            // Empty
        }

        /// <summary>
        ///  Called immediately prior to a c component being recycled.
        /// </summary>
        /// <param name="Component"></param>
        protected virtual void OnComponentDestroyed(TComponent component)
        {
            // empty
        }
    }
}

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
    /// Takes care of component life cycle management in addition to processing update
    /// cycles.
    /// TODO: Split up into ComponentProcessor and PooledComponentProcessor<T>. The pooled processor should contain
    ///       the instance pooling.
    /// </summary>
    public abstract class PooledComponentProcessor<T> : IComponentProcessor, IEnumerable<IGameObject>
        where T : IComponent, IRecyclable, new()
    {
        /// <summary>
        ///  The number of components that should be created initially for improved performance.
        /// </summary>
        public const int DefaultCapacity = 100;

        /// <summary>
        ///  List of game objects that are tracked for processing.
        /// </summary>
        private readonly List<IGameObject> mTrackedObjects;

        /// <summary>
        /// A pre-allocated pool of instances to speed up object creation and destruction
        /// </summary>
        private readonly InstancePool<T> mComponentPool;

        /// <summary>
        /// Constructor
        /// </summary>
        public PooledComponentProcessor()
        {
            mTrackedObjects = new List<IGameObject>(DefaultCapacity);
            mComponentPool = new InstancePool<T>(DefaultCapacity);
        }

        /// <summary>
        ///  Adds the game object to this object processor for future updates.
        /// </summary>
        /// <param name="gameObject">The game object to track.</param>
        void Add(IGameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }

            mTrackedObjects.Add(gameObject);
            gameObject.AddComponent(CreateNewComponent(gameObject));
        }

        /// <summary>
        ///  Adds the game object to this object processor for future updates.
        /// </summary>
        /// <param name="gameObject">The game object to track.</param>
        void Remove(IGameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }

            if (mTrackedObjects.Remove(gameObject))
            {
                var component = gameObject.GetComponent<T>(); // TODO: Verify must succeed.    
                DestroyComponent(component);
                gameObject.DeleteComponent<T>();
            }
            else
            {
                throw new ArgumentException("Game object is in procesor's list of game objects");
            }
        }

        /// <summary>
        /// Creates a new component for the requested game object instance
        /// </summary>
        /// <param name="owner">The game object that will own this instance</param>
        /// <returns>A newly create game component instance</returns>
        private T CreateNewComponent(IGameObject owner)
        {
            var instance = mComponentPool.Take();
            instance.Owner = owner;

            owner.AddComponent<T>(instance);
            OnComponentCreated(instance);

            return instance;
        }

        /// <summary>
        /// Destroys a component
        /// </summary>
        /// <param name="component"></param>
        public virtual void DestroyComponent(T instance)
        {
            // Reset to component's default state, and then return it to the component pool.
            OnComponentDestroyed(instance);
            mComponentPool.Return(instance);
        }

        /// <summary>
        /// Updates all active game components managed by this component manager
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public virtual void Update(double currentTime, double deltaTime)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < mTrackedObjects.Count; ++index)
            {
                UpdateGameObject(mTrackedObjects[index]);
            }
        }

        public abstract void UpdateGameObject(IGameObject gameObject);

        /// <summary>
        ///  Generate formatted text output detailing the state of this component collection.
        /// </summary>
        /// <returns>Debugging information.</returns>
        public string DumpDebugInfoToString()
        {
            StringBuilder debugText = new StringBuilder();

            debugText.Append("{\n");
            debugText.Append(String.Format("\tcomponent_manager: \"{0}\",\n", typeof(T).Name));
            debugText.Append(String.Format("\tactive_instances: {0},\n", mComponentPool.ActiveCount));
            debugText.Append(String.Format("\tfree_instances: {0},\n", mComponentPool.FreeCount));
            debugText.Append(String.Format("\ttotal_instances: {0},\n", mComponentPool.TotalCount));
            debugText.Append("\tinstances: [\n");

            // Dump diagnostic information on active components only
            foreach (T instance in mComponentPool)
            {
                debugText.Append("\t\t");
                debugText.Append(instance.ToString());
                debugText.Append(",\n");
            }

            debugText.Append("\t]\n}\n");

            return debugText.ToString();
        }

        /// <summary>
        ///  Returns an enumerator that enumerates over the components in this collection.
        /// </summary>
        /// <returns>Component enumerator</returns>
        public List<IGameObject>.Enumerator GetEnumerator()
        {
            return mTrackedObjects.GetEnumerator();
        }

        /// <summary>
        ///  Returns an enumerator that enumerates over the components in this collection.
        /// </summary>
        /// <returns>Component enumerator</returns>
        IEnumerator<IGameObject> IEnumerable<IGameObject>.GetEnumerator()
        {
            return mTrackedObjects.GetEnumerator();
        }

        /// <summary>
        ///  Returns an enumerator that enumerates over the components in this collection.
        /// </summary>
        /// <returns>Component enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mComponentPool.GetEnumerator();
        }

        /// <summary>
        ///  Called after a component is created.
        /// </summary>
        /// <param name="Component"></param>
        protected virtual void OnComponentCreated(T component)
        {
            // Empty
        }

        /// <summary>
        ///  Called immediately prior to a c component being recycled.
        /// </summary>
        /// <param name="Component"></param>
        protected virtual void OnComponentDestroyed(T component)
        {
            // empty
        }
    }
}

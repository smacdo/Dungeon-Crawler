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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.GameObjects
{
    /// <summary>
    /// Takes care of component life cycle management in addition to processing update
    /// cycles.
    /// </summary>
    public class ComponentCollection<T> : IEnumerable<T>
        where T : IComponent, IRecyclable, new()
    {
        /// <summary>
        ///  The number of components that should be created initially for improved performance.
        /// </summary>
        public const int DEFAULT_CAPACITY = 100;

        /// <summary>
        /// A pre-allocated pool of instances to speed up object creation and destruction
        /// </summary>
        protected InstancePool<T> mComponentPool;

        /// <summary>
        /// Constructor
        /// </summary>
        public ComponentCollection()
        {
            mComponentPool = new InstancePool<T>( DEFAULT_CAPACITY );
        }

        /// <summary>
        /// Creates a new component for the requested game object instance
        /// </summary>
        /// <param name="owner">The game object that will own this instance</param>
        /// <returns>A newly create game component instance</returns>
        public T Create( IGameObject owner )
        {
            T instance = mComponentPool.Take();

            instance.Owner = owner;

            owner.Add<T>( instance );
            OnComponentCreated( instance );

            return instance;
        }

        /// <summary>
        /// Destroys a component
        /// </summary>
        /// <param name="component"></param>
        public virtual void Destroy( T instance )
        {
            // Reset to component's default state, and then return it to the component pool.
            OnComponentDestroyed( instance );
            mComponentPool.Return( instance );       
        }

        /// <summary>
        /// Updates all active game components managed by this component manager
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public virtual void Update( GameTime gameTime )
        {
            foreach ( T instance in mComponentPool )
            {
                //instance.Update( gameTime );
            }
        }

        /// <summary>
        ///  Generate formatted text output detailing the state of this component collection.
        /// </summary>
        /// <returns>Debugging information.</returns>
        public string DumpDebugInfoToString()
        {
            StringBuilder debugText = new StringBuilder();

            debugText.Append( "{\n" );
            debugText.Append( String.Format( "\tcomponent_manager: \"{0}\",\n", typeof( T ).Name ) );
            debugText.Append( String.Format( "\tactive_instances: {0},\n", mComponentPool.ActiveCount ) );
            debugText.Append( String.Format( "\tfree_instances: {0},\n", mComponentPool.FreeCount ) );
            debugText.Append( String.Format( "\ttotal_instances: {0},\n", mComponentPool.TotalCount ) );
            debugText.Append( "\tinstances: [\n" );

            // Dump diagnostic information on active components only
            foreach ( T instance in mComponentPool )
            {
                debugText.Append( "\t\t" );
                debugText.Append( instance.ToString() );
                debugText.Append( ",\n" );
            }

            debugText.Append( "\t]\n}\n" );
            
            return debugText.ToString();
        }

        /// <summary>
        ///  Returns an enumerator that enumerates over the components in this collection.
        /// </summary>
        /// <returns>Component enumerator</returns>
        public LinkedList<T>.Enumerator GetEnumerator()
        {
            return mComponentPool.GetEnumerator();
        }

        /// <summary>
        ///  Returns an enumerator that enumerates over the components in this collection.
        /// </summary>
        /// <returns>Component enumerator</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return mComponentPool.GetEnumerator();
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
        protected virtual void OnComponentCreated( T Component )
        {
            // Empty
        }

        /// <summary>
        ///  Called immediately prior to a c component being recycled.
        /// </summary>
        /// <param name="Component"></param>
        protected virtual void OnComponentDestroyed( T Component )
        {
            // empty
        }
    }
}

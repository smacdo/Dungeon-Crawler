using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Scott.Common;

namespace Scott.Game.Entity
{
    /// <summary>
    /// Takes care of component life cycle management in addition to processing update
    /// cycles.
    /// </summary>
    public class ComponentCollection<T> : IComponentCollection, IEnumerable<T>
        where T : IComponent, IRecyclable, new()
    {
        public const int DEFAULT_CAPACITY = 100;

        /// <summary>
        /// A pre-allocated pool of instances to speed up object creation and destruction
        /// </summary>
        private InstancePool<T> mComponentPool;

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
            instance.Enabled = true;
            instance.Id = Guid.NewGuid();

            owner.AddComponent<T>( instance );

            return instance;
        }

        /// <summary>
        /// Creates a new component for the requested game object instance
        /// </summary>
        /// <param name="owner">The game object that will own this instance</param>
        /// <returns>A newly create game component instance</returns>
        IComponent IComponentCollection.Create( IGameObject owner )
        {
            return (IComponent) Create( owner );
        }

        /// <summary>
        /// Destroys a component
        /// </summary>
        /// <param name="component"></param>
        public virtual void Destroy( T instance )
        {
            // Reset the component to it's default state, and then return it to the
            // component pool
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
                if ( instance.Enabled )
                {
                    instance.Update( gameTime );
                }
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
    }
}

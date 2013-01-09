using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Takes care of component life cycle management in addition to processing update
    /// cycles.
    /// </summary>
    public class ComponentManager<T> where T : IGameObjectComponent, new()
    {
        /// <summary>
        /// A pre-allocated pool of instances to speed up object creation and destruction
        /// </summary>
        private InstancePool<T> mComponentPool;
        private UniqueIdManager mIdManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public ComponentManager( int capacity )
        {
            mComponentPool = new InstancePool<T>( capacity );
            mIdManager = new UniqueIdManager();
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
        /// Creates a new component for the requested game object instance
        /// </summary>
        /// <param name="owner">The game object that will own this instance</param>
        /// <returns>A newly create game component instance</returns>
        public virtual T Create( GameObject owner )
        {
            // Instantiate the game component and set it up
            T instance = mComponentPool.Take();

            instance.ResetComponent( owner, true, mIdManager.AllocateId() );
            owner.AddComponent<T>( instance );

            return instance;
        }

        /// <summary>
        /// Destroys a component
        /// </summary>
        /// <param name="component"></param>
        public virtual void Destroy( T instance )
        {
            // Reset the component to it's default state, and then return it to the
            // component pool
            instance.ResetComponent( null, false, 0 );
            mComponentPool.Return( instance );
        }

        public string DumpDebugDumpDebugInfoToString()
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
    }
}

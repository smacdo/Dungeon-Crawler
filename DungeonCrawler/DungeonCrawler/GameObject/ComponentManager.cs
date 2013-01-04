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
        private const int DEFAULT_CAPACITY = 4096;

        /// <summary>
        /// A pre-allocated pool of instances to speed up object creation and destruction
        /// </summary>
        private InstancePool<T> mComponentPool;

        /// <summary>
        /// Constructor
        /// </summary>
        public ComponentManager()
        {
            mComponentPool = new InstancePool<T>( DEFAULT_CAPACITY );
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
        public T Create( GameObject owner )
        {
            // [TODO: Should we force each game object to only have one component of each
            //        type?]

            // Instantiate the game component and set it up
            T instance = mComponentPool.Take();

            instance.ResetComponent( owner, true );
            return instance;
        }

        /// <summary>
        /// Destroys a component
        /// </summary>
        /// <param name="component"></param>
        public void Destroy( T instance )
        {
            // Reset the component to it's default state, and then return it to the
            // component pool
            instance.ResetComponent();
            mComponentPool.Return( instance );
        }
    }
}

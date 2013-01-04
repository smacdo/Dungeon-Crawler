using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Base class for all game object components
    /// </summary>
    public class IGameObjectComponent
    {
        private GameObject mGameObject;

        /// <summary>
        /// The game object that owns this components
        /// </summary>
        public GameObject GameObject
        {
            get
            {
                return mGameObject;
            }
            set
            {
                if ( mGameObject != null )
                {
                    throw new NotSupportedException( "Reparenting game objects is not supported" );
                }
                else
                {
                    mGameObject = value;
                }
            }
        }

        /// <summary>
        /// Enables or disable the game component instance. A disabld component will not
        /// be updated or displayed.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Component constructor
        /// </summary>
        public IGameObjectComponent()
        {
            GameObject = null;
            Enabled = false;
        }

        public virtual void Update( GameTime time )
        {
            return;
        }

        public void ResetComponent()
        {
            ResetComponent( null, false );
        }

        public void ResetComponent( GameObject gameObject, bool enabled )
        {
            GameObject = gameObject;
            Enabled = enabled;
        }
    }
}

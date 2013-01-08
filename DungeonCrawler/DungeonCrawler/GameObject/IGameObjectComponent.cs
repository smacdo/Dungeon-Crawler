using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Base class for all game object components
    /// 
    /// NOTE: DO NOT STORE REFERENCES TO GAME COMPONENTS FOR MORE THAN ONE FRAME!
    ///       Instead, re-request the reference from either the GameObjectCollection
    ///       or the specific game object. This is to ensure we don't go crazy trying
    ///       to figure out who is still holding references to deleted instances.
    /// 
    /// TODO: Explain this much better
    /// </summary>
    public abstract class IGameObjectComponent
    {
        private GameObject mOwner;
        public ulong Id { get; private set; }

        /// <summary>
        /// The game object that owns this components
        /// </summary>
        public GameObject Owner
        {
            get
            {
                return mOwner;
            }
            set
            {
                if ( mOwner != null )
                {
                    throw new NotSupportedException( "Reparenting game objects is not supported" );
                }
                else
                {
                    mOwner = value;
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
            Owner = null;
            Enabled = false;
            Id = 0;     // Zero is always an illegal uid
        }

        /// <summary>
        /// Updates the game component
        /// </summary>
        /// <param name="time">The current simulation time</param>
        public abstract void Update( GameTime time );

        /// <summary>
        /// Resets the game component to a default state. The component is disabled and
        /// will have no game object owner until one is assigned.
        /// </summary>
        public void ResetComponent()
        {
            ResetComponent( null, false, 0 );   // Zero is always an invalid UID
        }

        /// <summary>
        /// Resets the game component to a default state
        /// </summary>
        /// <param name="gameObject">The game object that owns this component</param>
        /// <param name="enabled">Whetehr</param>
        public void ResetComponent( GameObject gameObject, bool enabled, ulong id )
        {
            Owner = gameObject;
            Enabled = enabled;
            Id = id;
        }

        /// <summary>
        /// Writes the game component out to a stirng
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if ( Owner != null )
            {
                return String.Format( "{{ id: {0}, owner: \"{1}\", enabled: {2}, {3} }}",
                                      Id,
                                      Owner.Name,
                                      Enabled,
                                      DumpDebugInfo() );
            }
            else
            {
                return String.Format( "{{ id {0}, owner: 0, enabled: {1}, {2} }}",
                                      Id,
                                      Enabled,
                                      DumpDebugInfo() );
            }
        }

        public virtual string DumpDebugInfo()
        {
            return string.Empty;
        }
    }
}

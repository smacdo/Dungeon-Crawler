using Microsoft.Xna.Framework;
using Scott.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
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
    public abstract class Component : IComponent, IRecyclable
    {
        private IGameObject mOwner;
        private Guid mId;

        /// <summary>
        ///  Gets or sets the component's unique identifier.
        /// </summary>
        public Guid Id
        {
            get
            {
                return mId;
            }
            set
            {
                // only set the id if it is empty
                if ( mId == Guid.Empty )
                {
                    mId = value;
                }
                else
                {
                    throw new InvalidOperationException( "Cannot reassign component id once oen has been assigned" );
                }
            }
        }

        /// <summary>
        /// The game object that owns this components
        /// </summary>
        public IGameObject Owner
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
        public Component()
        {
            Recycle();
        }

        /// <summary>
        /// Updates the game component
        /// </summary>
        /// <param name="time">The current simulation time</param>
        public abstract void Update( GameTime time );

        /// <summary>
        ///  Resets the component's state back to it's default state.
        /// </summary>
        public void Recycle()
        {
            mOwner = null;
            mId = Guid.Empty;
            Enabled = false;
        }

        /// <summary>
        ///  Writes the game component out to a stirng
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format( "Component type: {1}, id: {1}", GetType().Name, mId ); 
        }

        /// <summary>
        ///  Returns a string containing information on this component instance.
        /// </summary>
        /// <returns></returns>
        public string DumpDebugInfo()
        {
            if ( Owner != null )
            {
                return String.Format( "{{ id: \"{0}\", owner: \"{1}\", enabled: {2} }}",
                                      Id,
                                      Owner.Name,
                                      Enabled,
                                      DumpDebugInfo() );
            }
            else
            {
                return String.Format( "{{ id \"{0}\", owner: 0, enabled: {1} }}",
                                      Id,
                                      Enabled,
                                      DumpDebugInfo() );
            }           
        }
    }
}

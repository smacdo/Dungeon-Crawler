using Microsoft.Xna.Framework;
using Scott.Dungeon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Game component that detects, resolves and handles collisions between game objects
    /// </summary>
    public class Collider : BoundingRect, IGameObjectComponent
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Collider()
            : this( new Rectangle( 0, 0, 0, 0 ), 0 )
        {

        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Bounding box dimensions</param>
        public Collider( Rectangle boundingBox )
            : this( boundingBox, 0, Vector2.Zero )
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation (in radians)</param>
        public Collider( Rectangle boundingBox, float rotation )
            : this( boundingBox, rotation, Vector2.Zero )
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation</param>
        /// <param name="origin">Rotational pivot position. Top left is (0,0).</param>
        public Collider( Rectangle boundingBox, float rotation, Vector2 origin )
            : base( boundingBox, rotation, origin )
        {
            Owner = null;
            Enabled = false;
            Id = 0;     // Zero is always an illegal uid
        }

        /// <summary>
        /// Perform update computations
        /// </summary>
        /// <param name="time"></param>
        public void Update( GameTime time )
        {
            Rectangle worldCollisionRect = new Rectangle( (int) Owner.Position.X + UnrotatedBoundingRect.X,
                                                          (int) Owner.Position.Y + UnrotatedBoundingRect.Y,
                                                          (int) this.Width,
                                                          (int) this.Height );

            GameRoot.Debug.DrawRect( worldCollisionRect, Color.Green );
        }

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

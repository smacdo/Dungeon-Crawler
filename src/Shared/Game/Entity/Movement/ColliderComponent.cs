using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity.Movement
{
    /// <summary>
    /// An animation component manages a sprite's animation
    /// </summary>
    public class ColliderComponent : Component
    {
        /// <summary>
        /// Checks if this collider component had a collision on the current update cycle
        /// </summary>
        public bool HadCollision { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ColliderComponent()
        {
            HadCollision = false;
        }

        /// <summary>
        /// Processes any pending collisions from this update cycle
        /// </summary>
        /// <param name="time"></param>
        public override void Update( GameTime time )
        {
            if ( HadCollision )
            {
                GameRoot.Debug.DrawFilledRect( Owner.Bounds.BroadPhaseRectangle, Color.Red );
            }
        }
    }
}

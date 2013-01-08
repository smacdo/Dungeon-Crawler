using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    public class MovementManager : ComponentManager<Movement>
    {
        public MovementManager( int capacity )
            : base( capacity )
        {
        }

        /// <summary>
        /// Create a new movement game component
        /// </summary>
        /// <param name="owner">Game object owner</param>
        /// <returns>Newly created instance</returns>
        public override Movement Create( GameObject owner )
        {
            Movement movement = base.Create( owner );
            owner.Movement    = movement;

            return movement;
        }

        /// <summary>
        /// Destroy a movement game component
        /// </summary>
        /// <param name="instance"></param>
        public override void Destroy( Movement instance )
        {
            instance.GameObject.Movement = null;
            base.Destroy( instance );
        }
    }
}

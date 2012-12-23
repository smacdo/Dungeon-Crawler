using System;
using System.Collections.Generic;
using Scott.Dungeon.ComponentModel;
using Microsoft.Xna.Framework;

namespace Scott.Dungeon.Actor
{
    /// <summary>
    /// Performs a slashing attack
    /// </summary>
    public class ActionSlashAttack
    {
        /// <summary>
        /// The actor that is performing the slash attack
        /// </summary>
        public ActorController mActor { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actor"></param>
        public ActionSlashAttack( ActorController actor )
        {
            mActor = actor;
        }

        /// <summary>
        /// Update simulation with the state of slashing attack
        /// </summary>
        /// <param name="gameTime">Current simulation time</param>
        public void Update( GameTime gameTime )
        {

        }
    }
}

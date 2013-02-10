using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Scott.Game.Entity;
using Scott.Game.Entity.Movement;

namespace Scott.Game.Entity.Actor
{
    /// <summary>
    /// Represents a interactive character
    /// </summary>
    public class ActorController : Component
    {
        /// <summary>
        /// The current action that this actor is performing
        /// </summary>
        private ActionSlashAttack mCurrentAction;

        /// <summary>
        /// Constructor
        /// </summary>
        public ActorController()
        {
        }

        /// <summary>
        /// Perform a slashing attack in the direction this actor is facing
        /// </summary>
        public void SlashAttack()
        {
            if ( mCurrentAction == null )
            {
                MovementComponent movement = Owner.GetComponent<MovementComponent>();
                movement.CancelMove();
               
 //               mCurrentAction = new ActionSlashAttack( Owner, Owner.Direction );
            }
        }

        /// <summary>
        /// Updates the state of the game actor
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            if ( mCurrentAction != null )
            {
                mCurrentAction.Update( gameTime );

                // Cancel out the active action if it has completed
                if ( mCurrentAction.IsFinished )
                {
                    mCurrentAction = null;
                }
            }

        }
    }
}

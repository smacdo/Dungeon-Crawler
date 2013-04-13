using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Scott.Game.Entity;
using Scott.Game.Entity.Movement;
using Scott.GameContent;

namespace Scott.Game.Entity.Actor
{
    /// <summary>
    /// Represents a interactive character
    /// </summary>
    public class ActorController : Component
    {
        private IActorAction mCurrentAction;
        private Direction mDirection;

        public Direction Direction { get { return mDirection; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public ActorController()
        {
            mDirection = Direction.East;
        }

        /// <summary>
        ///  Instructs the player to move.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        public void Move( Direction direction, int distance )
        {
            if ( mCurrentAction == null || mCurrentAction.CanMove )
            {
                MovementComponent movement = Owner.GetComponent<MovementComponent>();
                movement.Move( direction, distance );

                mDirection = direction;
            }
        }

        /// <summary>
        /// Perform a slashing attack in the direction this actor is facing
        /// </summary>
        public void Perform( IActorAction action )
        {
            if ( mCurrentAction == null )
            {
                // Cancel any player movement.
                MovementComponent movement = Owner.GetComponent<MovementComponent>();
                movement.CancelMove();

                // Install the current action.
                mCurrentAction = action;
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
                mCurrentAction.Update( this, gameTime );

                // Cancel out the active action if it has completed
                if ( mCurrentAction.IsFinished )
                {
                    mCurrentAction = null;
                }
            }

        }
    }
}

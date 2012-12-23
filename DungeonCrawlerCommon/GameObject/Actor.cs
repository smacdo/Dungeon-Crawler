using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace scott.dungeon.gameobject
{
    /// <summary>
    /// Represents a interactive character
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// The game object that this component belongs to
        /// </summary>
        private GameObject mGameObject;

        /// <summary>
        /// Flag if this actor was requested to move in the upcoming update call.
        /// </summary>
        private bool mMoveRequested;

        /// <summary>
        /// Flag if this actor was moving on the last update call.
        /// </summary>
        private bool mWasMovingLastUpdateCall;

        /// <summary>
        /// The direction the player was facing on the last call
        /// </summary>
        private Direction mDirectionDuringLastCall;

        /// <summary>
        /// Direction that the player (or another controller) is requesting us to move in.
        /// </summary>
        /// <remarks>
        /// This is maintained seperately from the Movement component because we are treating
        /// it as a per frame movement action request. This is also not maintained as an
        /// action type because the player will hold the move key down, causing a ton of action
        /// spam.
        /// </remarks>
        private Direction mRequestedMoveDirection;

        /// <summary>
        /// Speed that we are moving at
        /// </summary>
        /// <remarks>
        /// This is maintained seperately from the Movement component because we are treating
        /// it as a per frame movement action request. This is also not maintained as an
        /// action type because the player will hold the move key down, causing a ton of action
        /// spam.
        /// </remarks>
        private int mRequestedMoveSpeed;

        /// <summary>
        /// Checks if the actor is moving
        /// </summary>
        public bool IsMoving
        {
            get
            {
                return mWasMovingLastUpdateCall;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Actor( GameObject gameObject )
        {
            mGameObject = gameObject;
            mMoveRequested = false;
            mWasMovingLastUpdateCall = false;
            mDirectionDuringLastCall = Direction.South;
            mRequestedMoveDirection = Direction.South;
            mRequestedMoveSpeed = 0;
        }

        /// <summary>
        /// Requests the actor to move in a specified direction for the duration of this update
        /// cycle
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        /// <param name="speed">Speed at which to move</param>
        public void Move( Direction direction, int speed )
        {
            // Request a move
            mMoveRequested = true;
            mRequestedMoveDirection = direction;
            mRequestedMoveSpeed = speed;
        }

        /// <summary>
        /// Makes the actor face a different direction. If they are moving, then they will move in
        /// this direction
        /// </summary>
        /// <param name="direction">Direction to face</param>
        public void ChangeDirection( Direction direction )
        {
            mRequestedMoveDirection = direction;
        }

        /// <summary>
        /// Updates the state of the game actor
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime )
        {
            // Do we need to perform movement logic this frame?
            if ( mMoveRequested )
            {
                // Animation time! Are we starting an walk animation cycle, are we switching
                // directions mid-walk, or should we just continue animating the current cycle?
                if ( !mWasMovingLastUpdateCall || mRequestedMoveDirection != mDirectionDuringLastCall )
                {
                    mGameObject.Sprite.PlayAnimation( "Walk" + Enum.GetName( typeof( Direction ), mRequestedMoveDirection ) );
                }

                // Set up movement information so our actor actually does
                Movement movement = mGameObject.Movement;
                Debug.Assert( movement != null );

                movement.Direction = mRequestedMoveDirection;
                movement.Speed = mRequestedMoveSpeed;

                // Keep track of the fact that this actor is now moving
                mWasMovingLastUpdateCall = true;
                mDirectionDuringLastCall = mRequestedMoveDirection;

                // Reset movement information so we don't repeat this move on the next frame
                mMoveRequested = false;
            }
            else if ( mWasMovingLastUpdateCall )
            {
                // Looks like we've stopped walking. Update our sprite so that we're facing the right direction
                // and being idle.
                mGameObject.Sprite.PlayAnimation( "Stand" + Enum.GetName( typeof( Direction ), mRequestedMoveDirection ) );

                mWasMovingLastUpdateCall = false;
                mDirectionDuringLastCall = mRequestedMoveDirection;
            }
            else if ( mRequestedMoveDirection != mDirectionDuringLastCall )
            {
                // We've changed direction without actually moving
                mGameObject.Sprite.PlayAnimation( "Stand" + Enum.GetName( typeof( Direction ), mRequestedMoveDirection ) );
                mDirectionDuringLastCall = mRequestedMoveDirection;
            }
        }
    }
}

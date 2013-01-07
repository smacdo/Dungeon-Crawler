using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Scott.Dungeon.ComponentModel;
using Scott.Dungeon.Actor;

namespace Scott.Dungeon.AI
{
    /// <summary>
    /// Does AI logic
    /// </summary>
    public class AiController : IGameObjectComponent
    {
        private const double CHANGE_DIRECTION_CHANCE = 0.05;
        private const double START_WALKING_CHANCE = 0.15;
        private const double STOP_WALKING_CHANCE = 0.05;

        /// <summary>
        /// The last time this AI made a decision
        /// </summary>
        private TimeSpan mLastDecisionTime;

        /// <summary>
        /// Random generator for the AI controller
        /// </summary>
        private Random mRandom;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">Game object who owns this AI controller</param>
        public AiController()
        {
            mLastDecisionTime = TimeSpan.MinValue;
            mRandom = new Random();
        }

        /// <summary>
        /// Updates the state of the game actor
        /// </summary>
        /// <param name="gameTime">Current simulation time</param>
        public override void Update( GameTime gameTime )
        {
            TimeSpan decisionTimeDelta = TimeSpan.FromSeconds( 0.25 );

            // Is it time for us to make a decision?
            if ( mLastDecisionTime.Add( decisionTimeDelta ) <= gameTime.TotalGameTime )
            {
                PerformIdleUpdate( gameTime );
                mLastDecisionTime = gameTime.TotalGameTime;
            }
            else if ( mLastDecisionTime == TimeSpan.MinValue )
            {
                // First update call. Schedule an AI decision tick next frame
                mLastDecisionTime = gameTime.TotalGameTime.Subtract( decisionTimeDelta );
            }
            else
            {
                // Keep doing whatever the heck we were doing
                if ( GameObject.Actor.IsMoving )
                {
                    GameObject.Actor.Move( GameObject.Direction, 50 );
                }
            }
        }

        /// <summary>
        /// Idle AI logic
        /// </summary>
        /// <param name="gameTime"></param>
        private void PerformIdleUpdate( GameTime gameTime )
        {
            ActorController actor = GameObject.Actor;

            // Are we walking around or just standing?
            if ( actor.IsMoving )
            {
                // Character is moving around... should they stop moving? Change direction mid walk?
                if ( mRandom.NextDouble() <= STOP_WALKING_CHANCE )
                {
                    // nothing to do!
                }
                else
                {
                    // Should we change direction?
                    Direction direction = GameObject.Direction;

                    if ( mRandom.NextDouble() <= CHANGE_DIRECTION_CHANCE )
                    {
                        direction = (Direction) mRandom.Next( 0, 3 );
                    }

                    actor.Move( direction, 50 );
                }
            }
            else
            {
                // Character is standing around. Should they start moving? Maybe change
                // direction
                if ( mRandom.NextDouble() <= START_WALKING_CHANCE )
                {
                    // Should we change direction when we start walking?
                    Direction direction = GameObject.Direction;

                    if ( mRandom.NextDouble() <= CHANGE_DIRECTION_CHANCE )
                    {
                        direction = (Direction) mRandom.Next( 0, 3 );
                        Console.WriteLine( "Starting to walk" );
                    }

                    // Start walking now
                    actor.Move( direction, 50 );
                }
                else
                {
                    // Maybe we should look around?
                    if ( mRandom.NextDouble() <= CHANGE_DIRECTION_CHANCE )
                    {
                        // Pick a new direction
                        actor.ChangeDirection( (Direction) mRandom.Next( 0, 3 ) );
                    }
                }
            }

            // Should we change direction? (5% chance each frame)

        }
    }
}

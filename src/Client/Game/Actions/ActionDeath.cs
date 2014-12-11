using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Scott.Common;
using Scott.Forge.GameObjects.Graphics;
using Scott.Geometry;
using Scott.Forge.GameObjects;
using Scott.Forge.GameObjects.Actor;
using Scott.Game;

namespace Scott.Dungeon.Actions
{
    public enum DeathAnimationState
    {
        NotStarted,
        Performing,
        Finished
    }

    public class ActionDeath : IActorAction
    {
        private const float WAIT_TIME = 0.2f;
        private const float ACTION_TIME = 0.6f;     // how long the attack lasts, sync to animation

        private TimeSpan mTimeStarted = TimeSpan.MinValue;
        private DeathAnimationState mState = DeathAnimationState.NotStarted;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionDeath()
        {
        }

        /// <summary>
        ///  If the action has completed.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return mState == DeathAnimationState.Finished;
            }
        }

        /// <summary>
        ///  If the action allows actor movement.
        /// </summary>
        public bool CanMove
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///  Update the actor with the current state of our action.
        /// </summary>
        /// <param name="gameTime">Current simulation time</param>
        public void Update( ActorController actor, GameTime gameTime )
        {
            IGameObject owner = actor.Owner;
            Direction direction = owner.Transform.Direction;
            SpriteComponent sprite = owner.GetComponent<SpriteComponent>();
            TimeSpan waitTimeSpan = TimeSpan.FromSeconds( WAIT_TIME );
            TimeSpan actionTimeSpan = TimeSpan.FromSeconds( ACTION_TIME );

            switch ( mState )
            {
                case DeathAnimationState.NotStarted:
                    mTimeStarted = gameTime.TotalGameTime;
                    mState = DeathAnimationState.Performing;

                    // Enable the weapon sprite, and animate the attack
                    sprite.PlayAnimation( "Hurt", direction );
                    break;

                case DeathAnimationState.Performing:
                    // Have we finished the attack?
                    if ( mTimeStarted.Add( actionTimeSpan ) <= gameTime.TotalGameTime )
                    {
                        // Disable the weapon sprite now that the attack has finished
                        mState = DeathAnimationState.Finished;
                    }
                    break;

                case DeathAnimationState.Finished:
                    break;
            }
        }
    }
}
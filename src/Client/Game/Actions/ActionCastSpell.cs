using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Scott.Common;
using Scott.Game.Entity.Graphics;
using Scott.Geometry;
using Scott.Game.Entity;
using Scott.Game.Entity.Actor;
using Scott.Game;

namespace Scott.Dungeon.Actions
{
    public enum CastSpellState
    {
        NotStarted,
        Performing,
        Finished
    }

    public class ActionCastSpell : IActorAction
    {
        private const float WAIT_TIME = 0.2f;
        private const float ACTION_TIME = 0.7f;     // how long the attack lasts, sync to animation

        private TimeSpan mTimeStarted = TimeSpan.MinValue;
        private CastSpellState mState = CastSpellState.NotStarted;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionCastSpell()
        {
        }

        /// <summary>
        ///  If the action has completed.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return mState == CastSpellState.Finished;
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
                case CastSpellState.NotStarted:
                    mTimeStarted = gameTime.TotalGameTime;
                    mState = CastSpellState.Performing;

                    // Enable the weapon sprite, and animate the attack
                    sprite.PlayAnimation( "Spell", direction );
                    break;

                case CastSpellState.Performing:
                    // Have we finished the attack?
                    if ( mTimeStarted.Add( actionTimeSpan ) <= gameTime.TotalGameTime )
                    {
                        // Disable the weapon sprite now that the attack has finished
                        mState = CastSpellState.Finished;
                    }
                    break;

                case CastSpellState.Finished:
                    break;
            }
        }
    }
}
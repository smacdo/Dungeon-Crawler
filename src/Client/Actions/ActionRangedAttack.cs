﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Scott.GameContent;
using Scott.Common;
using Scott.Game.Entity.Graphics;
using Scott.Geometry;
using Scott.Game.Entity;
using Scott.Game.Entity.Actor;

namespace Scott.Dungeon.Actions
{
    public enum RangedAttackState
    {
        NotStarted,
        Performing,
        Finished
    }

    public class ActionRangedAttack : IActorAction
    {
        private const float WAIT_TIME = 0.2f;
        private const float ACTION_TIME = 1.3f;     // how long the attack lasts, sync to animation

        private TimeSpan mTimeStarted = TimeSpan.MinValue;
        private RangedAttackState mState = RangedAttackState.NotStarted;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionRangedAttack()
        {
        }

        /// <summary>
        ///  If the action has completed.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return mState == RangedAttackState.Finished;
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
            Direction direction = owner.Direction;
            SpriteComponent sprite = owner.GetComponent<SpriteComponent>();
            TimeSpan waitTimeSpan = TimeSpan.FromSeconds( WAIT_TIME );
            TimeSpan actionTimeSpan = TimeSpan.FromSeconds( ACTION_TIME );

            switch ( mState )
            {
                case RangedAttackState.NotStarted:
                    mTimeStarted = gameTime.TotalGameTime;
                    mState = RangedAttackState.Performing;

                    // Enable the weapon sprite, and animate the attack
                    sprite.PlayAnimation( "Bow", direction );
                    break;

                case RangedAttackState.Performing:
                    // Have we finished the attack?
                    if ( mTimeStarted.Add( actionTimeSpan ) <= gameTime.TotalGameTime )
                    {
                        // Disable the weapon sprite now that the attack has finished
                        mState = RangedAttackState.Finished;
                    }
                    break;

                case RangedAttackState.Finished:
                    break;
            }
        }
    }
}
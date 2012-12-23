using System;
using System.Collections.Generic;
using Scott.Dungeon.ComponentModel;
using Microsoft.Xna.Framework;
using Scott.Dungeon.Graphics;

namespace Scott.Dungeon.Actor
{
    public enum ActionAttackStatus
    {
        NotStarted,
        InProgress,
        Finished
    }

    /// <summary>
    /// Performs a slashing attack
    /// </summary>
    public class ActionSlashAttack
    {
        private const float ACTION_TIME = 0.6f;     // how long the attack lasts, sync to animation

        /// <summary>
        /// The actor that is performing the slash attack
        /// </summary>
        private ActorController mActor;
        private CharacterSprite mSprite;

        private Direction mDirection;

        private TimeSpan mTimeStarted;
        private ActionAttackStatus mAttackStatus;

        /// <summary>
        /// Check if the action has finished
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return mAttackStatus == ActionAttackStatus.Finished;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actor"></param>
        public ActionSlashAttack( GameObject owner, Direction direction )
        {
            mActor = owner.Actor;
            mSprite = owner.CharacterSprite;
            mDirection = direction;
            mTimeStarted = TimeSpan.MinValue;
            mAttackStatus = ActionAttackStatus.NotStarted;
        }

        /// <summary>
        /// Update simulation with the state of slashing attack
        /// </summary>
        /// <param name="gameTime">Current simulation time</param>
        public void Update( GameTime gameTime )
        {
            TimeSpan actionTimeSpan = TimeSpan.FromSeconds( ACTION_TIME );

            switch ( mAttackStatus )
            {
                case ActionAttackStatus.NotStarted:
                    mTimeStarted = gameTime.TotalGameTime;
                    mAttackStatus = ActionAttackStatus.InProgress;

                    // Enable the weapon sprite, and animate the attack
                    mSprite.WeaponSprite.Visible = true;
                    mSprite.PlayAnimation( "Slash" + Enum.GetName( typeof( Direction ), mDirection ) );

                    break;

                case ActionAttackStatus.InProgress:
                    // Perform attack hit detection

                    // Have we finished the attack?
                    if ( mTimeStarted.Add( actionTimeSpan ) <= gameTime.TotalGameTime )
                    {
                        // Disable the weapon sprite now that the attack has finished
                        mSprite.WeaponSprite.Visible = false;
                        mAttackStatus = ActionAttackStatus.Finished;
                    }
                    break;

                case ActionAttackStatus.Finished:
                    break;
            }
        }
    }
}

﻿using System;
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
    public enum ActionAttackStatus
    {
        NotStarted,
        StartingUp,
        Performing,
        Finished
    }

    public class ActionSlashAttack : IActorAction
    {
        private const float WAIT_TIME = 0.2f;
        private const float ACTION_TIME = 0.6f;     // how long the attack lasts, sync to animation

        private TimeSpan mTimeStarted = TimeSpan.MinValue;
        private ActionAttackStatus mAttackStatus = ActionAttackStatus.NotStarted;

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

        public bool CanMove
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionSlashAttack()
        {
        }

        /// <summary>
        /// Update simulation with the state of slashing attack
        /// </summary>
        /// <param name="gameTime">Current simulation time</param>
        public void Update( ActorController actor, GameTime gameTime )
        {
            IGameObject owner = actor.Owner;
            Direction direction = owner.Transform.Direction;
            SpriteComponent sprite = owner.GetComponent<SpriteComponent>();
            TimeSpan waitTimeSpan = TimeSpan.FromSeconds( WAIT_TIME );
            TimeSpan actionTimeSpan = TimeSpan.FromSeconds( ACTION_TIME );
            
            switch ( mAttackStatus )
            {
                case ActionAttackStatus.NotStarted:
                    mTimeStarted = gameTime.TotalGameTime;
                    mAttackStatus = ActionAttackStatus.StartingUp;

                    // Enable the weapon sprite, and animate the attack
                    sprite.PlayAnimation( "Slash", direction );
                    sprite.EnableLayer( "Weapon", true );
                    break;

                case ActionAttackStatus.StartingUp:
                    // Are we still waiting for the attack to begin?
                    if ( mTimeStarted.Add( waitTimeSpan ) <= gameTime.TotalGameTime )
                    {
                        mAttackStatus = ActionAttackStatus.Performing;
                    }
                    break;

                case ActionAttackStatus.Performing:
                    // Have we finished the attack?
                    if ( mTimeStarted.Add( actionTimeSpan ) <= gameTime.TotalGameTime )
                    {
                        // Disable the weapon sprite now that the attack has finished
                        sprite.EnableLayer( "Weapon", false );
                        mAttackStatus = ActionAttackStatus.Finished;
                    }
                    else
                    {
                        // Perform attack hit detection
                        DrawHitBox( gameTime );
                    }
                    break;

                case ActionAttackStatus.Finished:
                    break;
            }
        }

        /// <summary>
        /// Draws a hit box for the game
        /// </summary>
        private void DrawHitBox( GameTime gameTime )
        {
            /*            double currentTime = gameTime.TotalGameTime.TotalSeconds;
                        double startedAt   = mTimeStarted.TotalSeconds + WAIT_TIME;
                        double finishedAt  = startedAt + ACTION_TIME - WAIT_TIME;
                        double weightedAmount = MathUtil.NormalizeToZeroOneRange( currentTime, startedAt, finishedAt );
            
                        float angleDeg = MathHelper.Lerp( 170.0f, 10.0f, (float) weightedAmount );
                        float radians = MathHelper.ToRadians( angleDeg );

                        Vector2 actorPosition = mGameObject.Position;
                        Vector2 weaponOffset = new Vector2( 32, 32 );
                        Vector2 weaponSize = new Vector2( 55, 10 );
                        Vector2 weaponPivot = new Vector2( 0, 5 );

                        Vector2 weaponWorldPos = actorPosition + weaponOffset;

                        Rectangle weaponRect = new Rectangle( (int) weaponWorldPos.X, 
                                                              (int) weaponWorldPos.Y,
                                                              (int) weaponSize.X,
                                                              (int) weaponSize.Y );

                        BoundingRect bounds = new BoundingRect( weaponRect, radians, weaponPivot + weaponWorldPos );
                                                            //    new Vector2( 0.0f, 10.0f / 2.0f ) );

                        GameRoot.Debug.DrawBoundingBox( bounds, Color.HotPink );


                        // Test all the other skeletons out there
                        List<GameObject> collisions = new List<GameObject>();

                        for ( int i = 0; i < GameRoot.Enemies.Count; ++i )
                        {
                            GameObject obj = GameRoot.Enemies[i];
                            Vector2 pos = obj.Position;

                            Rectangle staticInnerRect = new Rectangle( (int) pos.X, (int) pos.Y, 64, 64 );
                            BoundingRect staticRect = new BoundingRect( staticInnerRect );
                            GameRoot.Debug.DrawBoundingBox( staticRect, Color.PowderBlue );

                            // lets see what happens
                            if ( bounds.Intersects( staticRect ) )
                            {
                                collisions.Add( obj );
                                GameRoot.Debug.DrawFilledRect( staticInnerRect, Color.White );
                            }
                        }

                        // Delete the other colliding game objects
                        for ( int i = 0; i < collisions.Count; ++i )
                        {
                            GameRoot.Enemies.Remove( collisions[i] );
                        }*/
        }
    }
}
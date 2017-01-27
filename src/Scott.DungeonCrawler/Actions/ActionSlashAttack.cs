/*
 * Copyright 2012-2014 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using Scott.Forge;
using Scott.Forge.Engine;
using Scott.Forge.Engine.Actors;
using Scott.Forge.Engine.Sprites;
using Scott.Forge.GameObjects;

namespace Scott.DungeonCrawler.Actions
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
        public void Update(IGameObject actorGameObject, double currentTimeSeconds, double deltaTime)
        {
            var actor = actorGameObject.Get<ActorComponent>();
            var direction = actor.Direction;

            var sprite = actorGameObject.Get<SpriteComponent>();

            var currentTime = TimeSpan.FromSeconds(currentTimeSeconds);;
            var waitTimeSpan = TimeSpan.FromSeconds( WAIT_TIME );
            var actionTimeSpan = TimeSpan.FromSeconds( ACTION_TIME );
            
            switch ( mAttackStatus )
            {
                case ActionAttackStatus.NotStarted:
                    mTimeStarted = currentTime;
                    mAttackStatus = ActionAttackStatus.StartingUp;

                    // Enable the weapon sprite, and animate the attack
                    sprite.PlayAnimation( "Slash", direction );
                    //sprite.EnableLayer( "Weapon", true );
                    break;

                case ActionAttackStatus.StartingUp:
                    // Are we still waiting for the attack to begin?
                    if (mTimeStarted.Add(waitTimeSpan) <= currentTime)
                    {
                        mAttackStatus = ActionAttackStatus.Performing;
                    }
                    break;

                case ActionAttackStatus.Performing:
                    // Have we finished the attack?
                    if (mTimeStarted.Add(actionTimeSpan) <= currentTime)
                    {
                        // Disable the weapon sprite now that the attack has finished
                        //sprite.EnableLayer( "Weapon", false );
                        mAttackStatus = ActionAttackStatus.Finished;
                    }
                    else
                    {
                        // Perform attack hit detection
                        DrawHitBox(actorGameObject, currentTimeSeconds);
                    }
                    break;

                case ActionAttackStatus.Finished:
                    break;
            }
        }

        /// <summary>
        /// Draws a hit box for the game
        /// </summary>
        private void DrawHitBox(IGameObject actor, double currentTimeSeconds)
        {
            /*double startedAt   = mTimeStarted.TotalSeconds + WAIT_TIME;
            double finishedAt  = startedAt + ACTION_TIME - WAIT_TIME;
            double weightedAmount = MathHelper.NormalizeToZeroOneRange(currentTimeSeconds, startedAt, finishedAt );
            
            float angleDeg = Interpolation.Lerp( 170.0f, 10.0f, (float) weightedAmount );
            float radians = MathHelper.DegreeToRadian( angleDeg );

            var actorPosition = actor.Transform.Position;
            var weaponOffset = new Vector2( 32, 32 );
            var weaponSize = new Vector2( 55, 10 );
            var weaponPivot = new Vector2( 0, 5 );

            var weaponWorldPos = actorPosition + weaponOffset;

            var weaponRect = new RectF(weaponWorldPos.X,  weaponWorldPos.Y, weaponSize.X, weaponSize.Y);
            var bounds = new BoundingArea( weaponRect, radians, weaponPivot + weaponWorldPos );
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
/*
 * Copyright 2012-2017 Scott MacDonald
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
        private const double StartupSeconds = 0.2f;
        private const double SweepSeconds = 0.6f;     // how long the attack lasts, sync to animation
        private const string MeleeWeaponName = "MeleeWeapon";
        private const string SlashAnimationName = "Slash";

        private double mSecondsSinceStart = 0.0f;
        private ActionAttackStatus mAttackStatus = ActionAttackStatus.NotStarted;

        /// <summary>
        ///  Get if action has finished.
        /// </summary>
        public bool IsFinished { get { return mAttackStatus == ActionAttackStatus.Finished; } }

        /// <summary>
        ///  Get if actor can move while performing this action.
        /// </summary>
        public bool CanMove { get { return false; } }

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
            var actorSprite = actorGameObject.Get<SpriteComponent>();

            var actor = actorGameObject.Get<ActorComponent>();
            var direction = actor.Direction;

            // Get the weapon game object for animation (Which is attached to the character).
            var weaponGameObject = actorGameObject.FindChildByName(MeleeWeaponName);
            var weaponSprite = (weaponGameObject != null ? weaponGameObject.Get<SpriteComponent>() : null);
                        
            switch ( mAttackStatus )
            {
                case ActionAttackStatus.NotStarted:
                    // Enable the weapon sprite, and animate the attack
                    actorSprite.PlayAnimation(SlashAnimationName, direction );
                    
                    if (weaponSprite != null)
                    {
                        weaponGameObject.Active = true;
                        weaponSprite.PlayAnimation(SlashAnimationName, direction);
                    }

                    // Start animation.
                    mAttackStatus = ActionAttackStatus.StartingUp;

                    break;

                case ActionAttackStatus.StartingUp:
                    // Wait for slash animation to begin sweep animation.
                    if (mSecondsSinceStart < StartupSeconds)
                    {
                        mSecondsSinceStart += deltaTime;
                    }
                    else
                    {
                        mAttackStatus = ActionAttackStatus.Performing;
                    }
                    break;

                case ActionAttackStatus.Performing:
                    // Perform attack hit detection until the animation completes.
                    if (mSecondsSinceStart < StartupSeconds + SweepSeconds)
                    {
                        // Perform attack hit detection
                        DrawHitBox(actorGameObject, currentTimeSeconds);
                        mSecondsSinceStart += deltaTime;
                    }
                    else
                    {
                        // Disable the weapon sprite now that the attack has finished
                        if (weaponGameObject != null)
                        {
                            weaponGameObject.Active = false;
                        }

                        mAttackStatus = ActionAttackStatus.Finished;
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
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
using System.Collections.Generic;
using Scott.Forge;
using Scott.Forge.Engine;
using Scott.Forge.Actors;
using Scott.Forge.Physics;
using Scott.Forge.Sprites;
using Scott.Forge.GameObjects;
using Scott.Forge.Spatial;

namespace Scott.DungeonCrawler.Actions
{
    public enum ActionAttackStatus
    {
        NotStarted,
        StartingUp,
        Performing,
        Finished
    }

    /// <summary>
    ///  Actor attack logic and animation.
    /// </summary>
    /// <remarks>
    ///  TODO: Right now some of the animations do not match the hitbox. The attack sweep north and west animations
    ///  are opposite. Best solution is to store start/finish angles in an array for each direction possibility.
    /// </remarks>
    public class ActionSlashAttack : IActorAction
    {
        private const float StartupSeconds = 0.2f;
        private const float AttackSeconds = 0.3f;     // how long the attack lasts, sync to animation.
        private const float AttackAngleDegreesStart = 80.0f;
        private const float AttackAngleDegreesEnd = -80.0f;
        private const string MeleeWeaponName = "MeleeWeapon";
        private const string SlashAnimationName = "Slash";

        private const float WeaponOffsetX = 32;
        private const float WeaponOffsetY = 32;
        private const float WeaponWidth = 50;
        private const float WeaponHeight = 10;
        private const float WeaponPivotX = 0;
        private const float WeaponPivotY = 5;

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
                    if (mSecondsSinceStart < StartupSeconds + AttackSeconds)
                    {
                        // Perform attack hit detection
                        DrawHitBox(actor, (float)mSecondsSinceStart);
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
        private void DrawHitBox(ActorComponent actor, float elapsedSeconds)
        {
            // Calculate progress of weapon attack animation as a value in the range [0, 1).
            const float AnimationStartTime = StartupSeconds;
            const float AnimationFinishTime = StartupSeconds + AttackSeconds;

            var animationPosition = MathHelper.Clamp(
                MathHelper.NormalizeToZeroOneRange(elapsedSeconds, AnimationStartTime, AnimationFinishTime),
                0.0f,
                1.0f);

            // Calculate the rotation of the weapon hitbox as a function of the animation progress and the actor's
            // current direction.
            var startAngle = MathHelper.DegreeToRadian(AttackAngleDegreesStart);
            var endAngle = MathHelper.DegreeToRadian(AttackAngleDegreesEnd);

            var actorRotationRad = actor.Direction.ToRotationRadians();
            var interpolatedRad = Interpolation.Lerp(startAngle, endAngle, animationPosition);
            var finalRad = MathHelper.NormalizeAngleTwoPi(interpolatedRad + actorRotationRad);
            var radians = finalRad;

            // Generate weapon hitbox.
            var bounds = new BoundingArea(
                actor.Owner.Transform.WorldPosition + new Vector2(WeaponOffsetX, WeaponOffsetY),    // Top left.
                new SizeF(WeaponWidth, WeaponHeight),                                               // Size.
                radians,                                                                            // Rotation.
                new Vector2(WeaponPivotX, WeaponPivotY));                                           // Rotation pivot.

            // Draw the bounding area for visualization testing.
            GameRoot.Debug.DrawBoundingArea(
                bounds,
                Microsoft.Xna.Framework.Color.HotPink);

            // Now find all objects touching the hitbox.
            //  TODO: Find only enemies or are tagged enemy.
            //  TODO: Add bounding region to spatial index query.
            //  TODO: Remove cast from IScene -> Scene once Core/Engine are merged and once IGameObject/IGameScene
            //        interfaces are removed.
            var physics = actor.Owner.Get<PhysicsComponent>();
            var bbrect = new BoundingRect(bounds.AxisAlignedMinPoint, bounds.AxisAlignedMaxPoint);
            var go = (GameObject) actor.Owner;
            var scene = (GameScene) go.Scene;

            foreach (var result in scene.Physics.SpatialIndex.Query(bbrect, physics))
            {
#if DEBUG
                if (GameRoot.Settings.DrawWeaponHitDebug)
                {
                    GameRoot.Debug.DrawFilledRect(
                        new RectF(result.WorldBounds.MinPoint, result.WorldBounds.MaxPoint),
                        Microsoft.Xna.Framework.Color.Red);
                }
#endif
            }

        }
    }
}
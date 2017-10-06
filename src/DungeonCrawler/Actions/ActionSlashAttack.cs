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
using Forge;
using Forge.Gameplay;
using Forge.Physics;
using Forge.Sprites;
using Forge.GameObjects;
using Forge.Spatial;
using DungeonCrawler.Components;

namespace DungeonCrawler.Actions
{
    public enum ActionAttackStatus
    {
        NotStarted,
        StartingUp,
        Performing,
        Finished
    }

    /// <summary>
    ///  Actor melee attack option.
    /// </summary>
    /// <remarks>
    ///  TODO: Right now some of the animations do not match the hitbox. The attack sweep north and west animations
    ///  are opposite. Best solution is to store start/finish angles in an array for each direction possibility.
    /// </remarks>
    public class ActionSlashAttack : IGameplayAction
    {
        // how long before attack starts (the windup).
        private static readonly TimeSpan StartupSeconds = TimeSpan.FromSeconds(0.2);
        // how long the attack lasts, sync to animation.
        private static readonly TimeSpan AttackSeconds = TimeSpan.FromSeconds(0.3);
        private const float AttackAngleDegreesStart = 80.0f;
        private const float AttackAngleDegreesEnd = -80.0f;
        private const string SlashAnimationName = "Slash";

        private const float WeaponOffsetX = 0;
        private const float WeaponOffsetY = 0;
        private const float WeaponWidth = 50;
        private const float WeaponHeight = 10;
        private const float WeaponPivotX = 0;
        private const float WeaponPivotY = 5;

        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private ActionAttackStatus _state = ActionAttackStatus.NotStarted;

        private GameObject _weapon = null;

        /// <summary>
        ///  Constructor
        /// </summary>
        public ActionSlashAttack()
        {
        }

        /// <inheritdoc />
        public bool IsFinished { get => _state == ActionAttackStatus.Finished; }

        /// <inheritdoc />
        public bool CanMove { get => false; }

        /// <inheritdoc />
        public void Start(GameObject self)
        {
            // Get the melee weapon from the game actor's primary weapon slot.
            //  TODO: Handle offhand weapons.
            //  TODO: Handle failures.
            //  TODO: Get weapon info from a weapon component.
            var equipment = self.Get<EquipmentComponent>();
            _weapon = equipment.PrimaryHand ?? throw new InvalidOperationException("Weapon not found");
        }

        /// <inheritdoc />
        public void Update(GameObject self, TimeSpan currentTimeSeconds, TimeSpan deltaTime)
        {
            var actorSprite = self.Get<SpriteComponent>();

            var actor = self.Get<LocomotionComponent>();
            var direction = self.Transform.Forward;

            var weaponSprite = _weapon.Get<SpriteComponent>();

            switch (_state)
            {
                case ActionAttackStatus.NotStarted:
                    // Enable the weapon sprite, and animate the attack
                    actorSprite.PlayAnimation(SlashAnimationName);

                    _weapon.Active = true;
                    weaponSprite.PlayAnimation(SlashAnimationName);

                    // Start animation.
                    _state = ActionAttackStatus.StartingUp;

                    break;

                case ActionAttackStatus.StartingUp:
                    // Wait for slash animation to begin sweep animation.
                    if (_elapsedTime < StartupSeconds)
                    {
                        _elapsedTime += deltaTime;
                    }
                    else
                    {
                        _state = ActionAttackStatus.Performing;
                    }
                    break;

                case ActionAttackStatus.Performing:
                    // Perform attack hit detection until the animation completes.
                    if (_elapsedTime < StartupSeconds + AttackSeconds)
                    {
                        // Perform attack hit detection
                        DoHitDetection(self, _elapsedTime);
                        _elapsedTime += deltaTime;
                    }
                    else
                    {
                        // Disable the weapon sprite now that the attack has finished.
                        _weapon.Active = false;

                        _state = ActionAttackStatus.Finished;
                    }
                    break;

                case ActionAttackStatus.Finished:
                    break;
            }
        }

        /// <summary>
        /// Draws a hit box for the game
        /// </summary>
        private void DoHitDetection(GameObject self, TimeSpan time)
        {
            // Calculate progress of weapon attack animation as a value in the range [0, 1).
            var AnimationStartTime = StartupSeconds;
            var AnimationFinishTime = StartupSeconds + AttackSeconds;

            var animationPosition = MathHelper.Clamp(
                MathHelper.NormalizeToZeroOneRange(
                    (float)time.TotalSeconds,
                    (float)AnimationStartTime.TotalSeconds,
                    (float)AnimationFinishTime.TotalSeconds),
                0.0f,
                1.0f);

            // Calculate the rotation of the weapon hitbox as a function of the animation progress and the actor's
            // current direction.
            var startAngle = MathHelper.DegreeToRadian(AttackAngleDegreesStart);
            var endAngle = MathHelper.DegreeToRadian(AttackAngleDegreesEnd);

            var actorRotationRad = self.Transform.WorldRotation;
            var interpolatedRad = Interpolation.Lerp(startAngle, endAngle, animationPosition);
            var finalRad = MathHelper.NormalizeAngleTwoPi(interpolatedRad + actorRotationRad);
            var radians = finalRad;
            
            // Generate weapon hitbox.
            var bounds = new BoundingArea(
                self.Transform.WorldPosition + new Vector2(WeaponOffsetX, WeaponOffsetY), // Top left.
                new SizeF(WeaponWidth, WeaponHeight),                                 // Size.
                radians,                                                              // Rotation.
                new Vector2(WeaponPivotX, WeaponPivotY));                             // Rotation pivot.

            DrawWeaponDamageArea(self, bounds);

            // Now find all objects touching the hitbox.
            //  TODO: Find only enemies or are tagged enemy.
            //  TODO: Add bounding region (rather than just bounding rect) to spatial index query.
            var physics = self.Get<PhysicsComponent>();
            var bbrect = new BoundingRect(bounds.AxisAlignedMinPoint, bounds.AxisAlignedMaxPoint);
            var scene = self.Scene;

            foreach (var result in scene.Physics.SpatialIndex.Query(bbrect, physics))
            {
                // Apply damage to any game object in the blast zone.
                //  TODO: Use a damage component to calculate damage reductions and whatnot.
                var otherDamaage = result.Owner.TryGet<DamageComponent>();

                if (otherDamaage != null)
                {
                    // TODO: Get damage value from weapon component.
                    otherDamaage.TakeIncomingDamage(25.0f);
                }

#if DEBUG
                // TODO: Move this into damage component.
                // Draw hit visualization  in debug mode.
                var hitbox = new RectF(result.WorldBounds.MinPoint, result.WorldBounds.MaxPoint);
                DrawWeaponAttackedBox(self, hitbox);
#endif
            }

        }

        /// <summary>
        ///  Draw area that weapon will deal damage.
        /// </summary>
        private void DrawWeaponDamageArea(GameObject self, BoundingArea damageArea)
        {
            // Need to get the camera to convert from world space to camera space for debugging.
            var camera = self.Scene.MainCamera;
            var positionInCameraSpace = camera.WorldToScreen(damageArea.WorldPosition);

            var oldWorldPosition = damageArea.WorldPosition;
            damageArea.WorldPosition = positionInCameraSpace;

            // Draw the bounding area for visualization testing.
            Globals.Debug.DrawBoundingArea(
                damageArea,
                Microsoft.Xna.Framework.Color.HotPink);

            // REAPPLY
            damageArea.WorldPosition = oldWorldPosition;
        }

        private void DrawWeaponAttackedBox(GameObject self, RectF hitbox)
        {
            // Need to get the camera to convert from world space to camera space for debugging.
            var camera = self.Scene.MainCamera;
            hitbox  = camera.WorldToScreen(hitbox);

            Globals.Debug.DrawFilledRect(
                hitbox,
                Microsoft.Xna.Framework.Color.Red);
        }
    }
}
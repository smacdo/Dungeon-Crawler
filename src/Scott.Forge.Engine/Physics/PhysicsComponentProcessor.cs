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
using Scott.Forge.Engine.Graphics;
using Scott.Forge.GameObjects;
using Scott.Forge.Spatial;

namespace Scott.Forge.Engine.Physics
{
    /// <summary>
    ///  A simple proof of concept collision detection and response processor.
    /// </summary>
    /// <remarks>
    ///  TODO: Make the collision detection algorithm not suck nearly as much. Right now I am using
    ///        an N^2 algorithm.
    /// 
    ///  Good overal documentation, has a list of high level steps to perform in order. Also talks a lot about grid
    ///  partitioning.
    ///  http://buildnewgames.com/broad-phase-collision-detection/
    /// 
    /// http://elancev.name/oliver/2D%20polygon.htm
    /// </summary>
    public class PhysicsComponentProcessor : ComponentProcessor<PhysicsComponent>
    {
        private ISpatialIndex<PhysicsComponent> mSpatialIndex = new SimpleSpatialIndex<PhysicsComponent>();

        public ISpatialIndex<PhysicsComponent> SpatialIndex { get { return mSpatialIndex; } }
        
        /// <summary>
        ///  Update all attached physics components.
        /// </summary>
        /// <param name="currentTime">Current elapsed simulation time in seconds.</param>
        /// <param name="deltaTime">Seconds since last call to update.</param>
        public override void Update(double currentTime, double deltaTime)
        {
            CalculateDesiredMovements(currentTime, deltaTime);
            FindAndResolveCollisions();

            DrawCollisionBoxes();
        }

        /// <summary>
        ///  Apply movement physics and calculate where each physics object would like to move to. Updates the spatial
        ///  index with the current position of each physics object.
        /// </summary>
        /// <param name="currentTime">Current elapsed simulation time in seconds.</param>
        /// <param name="deltaTime">Seconds since last call to update.</param>
        private void CalculateDesiredMovements(double currentTime, double deltaTime)
        {
            // Reset the spatial index to clear all physics components and their positions.
            // TODO: Is there a way to speed this up by not resetting everyone every update cycle?
            mSpatialIndex.Clear();

            for (int i = 0; i < mComponents.Count; i++)
            {
                var c = mComponents[i];

                // Add physics component to the spatial index.
                mSpatialIndex.Add(c, c.WorldBounds);

                // Calculate the new desired position of this physics componet. This value will be fed into the
                // collision and collison response solver.
                CalculateDesiredMovement(mComponents[i], currentTime, deltaTime);
            }
        }

        /// <summary>
        ///  Apply movement physics and calculate where the physics object would like to move to.
        /// </summary>
        /// <param name="c">Physics component to update.</param>
        /// <param name="currentTime">Current elapsed simulation time in seconds.</param>
        /// <param name="deltaTime">Seconds since last call to update.</param>
        private void CalculateDesiredMovement(PhysicsComponent c, double currentTime, double deltaTime)
        {
            // TODO: Rewrite the physics code below because the current implementation sucks. Its a big hack with
            //       nothing simulated accurately. Actually I bet the code doesn't even follow the comments I wrote...
            //       ... after all it's been years since I updated this and I kind of refactored it too.

            // Apply acceleration and friction.
            if (c.Acceleration.LengthSquared != 0.0f)
            {
                c.Velocity += (c.Acceleration * (float) deltaTime);
                c.Velocity += (c.Velocity.Normalized().Negated() * 0.95f);       // bad way to do friction.
            }
            
            // Limit velocity to a maximum value.
            //  TODO: Do this proper: http://answers.unity3d.com/questions/9985/limiting-rigidbody-velocity.html
            if (c.Velocity.LengthSquared > c.MaxSpeed * c.MaxSpeed)
            {
                c.Velocity = c.Velocity.Normalized() * c.MaxSpeed;
            }

            // Calculate new desired position.
            var desiredPosition = (c.Owner != null ? c.Owner.Transform.WorldPosition : Vector2.Zero);
            desiredPosition += (c.Velocity * (float) deltaTime);

            c.DesiredPosition = desiredPosition;

            // Temporary hack : stop movement
            c.Acceleration = Vector2.Zero;
        }
        
        /// <summary>
        ///  Iterate through all physics objects and resolve any pending collisions before placing each object at their
        ///  desired position.
        /// </summary>
        private void FindAndResolveCollisions()
        {
            Vector2 minimumTranslationVector = Vector2.Zero;

            for (int i = 0; i < mComponents.Count; ++i)
            {
                ResolveCollisionsFor(mComponents[i]);
            }
        }

        /// <summary>
        ///  Place the given physics object at its desired position and resolve any collisions before updating the
        ///  spatial index with the final position.
        /// </summary>
        /// <param name="physics">Physics component to update.</param>
        private void ResolveCollisionsFor(PhysicsComponent physics)
        {
            // Check if this object is outside level bounds and nudge it back in.
            // TODO: Combine the check and displacement calculation functions into one function.
            if (!IsInLevelBounds(physics.DesiredWorldBounds))
            {
                var displacemnet = GetLevelOutOfBoundsDisplacement(physics.DesiredWorldBounds);
                physics.DesiredPosition += displacemnet;

                physics.RaiseOnLevelBoundsCollision();
            }

            // Check for collisions at the object's desired location.
            var collidee = mSpatialIndex.QueryOne(physics.DesiredWorldBounds, physics);

            if (collidee == null)
            {
                // Nothing is colliding with this object so simply set the object's actual position to be its desired
                // position.
                physics.Owner.Transform.WorldPosition = physics.DesiredPosition;
            }
            else
            {
                // There is an object colliding with this physics component. Calculate the minimum vector to displace
                // the colliding object, and then displace the colliding pair in such a way as to ensure both objects
                // are no longer colliding.
                physics.RaiseOnCollisionEvent(collidee.Owner);
                collidee.RaiseOnCollisionEvent(physics.Owner);

                // TODO: Do not recalculate collision to get displacement angle.
                var minimumDisplacement =
                    physics.DesiredWorldBounds.GetMinimumDisplacementAngle(collidee.WorldBounds);                

                // Displace the object along the smaller of the two axis from the displacement vector.
                var displacement = Vector2.Zero;
                float minDX = minimumDisplacement.X;
                float minDY = minimumDisplacement.Y;

                if (minDX == 0.0f)
                {
                    displacement.Y = minDY;
                }
                else if (minDY == 0.0f)
                {
                    displacement.X = minDX;
                }
                else if (Math.Abs(minDX) <= Math.Abs(minDY))
                {
                    displacement.X = minDX;
                }
                else
                {
                    displacement.Y = minDY;
                }

                // TODO: If the owner has a movement component attached then consider apply separation force.
                physics.Owner.Transform.WorldPosition += displacement;
            }

            // Update spatial index with the final position of this physics component.
            mSpatialIndex.Update(physics, physics.WorldBounds);
        }

        /// <summary>
        ///  Draw debug collision information.
        /// </summary>
        private void DrawCollisionBoxes()
        {
            for (int i = 0; i < mComponents.Count; ++i)
            {
                var cc = mComponents[i];
                var color = false/*cc.CollisionThisFrame*/ ? 
                    Microsoft.Xna.Framework.Color.Red :
                    Microsoft.Xna.Framework.Color.Yellow;

                GameRoot.Debug.DrawBoundingRect(
                    cc.WorldBounds,
                    color);
            }
        }

        /// <summary>
        ///  Check if the given area is inside of the level bounds.
        /// </summary>
        /// <remarks>
        ///  TODO: Use the scene dimensions rather than screen dimensions!
        /// </remarks>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsInLevelBounds(BoundingRect bounds)
        {
            return
                bounds.MinPoint.X >= 0 &&
                bounds.MaxPoint.X < Screen.Width &&
                bounds.MinPoint.Y >= 0 &&
                bounds.MaxPoint.Y < Screen.Height;
        }

        public static Vector2 GetLevelOutOfBoundsDisplacement(BoundingRect bounds)
        {
            var result = Vector2.Zero;

            var left = bounds.MinPoint.X;
            var right = bounds.MaxPoint.X;
            var top = bounds.MinPoint.Y;
            var bottom = bounds.MaxPoint.Y;
            
            if (left < 0)
            {
                result.X = -left;
            }
            else if (right > Screen.Width)
            {
                result.X = -(right - Screen.Width);
            }
            
            if (top < 0)
            {
                result.Y = -top;
            }
            else if (bottom > Screen.Height)
            {
                result.Y = -(bottom - Screen.Height);
            }

            return result;
        }


        protected override void UpdateComponent(PhysicsComponent component, double currentTime, double deltaTime)
        {
            // Empty.
        }
    }
}

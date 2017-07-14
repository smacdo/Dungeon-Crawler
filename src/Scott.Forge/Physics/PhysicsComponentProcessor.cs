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
using Scott.Forge.GameObjects;
using Scott.Forge.Graphics;
using Scott.Forge.Spatial;

namespace Scott.Forge.Physics
{
    /// <summary>
    ///  A simple proof of concept physics based movemen engine and collision response solver.
    /// </summary>
    /// <remarks>
    ///  Some reference documents that were consulted while building the physics engine.
    ///  http://buildnewgames.com/broad-phase-collision-detection/
    ///  http://elancev.name/oliver/2D%20polygon.htm
    /// </summary>
    public class PhysicsComponentProcessor : ComponentProcessor<PhysicsComponent>
    {
        public SpatialIndex<PhysicsComponent> SpatialIndex { get; private set; }
            = new SpatialIndex<PhysicsComponent>();

        /// <summary>
        ///  Constructor.
        /// </summary>
        public PhysicsComponentProcessor(GameScene scene)
            : base(scene)
        {
            if (scene == null)
            {
                throw new ArgumentNullException(nameof(scene));
            }
        }

        /// <summary>
        ///  Update all attached physics components.
        /// </summary>
        /// <param name="currentTime">Current elapsed simulation time in seconds.</param>
        /// <param name="deltaTime">Seconds since last call to update.</param>
        public override void Update(double currentTime, double deltaTime)
        {
            CalculateDesiredMovements(currentTime, deltaTime);
            FindAndResolveCollisions();
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
            SpatialIndex.Clear();

            for (int i = 0; i < mComponents.Count; i++)
            {
                var c = mComponents[i];

                // Add physics component to the spatial index.
                SpatialIndex.Add(c, c.WorldBounds);

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
            var outOfBoundsDisplacement = Vector2.Zero;

            if (!IsInLevelBounds(physics.DesiredWorldBounds, ref outOfBoundsDisplacement))
            {
                physics.DesiredPosition += outOfBoundsDisplacement;
                physics.RaiseOnLevelBoundsCollision();
            }

            // Perform tile map collisions beofre object to object collision.
            var tilemapDisplacement = CalculateDisplacementForTileMapCollision(physics);

            if (tilemapDisplacement != Vector2.Zero)
            {
                physics.DesiredPosition += tilemapDisplacement;
            }

            // Check for collisions at the object's desired location.
            var collidee = SpatialIndex.QueryOne(physics.DesiredWorldBounds, physics);

            if (collidee == null)
            {
                // Nothing is colliding with this object so set the object's actual position to be its desired
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

                // TODO: If the owner has a movement component attached then consider apply separation force.
                // TODO: Do not recalculate collision to get displacement angle.
                var displacement =
                    physics.DesiredWorldBounds.GetAxisAlignedDisplacementVector(collidee.WorldBounds);
                
                physics.Owner.Transform.WorldPosition += displacement;
            }

            // Update spatial index with the final position of this physics component.
            SpatialIndex.Update(physics, physics.WorldBounds);
        }

        /// <summary>
        ///  Calculate the displacement vector to move a game object into a non-colliding area of the tile map.
        /// </summary>
        /// <param name="physics">Physics component to test.</param>
        /// <returns>Displacement vector.</returns>
        public Vector2 CalculateDisplacementForTileMapCollision(PhysicsComponent physics)
        {
            var finalDisplacement = Vector2.Zero;

            // Check tile collisions.
            // TODO: Profile and improve performance.
            // TODO: Handle cass where there is no tilemap.
            // TODO: Add special case 4 corner for better performance. (no for loops).
            var tilemap = Scene.Tilemap;
            var bounds = physics.DesiredWorldBounds;

            int leftTile = (int)Math.Floor(bounds.MinPoint.X / tilemap.TileWidth);
            int rightTile = (int)Math.Ceiling(bounds.MaxPoint.X / tilemap.TileWidth);
            int topTile = (int)Math.Floor(bounds.MinPoint.Y / tilemap.TileHeight);
            int bottomTile = (int)Math.Ceiling(bounds.MaxPoint.Y / tilemap.TileHeight);

            // TODO: Don't blindly clamp, find out why certain values are out of bounds (especially collision corner
            // cases that used to crash with out of bounds errors).
            leftTile = MathHelper.Clamp(leftTile, 0, tilemap.Cols - 1);
            rightTile = MathHelper.Clamp(rightTile, 0, tilemap.Cols - 1);

            topTile = MathHelper.Clamp(topTile, 0, tilemap.Rows - 1);
            bottomTile = MathHelper.Clamp(bottomTile, 0, tilemap.Rows - 1);

            var tileExtents = new SizeF(tilemap.TileWidth / 2, tilemap.TileHeight / 2);

            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    var tile = tilemap.Grid[x, y];

                    if (tile.Collision != 0)            // TODO: Use more fine grained points
                    {
                        var tileCenter = tilemap.GetWorldPositionForTile(new Point2(x, y));
                        var tileBounds = new BoundingRect(
                            center: tileCenter,
                            extentSize: tileExtents);

                        var depth = physics.DesiredWorldBounds.GetIntersectionDepth(tileBounds);

                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve shallow
                            if (absDepthX < absDepthY)
                            {
                                finalDisplacement += new Vector2(depth.X, 0);
                            }
                            else
                            {
                                finalDisplacement += new Vector2(0, depth.Y);
                            }
                        }
                    }
                }
            }

            return finalDisplacement;
        }

        /// <summary>
        ///  Check if the given bounding rectangle is inside of the level boundaries. If not then calculate a
        ///  displacement vector to nudge the rectangle back.
        /// </summary>
        /// <param name="bounds">Bounding rect to check.</param>
        /// <param name="displacement">Receives a displacement if bounding rect is not in level.</param>
        /// <returns>True if bounding rect is in level, false otherwise.</returns>
        public bool IsInLevelBounds(BoundingRect bounds, ref Vector2 displacement)
        {
            bool isInBounds =
                bounds.MinPoint.X >= 0 &&
                bounds.MaxPoint.X < Scene.Width &&
                bounds.MinPoint.Y >= 0 &&
                bounds.MaxPoint.Y < Scene.Height;

            if (!isInBounds)
            {
                var left = bounds.MinPoint.X;
                var right = bounds.MaxPoint.X;
                var top = bounds.MinPoint.Y;
                var bottom = bounds.MaxPoint.Y;

                if (left < 0)
                {
                    displacement.X = -left;
                }
                else if (right > Scene.Width)
                {
                    displacement.X = -(right - Scene.Width);
                }

                if (top < 0)
                {
                    displacement.Y = -top;
                }
                else if (bottom > Scene.Height)
                {
                    displacement.Y = -(bottom - Scene.Height);
                }
            }

            return isInBounds;
        }

        protected override void UpdateComponent(PhysicsComponent component, double currentTime, double deltaTime)
        {
            // Empty.
        }
    }
}

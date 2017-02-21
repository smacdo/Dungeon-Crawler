/*
 * Copyright 2012-2015 Scott MacDonald
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
using Scott.Forge.Engine.Sprites;
using System.Diagnostics;
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
    public class CollisionProcessor : ComponentProcessor<CollisionComponent>
    {
        private List<Pair<CollisionComponent, CollisionComponent>> mCollisions;
        private List<CollisionComponent> mCollisionQueryList;
        private ISpatialIndex<CollisionComponent> mSpatialIndex;

        public CollisionProcessor()
        {
            mCollisions = new List<Pair<CollisionComponent, CollisionComponent>>(1024);
            mCollisionQueryList = new List<CollisionComponent>(100);

            mSpatialIndex = new SimpleSpatialIndex<CollisionComponent>(10000, 10000);
        }

        public override void Update(double currentTime, double deltaTime)
        {
            mCollisions.Clear();
            mCollisionQueryList.Clear();

            UpdateCollisionComponents();
            FindCollisions(mCollisions);
            //ResolveCollisions(mCollisions);

            DrawCollisionBoxes();
        }

        protected override void UpdateComponent(CollisionComponent component, double currentTime, double deltaTime)
        {

        }

        /// <summary>
        ///  Update collision component position and rotation from the owner's transform and then update the scene
        ///  graph's spatial index to allow for fast spatial queries.
        /// </summary>
        private void UpdateCollisionComponents()
        {
            // TODO: Reset the spatial scene graph.
            // TODO: Is there a way to speed this up by not resetting everyone?
            mSpatialIndex.Clear();

            // Iterate through all collision components and update the scene spatial index.
            for (int i = 0; i < mComponents.Count; ++i)
            {
                var collider = mComponents[i];

                // Calculate new position for collider.
                // TODO: Fix this up, it is messy.
                // TODO: Remove broad phase box, use broad phase from bounding area.
                //   var desiredPosition = collider.Owner.Transform.WorldPosition;
                //   var startingPosition = collider.ActualPosition; // Actual position from last update call.


                // Make sure collider is still in the world bounds.
                // TODO: If collide with world bound, carefully check rotated bounding area.
                // TODO: Move the collider as close to the edge of the world bound rather than simply not updating the
                //       position.
                // TODO: Handle case where original box position is outside world bounds.

                var desiredBounds = collider.Bounds.AABB;
                desiredBounds.Position = collider.Owner.Transform.WorldPosition + collider.Offset;

                if (!IsInLevelBounds(desiredBounds))
                {
                    // TODO: Correctly move back to boundary rather than this snap back.
                    collider.DesiredPosition = collider.ActualPosition;
                    collider.Owner.Transform.WorldPosition = collider.ActualPosition;
                }
                else
                {
                    collider.DesiredPosition = collider.Owner.Transform.WorldPosition;
                }
                
                // Update spatial index with new collision bounds.
                var initialBounds = collider.Bounds.AABB;
                initialBounds.Position = collider.ActualPosition + collider.Offset;

                mSpatialIndex.Add(collider, collider.Bounds);
                collider.CollisionThisFrame = false;
            }
        }

        /// <summary>
        ///  Find collisions between objects and resolve them.
        /// </summary>
        /// <param name="collisions"></param>
        private void FindCollisions(List<Pair<CollisionComponent, CollisionComponent>> collisions)
        {
            Vector2 minimumTranslationVector = Vector2.Zero;

            for (int i = 0; i < mComponents.Count; ++i)
            {
                ResolveCollisionsFor(mComponents[i]);
            }
        }

        private void ResolveCollisionsFor(CollisionComponent collider)
        {
            mCollisions.Clear();

            // Calculate the desired position for this collider and see what it collides with.
            // TODO: Horrible code.
            collider.Bounds.WorldPosition = collider.DesiredPosition + collider.Offset;

            if (mSpatialIndex.Query(collider.Bounds, collider, mCollisionQueryList))
            {
                var collidee = mCollisionQueryList[0];
                
                // TODO: Do not recalculate collision to get displacement angle.
                var minimumDisplacement = Vector2.Zero;
                collider.Bounds.Intersects(collidee.Bounds, ref minimumDisplacement);

                // Mark components as having collided this frame.
                // TODO: Raise an event instead.
                collider.CollisionThisFrame = true;
                collidee.CollisionThisFrame = true;

                // Do not adjust position if this is a glancing collision.
                if (minimumDisplacement.IsZero)
                {
                    collider.ActualPosition = collider.DesiredPosition;
                }

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
                    
                /*Debug.WriteLine("Collision depth {0}, displacement ({1}, {2}), actual ({3}, {4})",
                    0,
                    minDX, minDY,
                    displacement.X, displacement.Y);
                Debug.WriteLine("Collider rect was {0}, collidee rect was {1}",
                    collider.Bounds.AABB,
                    collidee.Bounds.AABB);*/

                collider.Owner.Transform.WorldPosition += displacement;
                collider.Bounds.WorldPosition += displacement;
                collider.ActualPosition = collider.Owner.Transform.WorldPosition;

                /*Debug.WriteLine("Collider new rect is {0} and pos {1}",
                    collider.Bounds.AABB,
                    collider.Owner.Transform.WorldPosition);*/

                // Update spatial index with collider's new bounding area.
                mSpatialIndex.Update(collider, collider.Bounds);

                // ...
                // TODO: This is terribly written. Clean up.
            }
        }

        /// <summary>
        ///  Resolve collisions.
        /// </summary>
        /// <param name="collisions"></param>
        private void ResolveCollisions(List<Pair<CollisionComponent, CollisionComponent>> collisions)
        {
            var minimumTranslation = Vector2.Zero;

            for (int i = 0; i < collisions.Count; ++i)
            {
                var first = collisions[i].First;
                var second = collisions[i].Second;

                // TODO: If the owner has a movement component attached then apply separation force.

                if (first.Bounds.Intersects(second.Bounds, ref minimumTranslation))
                {
                    first.Owner.Transform.WorldPosition += minimumTranslation;
                }
            }
        }

        /// <summary>
        ///  Draw debug collision information.
        /// </summary>
        private void DrawCollisionBoxes()
        {
            for (int i = 0; i < mComponents.Count; ++i)
            {
                var cc = mComponents[i];
                var color = cc.CollisionThisFrame ? 
                    Microsoft.Xna.Framework.Color.Red :
                    Microsoft.Xna.Framework.Color.Yellow;

                GameRoot.Debug.DrawBoundingArea(
                    cc.Bounds,
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
        public static bool IsInLevelBounds(RectF bounds)
        {
            return bounds.TopLeft.X >= 0 &&
                   bounds.TopRight.X < Screen.Width &&
                   bounds.TopLeft.Y >= 0 &&
                   bounds.BottomLeft.Y < Screen.Height;
        }
    }
}

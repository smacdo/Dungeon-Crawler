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
        private List<CollisionComponent> mCollisionQueryList;
        private ISpatialIndex<CollisionComponent> mSpatialIndex;

        public ISpatialIndex<CollisionComponent> SpatialIndex { get { return mSpatialIndex; } }

        public CollisionProcessor()
        {
            mCollisionQueryList = new List<CollisionComponent>(100);
            mSpatialIndex = new SimpleSpatialIndex<CollisionComponent>(10000, 10000);
        }

        public override void Update(double currentTime, double deltaTime)
        {
            mCollisionQueryList.Clear();

            UpdateCollisionComponents();
            FindCollisions();

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

                // Create a bounding box for the desired position of this game object. If the collider is outside of
                // the level boundaries then nudge it back into the world before proceeding with collision detection.
                collider.DesiredPosition = collider.Owner.Transform.WorldPosition;

                var desiredBounds = collider.Bounds.AABB;
                desiredBounds.Position = collider.DesiredPosition + collider.Offset;

                if (!IsInLevelBounds(desiredBounds))
                {
                    var displacemnet = GetLevelOutOfBoundsDisplacement(desiredBounds);

                    collider.DesiredPosition += displacemnet;
                    collider.Owner.Transform.WorldPosition += displacemnet;

                    collider.RaiseOnLevelBoundsCollision();
                }
                
                // Update spatial index with new collision bounds.
                var initialBounds = collider.Bounds.AABB;
                initialBounds.Position = collider.ActualPosition + collider.Offset;

                mSpatialIndex.Add(collider, collider.Bounds);
            }
        }

        /// <summary>
        ///  Find collisions between objects and resolve them.
        /// </summary>
        /// <param name="collisions"></param>
        private void FindCollisions()
        {
            Vector2 minimumTranslationVector = Vector2.Zero;

            for (int i = 0; i < mComponents.Count; ++i)
            {
                ResolveCollisionsFor(mComponents[i]);
            }
        }

        private void ResolveCollisionsFor(CollisionComponent collider)
        {
            mCollisionQueryList.Clear();

            // Calculate the desired position for this collider and see what it collides with.
            // TODO: Horrible code.
            collider.Bounds.WorldPosition = collider.DesiredPosition + collider.Offset;

            if (mSpatialIndex.Query(collider.Bounds, collider, mCollisionQueryList))
            {
                var collidee = mCollisionQueryList[0];
                
                // TODO: Do not recalculate collision to get displacement angle.
                var minimumDisplacement = Vector2.Zero;
                collider.Bounds.Intersects(collidee.Bounds, ref minimumDisplacement);

                // Notify components of collision.
                collider.RaiseOnCollisionEvent(collidee.Owner);
                collidee.RaiseOnCollisionEvent(collider.Owner);

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

                // TODO: If the owner has a movement component attached then consider apply separation force.


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
                var color = false/*cc.CollisionThisFrame*/ ? 
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

        public static Vector2 GetLevelOutOfBoundsDisplacement(RectF bounds)
        {
            var result = Vector2.Zero;
            
            if (bounds.Left < 0)
            {
                result.X = -bounds.Left;
            }
            else if (bounds.Right > Screen.Width)
            {
                result.X = -(bounds.Right - Screen.Width);
            }
            
            if (bounds.Top < 0)
            {
                result.Y = -bounds.Top;
            }
            else if (bounds.Bottom > Screen.Height)
            {
                result.Y = -(bounds.Bottom - Screen.Height);
            }

            return result;
        }
    }
}

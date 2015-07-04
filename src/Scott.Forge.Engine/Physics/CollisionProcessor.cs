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

        public CollisionProcessor()
        {
            mCollisions = new List<Pair<CollisionComponent, CollisionComponent>>(1024);
        }

        public override void Update(double currentTime, double deltaTime)
        {
            mCollisions.Clear();

            ResetAllCollisions();
            FindCollisions(mCollisions);
            ResolveCollisions(mCollisions);

            DrawCollisionBoxes();
        }

        private void ResetAllCollisions()
        {
            for (int i = 0; i < mComponents.Count; ++i)
            {
                mComponents[i].Update();
            }
        }

        private void FindCollisions(List<Pair<CollisionComponent, CollisionComponent>> collisions)
        {
            for (int i = 0; i < mComponents.Count; ++i)
            {
                var first = mComponents[i];

                for (int j = i + 1; j < mComponents.Count; ++j)
                {
                    var second = mComponents[j];

                    // Perform a fast broadphase check for collision.
                    if (first.BroadPhaseBox.Intersects(second.BroadPhaseBox))
                    {
                        collisions.Add(new Pair<CollisionComponent, CollisionComponent>(first, second));

                        first.CollisionThisFrame = true;
                        second.CollisionThisFrame = true;
                    }
                }
            }
        }

        private void ResolveCollisions(List<Pair<CollisionComponent, CollisionComponent>> collisions)
        {
            var minimumTranslation = Vector2.Zero;

            for (int i = 0; i < collisions.Count; ++i)
            {
                var first = collisions[i].First;
                var second = collisions[i].Second;


                if (first.Bounds.Intersects(second.Bounds, ref minimumTranslation))
                {
                    Debug.WriteLine(
                        string.Format(
                            "Collision! A: {0}, B: {1}, mT: {2}",
                            first.BroadPhaseBox.Position,
                            second.BroadPhaseBox.Position,
                            minimumTranslation));

                    aaafirst.Owner.Transform.Position += minimumTranslation;

                    GameRoot.Debug.DrawLine(
                        second.Bounds.WorldPosition,
                        second.Bounds.WorldPosition + minimumTranslation,
                        Microsoft.Xna.Framework.Color.Orange);
                }
            }
        }

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

        protected override void UpdateComponent(CollisionComponent cc, double time, double deltaTime)
        {

        }

        /// <summary>
        ///  Check if the given area is inside of the level bounds.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsInLevelBounds(RectF bounds)
        {
            return bounds.TopLeft.X > 0 &&
                   bounds.TopRight.X < Screen.Width &&
                   bounds.TopLeft.Y > 0 &&
                   bounds.BottomLeft.Y < Screen.Height;
        }
    }
}

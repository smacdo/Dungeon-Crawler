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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scott.Forge.Spatial;

namespace Scott.Forge.Spatial
{
    /// <summary>
    ///  A simple brute force spatial grid implementatiaon with no optimizations.
    /// </summary>
    public class SimpleSpatialIndex<TObject> : ISpatialIndex<TObject> where TObject : new()
    {
        // TODO: Make default capacity configurable.
        // TODO: Use an array not a list for speed.
        private List<Pair<TObject, BoundingArea>> mList = new List<Pair<TObject, BoundingArea>>(1000);

        // TODO: Add overflow if capacity is exceeded since it is temporary.

        /// <summary>
        ///  Create a new simple spatial index.
        /// </summary>
        /// <param name="width">Width of the spatial index in world units.</param>
        /// <param name="height">Height of the spatial index in world units.</param>
        public SimpleSpatialIndex(float width, float height)
        {
            if (width <= 0)
            {
                throw new ArgumentException("Width must be larger than zero", "width");
            }

            if (height <= 0)
            {
                throw new ArgumentException("Height must be larger than zero", "height");
            }

            Width = width;
            Height = height;
        }

        /// <summary>
        ///  Maximum height of the spatial index.
        /// </summary>
        /// <remarks>
        ///  Spatial index uses the Forge coordinate system where Y increases as it goes down from the top left origin.
        /// </remarks>
        public float Height { get; private set; }

        /// <summary>
        ///  Maximum width of the spatial index.
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        ///  Clear the spatial index.
        /// </summary>
        public void Clear()
        {
            mList.Clear();
        }

        /// <summary>
        ///  Query the spatial index to find a list of objects occupying the requested query region.
        /// </summary>
        /// <param name="queryBounds">Spatial region to search.</param>
        /// <param name="exclude">Object ot exclude from matches (null if not required).</param>
        /// <param name="results">List that will receive query results.</param>
        /// <returns>True if at least one object was in the area, false otherwise.</returns>
        public void Add(TObject obj, BoundingArea bounds)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (bounds == null)
            {
                throw new ArgumentNullException("bounds");
            }

            // Check dimensions, make sure valid before inserting into grid.
            var aabb = bounds.AABB;

            if (aabb.Left < 0 || aabb.Right >= Width || aabb.Top < 0 || aabb.Bottom >= Height)
            {
                throw new ArgumentException("Object bounding area out of bounds", "bounds");
            }

            // Add object to end of object array.
            mList.Add(new Pair<TObject, BoundingArea>(obj, bounds));
        }

        /// <summary>
        ///  Update an object bounding area.
        /// </summary>
        /// <param name="obj">Object to update.</param>
        /// <param name="bounds">New bounding area for object.</param>
        /// <returns>True if the object was found and updated, false if the object was added.</returns>
        public bool Update(TObject obj, BoundingArea bounds)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (bounds == null)
            {
                throw new ArgumentNullException("bounds");
            }

            // Check dimensions, make sure valid before inserting into grid.
            var aabb = bounds.AABB;

            if (aabb.Left < 0 || aabb.Right >= Width || aabb.Top < 0 || aabb.Bottom >= Height)
            {
                throw new ArgumentException("Object bounding area out of bounds", "bounds");
            }

            // Search the list for the given object and update the bounds if found.
            bool didFindObject = false;

            for (int i = 0; i < mList.Count && !didFindObject; i++)
            {
                var entry = mList[i];

                if (ReferenceEquals(obj, entry.First))
                {
                    entry.Second = bounds;
                    didFindObject = true;
                }
            }

            // Add the object if it was not found in the object list.
            if (!didFindObject)
            {
                mList.Add(new Pair<TObject, BoundingArea>(obj, bounds));
            }

            return didFindObject;   
        }

        /// <summary>
        ///  Remove an object from the spatial index.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <returns>True if the object was found and rmeoved, false otherwise.</returns>
        public bool Remove(TObject obj)
        {
            // TODO: This logic needs to be updated when overflow is added.

            // Search for the object.
            bool didFind = false;

            for (int i = 0; i < mList.Count && !didFind; i++)
            {
                var o = mList[i].First;

                // Check if this is the object.
                if (ReferenceEquals(obj, o))
                {
                    didFind = true;
                    // Move object to the last entry in the array.
                    var lastIndex = mList.Count - 1;

                    if (i < lastIndex)
                    {
                        mList[i] = mList[lastIndex];
                    }

                    // Remove last entry in array. This should not be a performance problem because List<T> will not
                    // resize; it will keep entry as extra capacity.
                    mList.RemoveAt(lastIndex);
                }
            }

            return didFind;
        }

        /// <summary>
        ///  Store an object in the spatial index with the given bounding area.
        /// </summary>
        /// <param name="obj">Object to store.</param>
        /// <param name="bounds">Object's bounding area.</param>
        public bool Query(BoundingArea queryBounds, TObject excludes, List<TObject> results)
        {
            bool didFindAnything = false;

            foreach (var p in mList)
            {
                if (!ReferenceEquals(p.First, excludes))
                {
                    if (queryBounds.Intersects(p.Second))
                    {
                        didFindAnything = true;
                        results.Add(p.First);
                    }
                }
            }

            return didFindAnything;
        }
        
    }
}

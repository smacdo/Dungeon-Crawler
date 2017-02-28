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
    /// <remarks>
    ///  TODO: Unit testing!
    /// </remarks>
    public class SimpleSpatialIndex<TObject> : ISpatialIndex<TObject> where TObject : new()
    {
        public const int DefaultCapacity = 1000;

        private Pair<TObject, BoundingRect>[] mItems;
        private int mItemCount = 0;

        // TODO: Add support for item overflow with a linked list. Generate a pool of unused nodes and then add/remove
        //       them as needed at runtime. If the number of needed nodes is exceeded at runtime then allocate new ones
        //       and remember to delete them to return to initial pool capacity.

        /// <summary>
        ///  Constructor.
        /// </summary>
        public SimpleSpatialIndex()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="capacity">Starting object capacity.</param>
        public SimpleSpatialIndex(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException("Capacity must be larger than one", nameof(capacity));
            }

            mItems = new Pair<TObject, BoundingRect>[capacity];
        }

        /// <summary>
        ///  Clear the spatial index.
        /// </summary>
        public void Clear()
        {
            mItemCount = 0;

            // Reset item array to null to zero out live instances (allow garbage collection to kick in).
            Array.Clear(mItems, 0, mItems.Length);
        }

        /// <summary>
        ///  Query the spatial index to find a list of objects occupying the requested query region.
        /// </summary>
        /// <param name="queryBounds">Spatial region to search.</param>
        /// <param name="exclude">Object ot exclude from matches (null if not required).</param>
        /// <param name="results">List that will receive query results.</param>
        /// <returns>True if at least one object was in the area, false otherwise.</returns>
        public void Add(TObject obj, BoundingRect bounds)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (bounds == null)
            {
                throw new ArgumentNullException(nameof(bounds));
            }

            // Check if initial item array is full.
            // TODO: Implement overflow mechanics.
            if (mItemCount + 1 >= mItems.Length)
            {
                throw new InvalidOperationException("Spatial index out of space");
            }

            // Add item to list.
            mItems[mItemCount++] = new Pair<TObject, BoundingRect>(obj, bounds);
        }

        /// <summary>
        ///  Update an object bounding area.
        /// </summary>
        /// <param name=nameof(obj)>Object to update.</param>
        /// <param name=nameof(bounds)>New bounding area for object.</param>
        /// <returns>True if the object was found and updated, false if the object was added.</returns>
        public bool Update(TObject obj, BoundingRect bounds)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (bounds == null)
            {
                throw new ArgumentNullException(nameof(bounds));
            }

            // Search the list for the given object and update the bounds if found.
            // TODO: Add search in overflow list once overflow is supported.
            bool didFindObject = false;

            for (int i = 0; i < mItemCount && !didFindObject; i++)
            {
                var entry = mItems[i];

                if (ReferenceEquals(obj, entry.First))
                {
                    entry.Second = bounds;
                    didFindObject = true;
                }
            }

            // Add the object if it was not found in the object list.
            if (!didFindObject)
            {
                Add(obj, bounds);
            }

            return didFindObject;   
        }

        /// <summary>
        ///  Remove an object from the spatial index.
        /// </summary>
        /// <param name=nameof(obj)>Object to remove.</param>
        /// <returns>True if the object was found and rmeoved, false otherwise.</returns>
        public bool Remove(TObject obj)
        {
            // TODO: This logic needs to be updated when overflow is added.

            // Search for the object.
            bool didFind = false;

            for (int i = 0; i < mItemCount && !didFind; i++)
            {
                var o = mItems[i].First;

                // Check if this is the object.
                if (ReferenceEquals(obj, o))
                {
                    didFind = true;

                    // Move object to the last entry in the array.
                    var lastIndex = mItemCount - 1;

                    if (i < lastIndex)
                    {
                        mItems[i] = mItems[lastIndex];
                    }

                    // Remove last entry in array and then shrink item count.
                    mItems[lastIndex] = default(Pair<TObject, BoundingRect>);
                    mItemCount -= 1;
                }
            }

            return didFind;
        }

        /// <summary>
        ///  Query the spatial index to find the first object occupying the requested region.
        /// </summary>
        /// <param name=nameof(bounds)>Spatial region to search.</param>
        /// <param name="excludes">Object to exclude from matches (null if not required).</param>
        /// <returns>The first object that was found, or null for none.</returns>
        public TObject QueryOne(BoundingRect bounds, TObject excludes)
        {
            var queryResults = Query(bounds, excludes).GetEnumerator();
            TObject result = default(TObject);

            if (queryResults.MoveNext())
            {
                result = queryResults.Current;
            }

            return result;
        }

        /// <summary>
        ///  Query the spatial index to find a list of objects occupying the requested query region.
        /// </summary>
        /// <param name="queryBounds">Spatial region to search.</param>
        /// <param name="exclude">Object to exclude from matches (null if not required).</param>
        /// <param name="results">List that will receive query results.</param>
        /// <returns>True if at least one object was in the area, false otherwise.</returns>
        public bool Query(BoundingRect queryBounds, TObject excludes, IList<TObject> results)
        {
            bool didFind = false;

            foreach (var q in Query(queryBounds, excludes))
            {
                results.Add(q);
                didFind = true;
            }

            return didFind;
        }

        /// <summary>
        ///  Query the spatial index to find a list of objects occupying the requested query region.
        /// </summary>
        /// <param name="queryBounds">Spatial region to search.</param>
        /// <param name="exclude">Object to exclude from matches (null if not required).</param>
        /// <returns>Iterator with the results of the query.</returns>
        public QueryResult Query(BoundingRect queryBounds, TObject excludes)
        {
            return new QueryResult(this, queryBounds, excludes);
        }

        /// <summary>
        ///  Holds the results of a spatial query and has an iterator to easily iterate through the results.
        /// </summary>
        public struct QueryResult
        {
            private readonly SimpleSpatialIndex<TObject> mSpatial;
            private readonly BoundingRect mQueryBounds;
            private readonly TObject mExcludes;
            
            public QueryResult(SimpleSpatialIndex<TObject> spatial, BoundingRect bounds, TObject excludes)
            {
                mSpatial = spatial;
                mQueryBounds = bounds;
                mExcludes = excludes;
            }

            public QueryEnumerator GetEnumerator()
            {
                return new QueryEnumerator(mSpatial, mQueryBounds, mExcludes);
            }
        }

        /// <summary>
        ///  Implementation of a value enumerator.
        /// </summary>
        /// <remarks>
        ///  A struct value enumerator is used to avoid garbage generation for performance reasons.
        /// </remarks>
        public struct QueryEnumerator
        {
            private readonly SimpleSpatialIndex<TObject> mSpatial;
            private readonly BoundingRect mQueryBounds;
            private readonly TObject mExcludes;
            private int mIndex;

            public QueryEnumerator(SimpleSpatialIndex<TObject> spatial, BoundingRect bounds, TObject excludes)
            {
                mSpatial = spatial;
                mQueryBounds = bounds;
                mExcludes = excludes;
                mIndex = 0;
            }

            public TObject Current
            {
                get
                {
                    return mSpatial.mItems[mIndex - 1].First;
                }
            }

            public bool MoveNext()
            {
                while (mIndex < mSpatial.mItemCount)
                {
                    var p = mSpatial.mItems[mIndex++];

                    if (!ReferenceEquals(p.First, mExcludes))
                    {
                        if (mQueryBounds.Intersects(p.Second))
                        {
                            // Object matches!
                            return true;
                        }
                    }
                }

                // No more results.
                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

    }
}

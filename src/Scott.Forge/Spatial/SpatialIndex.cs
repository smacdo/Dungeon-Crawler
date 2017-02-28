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

namespace Scott.Forge.Spatial
{
    /// <summary>
    ///  2d spatial index for objects with bounding areas.
    /// </summary>
    public interface ISpatialIndex<TObject> where TObject : new()
    {
        /// <summary>
        ///  Clear the spatial index.
        /// </summary>
        void Clear();

        /// <summary>
        ///  Store an object in the spatial index with the given bounding area.
        /// </summary>
        /// <param name="obj">Object to store.</param>
        /// <param name="bounds">Object's bounding area.</param>
        void Add(TObject obj, BoundingRect bounds);

        /// <summary>
        ///  Query the spatial index to find the first object occupying the requested region.
        /// </summary>
        /// <param name="bounds">Spatial region to search.</param>
        /// <param name="excludes">Object to exclude from matches (null if not required).</param>
        /// <returns>The first object that was found, or null for none.</returns>
        TObject QueryOne(BoundingRect bounds, TObject excludes);

        /// <summary>
        ///  Query the spatial index to find a list of objects occupying the requested query region.
        /// </summary>
        /// <param name="queryBounds">Spatial region to search.</param>
        /// <param name="exclude">Object to exclude from matches (null if not required).</param>
        /// <param name="results">List that will receive query results.</param>
        /// <returns>True if at least one object was in the area, false otherwise.</returns>
        bool Query(BoundingRect queryBounds, TObject exclude, IList<TObject> results);

        /// <summary>
        ///  Remove an object from the spatial index.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <returns>True if the object was found and rmeoved, false otherwise.</returns>
        bool Remove(TObject obj);

        /// <summary>
        ///  Update an object bounding area.
        /// </summary>
        /// <param name="obj">Object to update.</param>
        /// <param name="bounds">New bounding area for object.</param>
        /// <returns>True if the object was found and updated, false if the object was added.</returns>
        bool Update(TObject obj, BoundingRect bounds);
    }
}

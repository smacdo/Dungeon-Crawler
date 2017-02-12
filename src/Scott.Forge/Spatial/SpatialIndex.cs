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
    ///  2d spatial index for objects with bounding areas. The index can store bounding areas from the origin of (0,0)
    ///  to (width, height).
    /// </summary>
    /// <remarks>
    ///  Objects with bounding regions outside of the size of the spatial index will throw an exception.
    /// </remarks>
    public interface ISpatialIndex<TObject> where TObject : new()
    {
        /// <summary>
        ///  Maximum height of the spatial index.
        /// </summary>
        /// <remarks>
        ///  Spatial index uses the Forge coordinate system where Y increases as it goes down from the top left origin.
        /// </remarks>
        float Height { get; }

        /// <summary>
        ///  Maximum width of the spatial index.
        /// </summary>
        float Width { get; }

        /// <summary>
        ///  Clear the spatial index.
        /// </summary>
        void Clear();

        /// <summary>
        ///  Store an object in the spatial index with the given bounding area.
        /// </summary>
        /// <param name="obj">Object to store.</param>
        /// <param name="bounds">Object's bounding area.</param>
        void Add(TObject obj, BoundingArea bounds);

        /// <summary>
        ///  Query the spatial index to find a list of objects occupying the requested query region.
        /// </summary>
        /// <param name="queryBounds">Spatial region to search.</param>
        /// <param name="exclude">Object ot exclude from matches (null if not required).</param>
        /// <param name="results">List that will receive query results.</param>
        /// <returns>True if at least one object was in the area, false otherwise.</returns>
        bool Query(BoundingArea queryBounds, TObject exclude, List<TObject> results);

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
        bool Update(TObject obj, BoundingArea bounds);
    }
}

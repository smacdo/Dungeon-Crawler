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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.Forge.Content
{
    /// <summary>
    ///  An abstract definition of a container containing zero or more content items.
    /// </summary>
    /// <remarks>
    ///  A content container is an abstract container of content. A container of content could be as simple as a
    ///  directory on the local file system, a zip file or a more exotic concept.
    ///  
    ///  See the ForgeContentManager for more information on how multiple content containers are used by Forge.
    /// </remarks>
    public interface IContentContainer
    {
        /// <summary>
        ///  Try to read a content item from the container.
        /// </summary>
        /// <param name="assetName">Name of the item.</param>
        /// <param name="readStream">A stream that will be set if the container holds the item.</param>
        /// <returns>True if the item is in this container, false otherwise.</returns>
        bool TryReadItem(string assetName, ref Stream readStream);
    }
}

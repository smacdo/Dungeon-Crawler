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
    ///  Represents a physical file system directory that contains content items.
    /// </summary>
    public class DirectoryContentContainer : IContentContainer
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="contentDirectory">Path to the content directory.</param>
        public DirectoryContentContainer(string contentDirectory)
        {
            if (contentDirectory == null)
            {
                throw new ArgumentNullException(contentDirectory);
            }

            contentDirectory = Path.GetFullPath(contentDirectory);

            if (!Directory.Exists(contentDirectory))
            {
                throw new ContentDirectoryDoesNotExistException(contentDirectory);
            }

            ContentDirectory = contentDirectory;
        }

        /// <summary>
        ///  Get or set a path to the content directory.
        /// </summary>
        public string ContentDirectory { get; private set; }

        /// <summary>
        ///  Try to read a content item from the container.
        /// </summary>
        /// <param name="assetName">Name of the item.</param>
        /// <param name="readStream">A stream that will be set if the container holds the item.</param>
        /// <returns>True if the item is in this container, false otherwise.</returns>
        public Task<bool> TryReadItem(string assetName, out Stream readStream)
        {
            if (assetName == null)
            {
                throw new ArgumentNullException(assetName);
            }

            var assetPath = string.Format("{0}\\{1}", ContentDirectory, assetName);

            if (File.Exists(assetPath))
            {
                // Open file stream in async mode.
                readStream = new FileStream(
                    assetPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    true);

                return Task.FromResult(true);
            }
            else
            {
                readStream = null;
                return Task.FromResult(true);
            }
        }
    }

    /// <summary>
    ///  Exception that is thrown if the content directory does not exist.
    /// </summary>
    public class ContentDirectoryDoesNotExistException : ForgeException
    {
        public ContentDirectoryDoesNotExistException(string contentDirectory)
            : base(string.Format("Content directory does not exist: {0}", contentDirectory))
        {
            ContentDirectory = contentDirectory;
        }

        public string ContentDirectory { get; private set; }
    }
}

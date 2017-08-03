/*
 * Copyright 2012-2014 Scott MacDonald
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

namespace Scott.Forge.Content
{
    /// <summary>
    ///  Exception while loading game content.
    /// </summary>
    public class ContentReaderException : ForgeException
    {
        /// <summary>
        ///  Create a new exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ContentReaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///  Create a new exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="filename">Name of the content file causing the exception.</param>
        public ContentReaderException(string message, string filename)
            : base(string.Format("Error reading {0}: {1}", filename, message))
        {
            FileName = filename;
        }

        /// <summary>
        ///  Create a new GameDataException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="filename">Name of the content file causing the exception.</param>
        public ContentReaderException(string message, string filename, Exception inner)
            : base(string.Format("Error reading {0}: {1}", filename, message), inner)
        {
            FileName = filename;
        }

        /// <summary>
        ///  Get the name of the content file that raised this exception.
        /// </summary>
        public string FileName { get; private set; }
    }

    /// <summary>
    ///  Exception when content manager tries to load an asset but cannot find a content reader.
    /// </summary>
    public class ContentReaderMissingException : ContentReaderException
    {
        public ContentReaderMissingException(string fileName)
            : base("Could not find a content reader for this file", fileName)
        {
        }
    }
}

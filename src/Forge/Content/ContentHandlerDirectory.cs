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

namespace Forge.Content
{
    /// <summary>
    ///  Manages a list of classes that can read and write content objects.
    /// </summary>
    public interface IContentHandlerDirectory
    {
        /// <summary>
        ///  Get a content reader that can load read files with a certain file extension.
        /// </summary>
        /// <typeparam name="T">Content type to load.</typeparam>
        /// <param name="fileExtension">File extension for asset filename.</param>
        /// <returns>A content reader that can read the given file extension.</returns>
        IContentReader<T> GetContentReaderFor<T>(string fileExtension);

        /// <summary>
        ///  Try to get a content reader that can load read files with a certain file extension.
        /// </summary>
        /// <typeparam name="T">Content type to load.</typeparam>
        /// <param name="fileExtension">File extension for asset filename.</param>
        /// <param name="reader">Receives the content reader if a reader was found.</param>
        /// <returns>True if a matching content reader was located, false otherwise.</returns>
        bool TryGetContentReaderFor<T>(string fileExtension, ref IContentReader<T> reader);
    }

    /// <summary>
    ///  Simple database of content readers that are manually configured via the constructor or by calling Add. For
    ///  more complex discovery of content readers please derive this class.
    /// </summary>
    public class ContentHandlerDirectory : IContentHandlerDirectory
    {
        private List<ReaderEntry> ContentReaders = new List<ReaderEntry>();

        /// <summary>
        ///  Register a content reader capable of reading the given file extensions.
        /// </summary>
        /// <remarks>
        ///  Include the dot prior to the extension name. (eg ".txt" instead of "txt").
        /// </remarks>
        /// <typeparam name="T">Asset class type.</typeparam>
        /// <param name="readerType">Content reader class type.</param>
        /// <param name="fileExtensions">Supported file extensions.</param>
        public void Add<T>(Type readerType, IEnumerable<string> fileExtensions)
        {
            Add(typeof(T), readerType, fileExtensions);
        }

        /// <summary>
        ///  Register a content reader capable of reading the given file extension.
        /// </summary>
        /// <remarks>
        ///  Include the dot prior to the extension name. (eg ".txt" instead of "txt").
        /// </remarks>
        /// <param name="contentType">Asset class type.</param>
        /// <param name="readerType">Content reader class type.</param>
        /// <param name="fileExtensions">Supported file extensions.</param>
        public void Add(Type contentType, Type readerType, IEnumerable<string> fileExtensions)
        {
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            if (readerType == null)
            {
                throw new ArgumentNullException(nameof(readerType));
            }

            if (fileExtensions == null)
            {
                throw new ArgumentNullException(nameof(fileExtensions));
            }
            else if (fileExtensions.Count() == 0)
            {
                throw new ArgumentException("Must have at least one file extension", nameof(fileExtensions));
            }
            else
            {
                foreach (var extension in fileExtensions)
                {
                    if (string.IsNullOrWhiteSpace(extension))
                    {
                        throw new ArgumentException(
                            "Extension in fileExtensions is null/empty",
                            nameof(fileExtensions));
                    }
                }
            }

            ContentReaders.Add(new ReaderEntry()
            {
                Extensions = fileExtensions.ToArray(),
                ContentType = contentType,
                ReaderType = readerType
            });
        }

        /// <inheritdoc />
        public IContentReader<T> GetContentReaderFor<T>(string fileExtension)
        {
            IContentReader<T> reader = null;

            if (!TryGetContentReaderFor(fileExtension, ref reader))
            {
                throw new ContentReaderNotFoundException(fileExtension);
            }

            return reader;
        }

        /// <inheritdoc />
        public bool TryGetContentReaderFor<T>(string fileExtension, ref IContentReader<T> reader)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                throw new ArgumentNullException(nameof(fileExtension));
            }

            // Find the right content reader type for this asset, instantiate and return.
            var readerType = FindContentReaderTypeFor<T>(fileExtension);

            if (readerType != null)
            {
                reader = Activator.CreateInstance(readerType) as IContentReader<T>;
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Find the content reader type capable of loading this asset.
        /// </summary>
        /// <param name="fileExtension">Content file extension.</param>
        /// <returns>Content reader type capable of loading this asset.</returns>
        private Type FindContentReaderTypeFor<T>(string fileExtension)
        {
            foreach (var entry in ContentReaders)
            {
                foreach (var anExtension in entry.Extensions)
                {
                    if (anExtension == fileExtension && entry.ContentType == typeof(T))
                    {
                        if (entry.ContentType == null)
                        {
                            throw new ContentReaderConfigurationException(
                                fileExtension,
                                contentReaderType: entry.ReaderType);
                        }

                        return entry.ReaderType;
                    }
                }
            }

            // Failed to locate matching content reader.
            return null;
        }

        /// <summary>
        ///  Content reader entry.
        /// </summary>
        private class ReaderEntry
        {
            /// <summary>
            ///  Resultant (content) object type.
            /// </summary>
            public Type ContentType;

            /// <summary>
            ///  Converter object type.
            /// </summary>
            public Type ReaderType;

            /// <summary>
            ///  File extensions.
            /// </summary>
            public string[] Extensions;
        }
    }
}

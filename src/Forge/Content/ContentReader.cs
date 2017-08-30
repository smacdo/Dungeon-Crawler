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
using System.Threading.Tasks;

namespace Forge.Content
{
    /// <summary>
    ///  Interface for a ContentReader capapble of loading objects of a declared type from an input stream.
    /// </summary>
    public interface IContentReader<TContent>
    {
        /// <summary>
        ///  Read a serialized asset from an input stream and return it as an object.
        /// </summary>
        /// <param name="inputStream">Stream to read serialized asset data from.</param>
        /// <param name="assetPath">Relative path used to create the inputStream.</param>
        /// <param name="content">Content manager for loading dependent data.</param>
        /// <returns>New object created from the input stream.</returns>
        Task<TContent> Read(
            Stream inputStream,
            string assetPath,
            IContentManager content);
    }

    /// <summary>
    ///  Content reader attribute describes a class that is capable of reading one or more file formats and returning
    ///  objects of a declared type. Forge uses this attribute to automatically discover content readers and their
    ///  configuration without hard coding configuration rules.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, Inherited = true )]
    public class ContentReaderAttribute : Attribute
    {
        /// <summary>
        ///  Get the type of object that this content reader produces.
        /// </summary>
        public Type ContentType { get; private set; }

        /// <summary>
        ///  Get the list of file extensions that this content reader supports reading from.
        /// </summary>
        public string[] FileExtensions { get; private set; }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="contentType">Type of object that this content reader produces.</param>
        /// <param name="fileExtension">File extension that this content reader supports.</param>
        public ContentReaderAttribute(Type contentType, string fileExtension)
        {
            ContentType = contentType;
            FileExtensions = new string[] { fileExtension };
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="contentType">Type of object that this content reader produces.</param>
        /// <param name="fileExtensions">A list of file extensions that this content reader supports.</param>
        public ContentReaderAttribute(Type contentType, string[] fileExtensions)
        {
            ContentType = contentType;
            FileExtensions = fileExtensions;
        }
    }
}

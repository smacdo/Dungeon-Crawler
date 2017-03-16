﻿/*
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

namespace Scott.Forge.Engine.Content
{
    /// <summary>
    ///  Represents an exception while loading game data.
    /// </summary>
    public class ContentManagerException : ForgeException
    {
        /// <summary>
        ///  Create a new GameDataException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ContentManagerException( string message )
            : base( message )
        {
            // Empty
        }

        /// <summary>
        ///  Create a new GameDataException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="filename">Name of the content file causing the exception.</param>
        public ContentManagerException( string message, string filename )
            : base ( "Error reading {0}: {1}".With( filename, message ) )
        {
            // Empty
        }

        /// <summary>
        ///  Create a new GameDataException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="filename">Name of the content file causing the exception.</param>
        public ContentManagerException( string message, string filename, Exception inner )
            : base( "Error reading {0}: {1}".With( filename, message ), inner )
        {
            // Empty
        }
    }

    /// <summary>
    ///  Exception when content manager tries to load an asset but cannot find a content reader.
    /// </summary>
    public class ContentReaderMissingException : ContentManagerException
    {
        public ContentReaderMissingException(string fileName)
            : base("Could not find a content reader for this file", fileName)
        {
        }
    }

    /// <summary>
    ///  Exception when content manager tries to load an asset but cannot find a content reader.
    /// </summary>
    public class MissingAssetException : ContentManagerException
    {
        public MissingAssetException(string assetName)
            : base("Could not locate the asset in the game content directory", assetName)
        {
        }
    }
    
    /// <summary>
    ///  Exception when content manager cannot find the root content folder.
    /// </summary>
    public class ContentDirectoryMissingException : ContentManagerException
    {
        public ContentDirectoryMissingException(string contentDirectoryPath)
            : base("Could not locate root content directory", contentDirectoryPath)
        {
        }
    }
}

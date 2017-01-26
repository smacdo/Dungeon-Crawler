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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Scott.Forge.Engine.Content
{
    /// <summary>
    ///  Abstract base class for content readers.
    /// </summary>
    internal abstract class ContentReader<T>
    {
        public ContentReader()
        {
            // empty
        }

        public abstract T Read( Stream input,
                                string assetName,
                                string contentDir,
                                ForgeContentManager content );
    }

    /// <summary>
    ///  Attribute that describes what type of content a given content reader class reads.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, Inherited = true )]
    public class ContentReaderAttribute : Attribute
    {
        public Type ContentType { get; private set; }
        public string Extension { get; private set; }

        public ContentReaderAttribute( Type contentType, string fileExtension )
        {
            ContentType = contentType;
            Extension = fileExtension;
        }
    }
}

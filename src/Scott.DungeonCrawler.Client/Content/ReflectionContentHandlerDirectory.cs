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
using Forge;
using Forge.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.DungeonCrawler.Client.Content
{
    /// <summary>
    ///  Desktop .NET content handler directory that discovers handlers using attribute based reflection.
    /// </summary>
    /// <remarks>
    ///  When an instance of this class is created the constructor will query the running assembly for a list
    ///  of classes that have the content reader attribute applied. All of these classes will be placed into
    ///  the newly constructed instance for querying.
    ///  
    ///  TODO: Rewrite this to support UWP apps and move it back into the Engine project.
    /// </remarks>
    public class ReflectionContentHandlerDirectory : ContentHandlerDirectory
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public ReflectionContentHandlerDirectory()
        {
            DiscoverAllReadersWithAttribute();
        }

        /// <summary>
        ///  Searches the currently loaded assemblies for asset content readers, and then records
        ///  them into a list of content readers for later use in asset loading.
        /// </summary>
        private void DiscoverAllReadersWithAttribute()
        {
            // Search for all classes with ContentReaderAttribute.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var attribute = typeof(ContentReaderAttribute);

            foreach (var readerType in AttributeUtils.GetTypesWithAttribute<ContentReaderAttribute>(assemblies))
            {
                var allReaderAttributes = readerType.GetCustomAttributes(typeof(ContentReaderAttribute), true);

                foreach (var untypedReaderAttribute in allReaderAttributes)
                {
                    var typedAttribute = untypedReaderAttribute as ContentReaderAttribute;

                    if (typedAttribute == null)
                    {
                        throw new InvalidOperationException("Content reader is missing ContentReader attribute");
                    }

                    Add(
                        contentType: typedAttribute.ContentType,
                        readerType: readerType,
                        fileExtensions: typedAttribute.FileExtensions);
                }
            }
        }
    }
}

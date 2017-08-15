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

namespace Scott.Forge.Content
{
    public class ContentManagerException : ForgeException
    {
        public ContentManagerException()
        {
        }

        public ContentManagerException(string message)
            : base(message)
        {
        }

        public ContentManagerException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class InvalidAssetNameException : ForgeException
    {
        public InvalidAssetNameException(string assetName)
            : base(string.Format("Asset '{0}' has a missing or invalid name", assetName))
        {
            AssetName = assetName;
        }

        public string AssetName { get; private set; }
    }

    public class AssetNotFoundException : ForgeException
    {
        public AssetNotFoundException(string assetName)
            : base(string.Format("Could not find asset '{0}' when loading content", assetName))
        {
            AssetName = assetName;
        }

        public string AssetName { get; private set; }
    }

    public class ContentReaderConfigurationException : ForgeException
    {
        public ContentReaderConfigurationException(
            string assetName,
            Type contentReaderType)
            : base(string.Format(
                "Could not load asset '{0}' because selected reader '{1}' is misconfigured",
                assetName,
                contentReaderType?.ToString() ?? "null"))
        {
            AssetName = assetName;
            ContentReaderType = contentReaderType;
        }

        public string AssetName { get; private set; }
        public Type ExpectedOutputType { get; private set; }
        public Type ActualOutputType { get; private set; }
        public Type ContentReaderType { get; private set; }
    }
}

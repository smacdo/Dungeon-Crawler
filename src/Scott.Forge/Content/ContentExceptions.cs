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

namespace Scott.Forge.Content
{
    /// <summary>
    ///  Content manager base exception.
    /// </summary>
    public class ContentManagerException : ForgeException
    {
        public ContentManagerException(string assetPath)
            : base($"Exception when loading asset '{assetPath}'")
        {
            AssetPath = assetPath;
        }

        public ContentManagerException(string assetPath, string message)
            : base(message)
        {
            AssetPath = assetPath;
        }

        public ContentManagerException(string assetPath, string message, Exception innerException)
            : base(message, innerException)
        {
            AssetPath = assetPath;
        }

        public string AssetPath { get; }
    }

    /// <summary>
    ///  Asset name is malformed.
    /// </summary>
    public class InvalidAssetNameException : ContentManagerException
    {
        public InvalidAssetNameException(string assetPath)
            : base(assetPath, $"Asset '{assetPath ?? string.Empty}' has a missing or invalid name")
        {
        }
    }

    /// <summary>
    ///  Asset could not be found.
    /// </summary>
    public class AssetNotFoundException : ContentManagerException
    {
        public AssetNotFoundException(string assetPath)
            : base(assetPath, $"Could not find asset '{assetPath ?? string.Empty}' when loading content")
        {
        }
    }

    /// <summary>
    ///  Actual asset type does not match expected asset type.
    /// </summary>
    public class AssetWrongTypeException : ContentManagerException
    {
        public AssetWrongTypeException(string assetPath, Type expectedType, Type actualType)
            : base(assetPath, FormatMessage(assetPath, expectedType, actualType))
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        public Type ExpectedType { get; }
        public Type ActualType { get; }

        private static string FormatMessage(string assetPath, Type expectedType, Type actualType)
        {
            return string.Format($"Expected asset '{0}' to be '{1}' but was '{2}'",
                  assetPath ?? string.Empty,
                  expectedType?.ToString() ?? string.Empty,
                  actualType?.ToString() ?? string.Empty);
        }
    }

    /// <summary>
    ///  Content reader entry has incorrect values.
    /// </summary>
    public class ContentReaderConfigurationException : ContentManagerException
    {
        public ContentReaderConfigurationException(string assetPath, Type contentReaderType)
            : base(assetPath, FormatMessage(assetPath, contentReaderType))
        {
            ContentReaderType = contentReaderType;
        }
        
        public Type ContentReaderType { get; }

        private static string FormatMessage(string assetPath, Type contentReaderType)
        {
            return string.Format(
                "Could not load asset '{0}' because selected reader '{1}' is misconfigured",
                assetPath ?? string.Empty,
                contentReaderType?.ToString() ?? string.Empty);
        }
    }

    /// <summary>
    ///  Cannot load XNB files because the content manager could not find any XNA content managers to use.
    /// </summary>
    public class XnbLoadingSupportMissingException : ContentManagerException
    {
        public XnbLoadingSupportMissingException(string assetPath)
            : base(assetPath, FormatMessage(assetPath))
        {
        }

        private static string FormatMessage(string assetPath)
        {
            return string.Format(
                "Could not load '{0}' because content manager does not have a XNA content manager",
                assetPath ?? string.Empty);
        }
    }
}

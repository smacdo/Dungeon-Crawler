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
    ///  Abstract interface for Forge content managers.
    /// </summary>
    public interface IContentManager : IDisposable
    {
        /// <summary>
        ///  Get the service provider associated with this content manager.
        /// </summary>
        /// <remarks>
        ///  This property is provided for compatibility with XNA content loaders. It should not be used by custom
        ///  Forge content loaders because it will eventually be removed.
        /// </remarks>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        ///  Load an asset.
        /// </summary>
        /// <typeparam name="T">Runtime content type to load.</typeparam>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>An instance of the loaded asset.</returns>
        TContent Load<TContent>(string assetName);

        /// <summary>
        ///  Disposes all assets loaded by this content manager.
        /// </summary>
        void Unload();
    }
}

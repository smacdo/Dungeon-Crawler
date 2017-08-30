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
using System.Threading.Tasks;

namespace Forge.Content
{
    /// <summary>
    ///  Content managers are an interface for loading and unloading game content objects.
    /// </summary>
    public interface IContentManager : IDisposable
    {
        /// <summary>
        ///  Get or set legacy XNA content manager. DEPRECATED!
        /// </summary>
        /// <remarks>
        ///  Avoid using this property if possible because XNA support will eventually be removed from the system.
        /// </remarks>
        Microsoft.Xna.Framework.Content.ContentManager XnaContentManager { get; set; }

        /// <summary>
        ///  Check if an asset is cached by the content manager.
        /// </summary>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns></returns>
        bool IsLoaded(string assetPath);

        /// <summary>
        ///  Load an asset by path name.
        /// </summary>
        /// <remarks>
        ///  All content items should be addressed by their relative path from the content directory. For example,
        ///  if content items are located at "C:\Game\Content\Images\Cat.png" then the path is "Images\Cat.png".
        /// </remarks>
        /// <typeparam name="TContent">Runtime asset class type.</typeparam>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>An instance of the loaded asset.</returns>
        Task<TContent> Load<TContent>(string assetPath);

        /// <summary>
        ///  Unload all assets loaded by this content manager.
        /// </summary>
        void Unload();

        /// <summary>
        ///  Unload an asset by path name.
        /// </summary>
        /// <remarks>
        ///  Unload will remove an asset from the asset cache, and if the object implements IDisposable it will
        ///  dispose of this object. Any live reference to the object will remain but be will become usuable.
        /// </remarks>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>True if the object was unloaded, false if it was never loaded.</returns>
        bool Unload(string assetPath);
    }
}

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
    /// <summary>
    ///  A caching content manager that proxies load/unload requests to a backing content store, and caches the results
    ///  for faster access.
    /// </summary>
    /// <remarks>
    ///  TODO: Use weak references to allow items to expire from the cache (if desired).
    /// </remarks>
    public class CachedContentManager : IContentManager
    {
        /// <summary>
        ///  True if the instance has been disposed, false otherwise.
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="actualContentManager">Backing content manager.</param>
        public CachedContentManager(IContentManager actualContentManager)
        {
            if (actualContentManager == null)
            {
                throw new ArgumentNullException(nameof(actualContentManager));
            }

            ActualContentManager = actualContentManager;
        }
        
        /// <summary>
        ///  Get or set the content manager that is used by the cache to load assets.
        /// </summary>
        public IContentManager ActualContentManager { get; set; }

        /// <summary>
        ///  Table of loaded assets indexed by asset name.
        /// </summary>
        private Dictionary<string, object> mCache = new Dictionary<string, object>();

        /// <summary>
        ///  Load an asset.
        /// </summary>
        /// <typeparam name="T">Runtime content type to load.</typeparam>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>An instance of the loaded asset.</returns>
        public TContent Load<TContent>(string assetName)
        {
            // Check the cache for the asset. If it exists then simply return another instance of the asset, otherwise
            // use the backing content manager to load it.
            object cachedObject = null;

            if (mCache.TryGetValue(assetName, out cachedObject))
            {
                return (TContent) cachedObject;
            }
            else
            {
                // Load the asset using the backing store.
                var asset = ActualContentManager.Load<TContent>(assetName);

                // Add the loaded item to the cache.
                mCache.Add(assetName, asset);
                return asset;
            }
        }

        /// <summary>
        ///  Dispose all content loaded by this content manager.
        /// </summary>
        public void Unload()
        {
            // TODO: Verify this works for unloading.
            foreach (var item in mCache)
            {
                var disposable = item.Value as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            mCache.Clear();
            ActualContentManager.Unload();
        }

        /// <summary>
        ///  Dispose of cached content.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Dispose of cached content.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                Unload();

                if (ActualContentManager != null)
                {
                    ActualContentManager.Dispose();
                }

                mDisposed = true;
            }
        }
    }
}

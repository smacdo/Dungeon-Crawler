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
    ///  Special shim that allows XNA to use the Forge content management system.
    /// </summary>
    public class XnaProxyContentManager : Microsoft.Xna.Framework.Content.ContentManager
    {
        /// <summary>
        ///  True if the instance has been disposed, false otherwise.
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="provider">The service provider that should be used to locate services.</param>
        /// <param name="rootDirectory">The directory path that contains content.</param>
        public XnaProxyContentManager(
            IServiceProvider provider,
            string rootDirectory,
            IContentManager actualContentManager)
            : base(provider, rootDirectory)
        {
            if (rootDirectory == null)
            {
                throw new ArgumentNullException(nameof(rootDirectory));
            }

            if (actualContentManager == null)
            {
                throw new ArgumentNullException(nameof(actualContentManager));
            }

            ActualContentManager = actualContentManager;
        }

        /// <summary>
        ///  Get or set the content manager that is used by this shim.
        /// </summary>
        public IContentManager ActualContentManager { get; set; }

        /// <summary>
        ///  Load an asset.
        /// </summary>
        /// <typeparam name="T">Runtime content type to load.</typeparam>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>An instance of the loaded asset.</returns>
        public override T Load<T>(string assetName)
        {
            return ActualContentManager.Load<T>(assetName);
        }

        /// <summary>
        ///  Unload an asset.
        /// </summary>
        public override void Unload()
        {
            ActualContentManager.Unload();
        }

        /// <summary>
        ///  Manually unload content when the content manager is disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
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

            base.Dispose(disposing);
        }
    }
}

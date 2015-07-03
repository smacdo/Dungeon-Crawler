/*
 * Copyright 2012-2015 Scott MacDonald
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
using Microsoft.Xna.Framework.Graphics;

namespace Scott.Forge.Engine.Graphics
{
    /// <summary>
    ///  Indirect texture cache. Allows sprites (and other objects) to hold an integer handle to a texture to imrpove
    ///  de-coupling and unit testing. Needs more work, current version is brute force and unlikely to stand up to long
    ///  term use. And refactor work, etc. Just proof of concept for the moment.
    /// </summary>
    public class TextureCache
    {
        public Dictionary<int, Texture2D> Textures { get; private set; }
        private int mNextId = 1;

        public TextureCache()
        {
            Textures = new Dictionary<int, Texture2D>(512);
        }

        public int Add(Texture2D texture)
        {
            int id = 0;

            lock (Textures)
            {
                id = mNextId++;
                Textures.Add(id, texture);
            }

            return id;
        }

        public Texture2D FindById(int handle)
        {
            return Textures[handle];
        }
    }
}

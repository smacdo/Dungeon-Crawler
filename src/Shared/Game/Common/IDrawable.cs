﻿
/*
 * IDrawable.cs
 * Copyright 2013 Scott MacDonald
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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game
{
    /// <summary>
    ///  Represents an object that can draw itself onto the screen.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        ///  Get or set if this object will draw.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        ///  Request that the object draw itself using the given simulation time.
        /// </summary>
        /// <param name="gameTime">Simulation time.</param>
        void Draw( GameTime gameTime );
    }
}

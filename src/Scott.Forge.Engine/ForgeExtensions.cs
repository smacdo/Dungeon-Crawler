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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Scott.Forge.Engine
{
    /// <summary>
    ///  Extension methods for forge classes.
    /// </summary>
    public static class ForgeExtensions
    {
        public static Rectangle ToXnaRectangle(this RectF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public static Microsoft.Xna.Framework.Vector2 ToXnaVector2(this Vector2 v)
        {
            return new Microsoft.Xna.Framework.Vector2(v.X, v.Y);
        }
    }
}

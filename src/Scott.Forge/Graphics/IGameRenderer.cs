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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Forge.Spatial;
using Scott.Forge.Tilemaps;

namespace Scott.Forge.Graphics
{
    /// <summary>
    ///  2d game renderer interface.
    /// </summary>
    public interface IGameRenderer
    {
        /// <summary>
        ///  Draw a texture atlas slice centered on the given screen space point with a rotation.
        /// </summary>
        /// <param name="atlas">Texture atlas to use.</param>
        /// <param name="offset">Rectangle section of texture atlas to draw.</param>
        /// <param name="position">Center position in screen space to draw at.</param>
        /// <param name="rotation">Rotation (in radians) to rotate texture while drawing.</param>
        void Draw(Texture2D atlas, RectF offset, Vector2 position, float rotation = 0.0f);

        /// <summary>
        ///  Draw a colored line segment.
        /// </summary>
        /// <param name="startPosition">Screen space position to start drawing.</param>
        /// <param name="endPosition">Screen space position to stop drawing.</param>
        /// <param name="color">Optional line color.</param>
        /// <param name="width">Optional line width.</param>
        void DrawLine(
            Vector2 startPosition,
            Vector2 endPosition,
            Color? color,
            float? width);

        /// <summary>
        ///  Draw colored text using the given font and optional background color.
        /// </summary>
        /// <param name="text">Text string to draw.</param>
        /// <param name="position">Top left corner in screen space to start drawing at.</param>
        /// <param name="font">XNA sprite font class to use.</param>
        /// <param name="textColor">Optional color of the text.</param>
        /// <param name="backgroundColor">Optional background color.</param>
        void DrawText(
            string text,
            Vector2 position,
            SpriteFont font,
            Color? textColor = null,
            Color? backgroundColor = null);

        /// <summary>
        ///  Draw a rectangle with an optional border and background color.
        /// </summary>
        /// <param name="rect">Size and position of the rectangle in screen space.</param>
        /// <param name="borderColor">Optional border color.</param>
        /// <param name="borderSize">Optional border size.</param>
        /// <param name="fillColor">Optional fill color.</param>
        void DrawRectangle(
            RectF rect,
            Color? fillColor = null,
            Color? borderColor = null,
            float? borderSize = null);

        /// <summary>
        ///  Draw a 2d tilemap.
        /// </summary>
        /// <param name="camera">Camera view to draw from.</param>
        /// <param name="tilemap">Tilemap to draw.</param>
        void DrawTilemap(Camera camera, TileMap tilemap);

        /// <summary>
        ///  Call when starting to draw a rendering frame.
        /// </summary>
        void StartDrawing(bool clearScreen = false);

        /// <summary>
        ///  Call when finished drawing a rendering frame.
        /// </summary>
        void FinishDrawing();
    }
}

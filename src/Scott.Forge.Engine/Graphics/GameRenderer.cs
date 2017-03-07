﻿/*
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Scott.Forge.Engine.Graphics
{
    /// <summary>
    ///  Responsible for drawing 2d sprites.
    /// </summary>
    /// <remarks>
    ///  TODO: Replace SpriteFont with custom sprite font rendering (don't rely on XNA).
    /// </remarks>
    public class GameRenderer
    {
        private GraphicsDevice mGraphicsDevice;
        private SpriteBatch mSpriteBatch;

        /// <summary>
        ///  A simple 1x1 texture that can be arbitrarily colored and stretched. Perfect for little boxes
        /// </summary>
        private Texture2D mWhitePixel;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="graphics">Reference to active graphics device.</param>
        public GameRenderer(GraphicsDevice graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            mGraphicsDevice = graphics;

            mSpriteBatch = new SpriteBatch(mGraphicsDevice);

            mWhitePixel = new Texture2D(mGraphicsDevice, 1, 1);
            mWhitePixel.SetData(new[] { Color.White });
        }
        
        /// <summary>
        ///  Draw a texture atlas slice centered on the given screen space point with a rotation.
        /// </summary>
        /// <remarks>
        ///  TODO: Convert XNA Rectangle argument to either RectF or BoundingRect. Also convert Sprite to use this.
        /// </remarks>
        /// <param name="atlas">Texture atlas to use.</param>
        /// <param name="offset">Rectangle section of texture atlas to draw.</param>
        /// <param name="position">Center position in screen space to draw at.</param>
        /// <param name="rotation">Rotation (in radians) to rotate texture while drawing.</param>
        public void Draw(Texture2D atlas, Rectangle offset, Vector2 position, float rotation = 0.0f)
        {
            // Convert center origin to top left origin for XNA.
            var topLeft = ConvertCenterToTopLeft(position.X, position.Y, offset.Width, offset.Height);

            mSpriteBatch.Draw(
                    texture: atlas,
                    position: new Microsoft.Xna.Framework.Vector2(topLeft.X, topLeft.Y),
                    sourceRectangle: offset,
                    color: Color.White,
                    rotation: rotation,
                    origin: Microsoft.Xna.Framework.Vector2.Zero,
                    scale: Microsoft.Xna.Framework.Vector2.One,
                    effects: SpriteEffects.None,
                    layerDepth: 0.0f);
        }

        /// <summary>
        ///  Draw a colored line segment on the screen.
        /// </summary>
        /// <param name="startPosition">Point to start drawing in screen space.</param>
        /// <param name="endPosition">Point to stop drawing in screen space.</param>
        /// <param name="color">Optional line color (White by default).</param>
        /// <param name="width">Optional line width (1 by default).</param>
        public void DrawLine(
            Vector2 startPosition,
            Vector2 endPosition,
            Color? color,
            int? width)
        {
            float angle  = (float) Math.Atan2(endPosition.Y - startPosition.Y, endPosition.X - startPosition.X);
            float length = Vector2.Distance(startPosition, endPosition);

            mSpriteBatch.Draw(
                texture: mWhitePixel,
                position: new Microsoft.Xna.Framework.Vector2(startPosition.X, startPosition.Y),
                sourceRectangle: null,
                color: color ?? Color.White,
                rotation: angle,
                origin: Microsoft.Xna.Framework.Vector2.Zero,
                scale: new Microsoft.Xna.Framework.Vector2(length, width ?? 1),
                effects: SpriteEffects.None,
                layerDepth: 0.0f);
        }

        /// <summary>
        ///  Draw colored text using the given font and optional background color.
        /// </summary>
        /// <param name="text">Text string to draw.</param>
        /// <param name="position">Top left corner in screen space to start drawing at.</param>
        /// <param name="font">XNA sprite font class to use.</param>
        /// <param name="textColor">Optional color of the text (null for default white).</param>
        /// <param name="backgroundColor">Optional background color (null for none).</param>
        public void DrawText(
            string text,
            Vector2 position,
            SpriteFont font,
            Color? textColor = null,
            Color? backgroundColor = null)
        {
            // Check arguments.
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            // Draw a colored background rectangle behind the sprite text if requested.
            if (backgroundColor != null)
            {
                // Draw a colored rectangle that is slightly larger than the requested text.
                var size = font.MeasureString(text);
                var rect = new Rectangle(
                    (int)(position.X - 2.0f ),
                    (int)(position.Y - 2.0f ),
                    (int)(size.X + 4.0f ),
                    (int)(size.Y + 4.0f ) );

                mSpriteBatch.Draw(mWhitePixel, rect, backgroundColor.Value);
            }

            // Draw the text.
            mSpriteBatch.DrawString(
                font,
                text,
                new Microsoft.Xna.Framework.Vector2(position.X, position.Y),
                textColor ?? Color.White);
        }

        /// <summary>
        ///  Draw rectangle outline.
        /// </summary>
        /// <param name="rect">Size and position of the rectangle in screen space.</param>
        /// <param name="color">Optional color, white by default.</param>
        public void DrawFilledRectangle(RectF rect, Color? color)
        {
            mSpriteBatch.Draw(
                mWhitePixel,
                new Rectangle((int) rect.Left, (int) rect.Top, (int) rect.Width, (int) rect.Height),
                color ?? Color.White);
        }

        /// <summary>
        ///  Draw a rectangle.
        /// </summary>
        /// <param name="rect">size and position of the rectangle in screen space.</param>
        /// <param name="color">Optional color, white by default.</param>
        public void DrawRectangleBorder(RectF rect, Color? color)
        {
            color = color ?? Color.White;

            int left = (int) rect.Left;
            int right = (int) rect.Right;
            int top = (int) rect.Top;
            int bottom = (int) rect.Bottom;
            int width = (int) rect.Width;
            int height = (int) rect.Height;

            mSpriteBatch.Draw(mWhitePixel, new Rectangle(left, top, width, 1), color.Value);
            mSpriteBatch.Draw(mWhitePixel, new Rectangle(left, top, 1, height), color.Value);
            mSpriteBatch.Draw(mWhitePixel, new Rectangle(left, bottom, width, 1), color.Value);
            mSpriteBatch.Draw(mWhitePixel, new Rectangle(right, top, 1, height), color.Value);
        }

        /// <summary>
        ///  Convert a position centered in a rectangle to the top left corner.
        /// </summary>
        /// <param name="x">Center X coordinate.</param>
        /// <param name="y">Center Y coordinate.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <returns>The top left corner of the rectangle.</returns>
        public static Vector2 ConvertCenterToTopLeft(float x, float y, float width, float height)
        {
            return new Vector2(
                x - width * 0.5f,
                y - height * 0.5f);
        }

        /// <summary>
        ///  Call when starting to draw a rendering frame.
        /// </summary>
        public void StartDrawing(bool clearScreen = false)
        {
            if (clearScreen)
            {
                mGraphicsDevice.Clear(Color.CornflowerBlue);
            }
            
            mSpriteBatch.Begin();
        }

        /// <summary>
        ///  Call when finished drawing a rendering frame.
        /// </summary>
        public void FinishDrawing()
        {
            mSpriteBatch.End();
        }
    }
}
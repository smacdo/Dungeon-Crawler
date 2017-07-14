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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Scott.Forge.Graphics;
using Scott.Forge.Spatial;
using Scott.Forge.Tilemaps;

namespace Scott.Forge.Graphics
{
    /// <summary>
    ///  Responsible for drawing 2d primitives for a game.
    /// </summary>
    /// <remarks>
    ///  TODO: Replace SpriteFont with custom sprite font rendering (don't rely on XNA).
    /// </remarks>
    public class GameRenderer : IGameRenderer
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
        /// <param name="atlas">Texture atlas to use.</param>
        /// <param name="atlasRect">Rectangle section of texture atlas to draw.</param>
        /// <param name="screenPosition">Center position in screen space to draw at.</param>
        /// <param name="rotation">Rotation (in radians) to rotate texture while drawing.</param>
        public void Draw(Texture2D atlas, RectF atlasRect, Vector2 screenPosition, float rotation = 0.0f)
        {
            Draw(
                atlas,
                new Rectangle(
                    x: (int)atlasRect.Left,
                    y: (int)atlasRect.Top,
                    width: (int)atlasRect.Width,
                    height: (int)atlasRect.Height),
                screenPosition,
                rotation);
        }

        /// <summary>
        ///  Draw a texture atlas slice centered on the given screen space point with a rotation.
        /// </summary>
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
        ///  Draw a colored line segment.
        /// </summary>
        /// <param name="startPosition">Screen space position to start drawing.</param>
        /// <param name="endPosition">Screen space position to stop drawing.</param>
        /// <param name="color">Optional line color.</param>
        /// <param name="width">Optional line width.</param>
        public void DrawLine(
            Vector2 startPosition,
            Vector2 endPosition,
            Color? color,
            float? width)
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
        /// <param name="textColor">Optional color of the text.</param>
        /// <param name="backgroundColor">Optional background color.</param>
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
            if (backgroundColor.HasValue)
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
        ///  Draw a rectangle with an optional border and background color.
        /// </summary>
        /// <param name="rect">Size and position of the rectangle in screen space.</param>
        /// <param name="borderColor">Optional border color.</param>
        /// <param name="borderSize">Optional border size.</param>
        /// <param name="fillColor">Optional fill color.</param>
        public void DrawRectangle(
            RectF rect,
            Color? fillColor = null,
            Color? borderColor = null,
            float? borderSize = null)
        {
            // Draw rectangle background first.
            if (fillColor.HasValue)
            {
                mSpriteBatch.Draw(
                    mWhitePixel,
                    new Rectangle((int) rect.Left, (int) rect.Top, (int) rect.Width, (int) rect.Height),
                    fillColor.Value);
            }

            // Draw rectangle border if requested.
            if (borderColor.HasValue || borderSize.HasValue)
            {
                var color = borderColor ?? Color.White;
                var size = borderSize ?? 1.0f;

                int left = (int) rect.Left;
                int right = (int) rect.Right;
                int top = (int) rect.Top;
                int bottom = (int) rect.Bottom;
                int width = (int) rect.Width;
                int height = (int) rect.Height;

                DrawLine(new Vector2(left, top), new Vector2(left + width, top), color, size);
                DrawLine(new Vector2(left, top), new Vector2(left, top + height), color, size);
                DrawLine(new Vector2(left, top + height), new Vector2(left + width, top + height), color, size);
                DrawLine(new Vector2(left + width, top + height), new Vector2(left + width, top), color, size);
            }
        }

        /// <summary>
        ///  Draw portion of tilemap that is visible.
        /// </summary>
        /// <param name="camera">Rendering camera.</param>
        /// <param name="tilemap">Tilemap to draw.</param>
        public void DrawTilemap(Camera camera, TileMap tilemap)
        {
            var topLeftTile = camera.LeftmostVisbileTile(tilemap);
            var bottomRightTile = camera.GetBottomRightmostVisibleTile(tilemap);

            for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
            {
                for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
                {
                    // Get the tile definition for the tile at this (x, y) position.
                    var tile = tilemap.TileSet[tilemap.Grid[x, y].Type];

                    // Calculate screen space position for the tile.
                    var tilePosition = tilemap.GetWorldPositionForTile(new Point2(x, y));
                    var screenSpacePosition = camera.WorldToScreen(tilePosition);

                    // Draw the tile using the tile set's texture atlas.
                    // TODO: Add fancy effects for thigns like water.
                    Draw(
                        tilemap.TileSet.Atlas,
                        new RectF(tile.AtlasX, tile.AtlasY, tilemap.TileWidth, tilemap.TileHeight),
                        screenSpacePosition);
                }
            }
        }

        /// <summary>
        ///  Draw collision map for portion of tilemap that is visible.
        /// </summary>
        /// <param name="camera">Rendering camera.</param>
        /// <param name="tilemap">Tilemap to draw.</param>
        public void DrawCollisionMap(Camera camera, TileMap tilemap)
        {
            var topLeftTile = camera.LeftmostVisbileTile(tilemap);
            var bottomRightTile = camera.GetBottomRightmostVisibleTile(tilemap);

            var tileExtent = new Vector2(tilemap.TileWidth / 2.0f, tilemap.TileHeight / 2.0f);

            for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
            {
                for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
                {
                    // Get the tile definition for the tile at this (x, y) position.
                    var tile = tilemap.TileSet[tilemap.Grid[x, y].Type];

                    // Calculate screen space position for the tile.
                    var tilePosition = tilemap.GetWorldPositionForTile(new Point2(x, y));
                    var screenSpacePosition = camera.WorldToScreen(tilePosition);

                    // Draw collision information for tile.
                    var boundRect = new RectF(
                        screenSpacePosition - tileExtent,
                        screenSpacePosition + tileExtent);

                    if (tilemap.Grid[x, y].Collision > 0)
                    {
                        DrawRectangle(
                            boundRect,
                            Color.Red);
                    }
                    else
                    {
                        DrawRectangle(
                            boundRect,
                            null,
                            Color.White,
                            1);
                    }
                }
            }
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

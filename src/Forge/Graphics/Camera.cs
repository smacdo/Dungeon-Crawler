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
using Forge.Spatial;
using Forge.Tilemaps;

namespace Forge.Graphics
{
    /// <summary>
    ///  General purpose 2d camera.
    /// </summary>
    /// <remarks>
    ///  Camera space is defined as a rectangle with the origin (0, 0) in the center of the viewport rectangle.
    ///  
    ///  The camera class uses several coordinate spaces to do its work. The camera's coordinate space is a rectangle
    ///  formed by the camera's viewport with the origin being the center of the camera. Screen space is treated as
    ///  the XNA coordinate system where the origin is the upper left corner of the window, and the size is the size
    ///  of the rendering window.
    /// 
    ///  TODO: Support camera rotation which will allow camera shakes.
    ///  TODO: Align the camera with the Transform class.
    /// </remarks>
    public class Camera
    {
        private float mHalfViewportWidth = 0.0f;
        private float mHalfViewportHeight = 0.0f;

        /// <summary>
        ///  Camera constructor.
        /// </summary>
        public Camera()
            : this(SizeF.Empty, Vector2.Zero)
        {
        }

        /// <summary>
        ///  Camera constructor.
        /// </summary>
        /// <param name="viewport">Viewport size in pixel units.</param>
        public Camera(SizeF viewport)
            : this(viewport, Vector2.Zero)
        {
        }

        /// <summary>
        ///  Camera constructor.
        /// </summary>
        /// <param name="viewport">Viewport size in pixel units.</param>
        /// <param name="positionWorldSpace">Position of camera center in pixel units in world space.</param>
        public Camera(SizeF viewport, Vector2 positionWorldSpace)
        {
            Position = positionWorldSpace;
            Viewport = viewport;
        }
        
        /// <summary>
        ///  Get or set the camera position in world space.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///  Get or set the camera viewport size.
        /// </summary>
        public SizeF Viewport
        {
            get { return new SizeF(mHalfViewportWidth * 2.0f, mHalfViewportHeight * 2.0f); }
            set
            {
                mHalfViewportWidth = value.Width / 2.0f;
                mHalfViewportHeight = value.Height / 2.0f;
            }
        }

        /// <summary>
        ///  Transform a vector in world space into screen space.
        /// </summary>
        /// <remarks>
        ///  Screen space is the XNA coordinate system with the origin at the top left of the window and a width
        ///  and height matching the window.
        /// </remarks>
        /// <param name="worldSpaceVector">World space vector.</param>
        /// <returns>Same vector but in screen space.</returns>
        public Vector2 WorldToScreen(Vector2 worldSpaceVector)
        {
            // The camera center is (0, 0) but the screen space origin (0, 0) is the top left of the screen.
            // 1. Translate the point to camera space by adding the inverse of the camera world position.
            // 2. Translate the result to screen space by adding half the viewport size.
            // 
            // Also need to truncate floating point values and do math using ints to prevent screen flickering
            // artifacts as the camera scrolls.
            return new Vector2(
                (int) worldSpaceVector.X - (int) Position.X + (int) mHalfViewportWidth,
                (int) worldSpaceVector.Y - (int) Position.Y + (int) mHalfViewportHeight);
        }

        /// <summary>
        ///  Transform a rectangle in world space into screen space.
        /// </summary>
        /// <remarks>
        ///  Screen space is the XNA coordinate system with the origin at the top left of the window and a width
        ///  and height matching the window.
        /// </remarks>
        /// <returns>Same rectangle but in screen space.</returns>
        public RectF WorldToScreen(RectF worldSpaceRect)
        {
            return new RectF(
                topLeft: WorldToScreen(worldSpaceRect.TopLeft),
                size: worldSpaceRect.Size);
        }

        /// <summary>
        ///  Transform a bounding rectangle in world space into screen space.
        /// </summary>
        /// <remarks>
        ///  Screen space is the XNA coordinate system with the origin at the top left of the window and a width
        ///  and height matching the window.
        /// </remarks>
        /// <returns>Same bounding rectangle but in screen space.</returns>
        public BoundingRect WorldToScreen(BoundingRect worldSpaceRect)
        {
            return new BoundingRect(
                center: WorldToScreen(worldSpaceRect.Center),
                extentSize: worldSpaceRect.ExtentSize);
        }

        /// <summary>
        ///  Transform a vector in camera space into world space.
        /// </summary>
        /// <param name="screenSpaceVector">Screen space vector.</param>
        /// <returns>Same vector but in world space.</returns>
        public Vector2 ScreenToWorld(Vector2 screenSpaceVector)
        {
            return screenSpaceVector + Position - new Vector2(mHalfViewportWidth, mHalfViewportHeight);
        }

        /// <summary>
        ///  Transform a rectangle in screen space into world space.
        /// </summary>
        /// <param name="screenSpaceRect"></param>
        /// <returns>Same rect but in screen space.</returns>
        public RectF ScreenToWorld(RectF screenSpaceRect)
        {
            return new RectF(
                ScreenToWorld(screenSpaceRect.TopLeft),
                ScreenToWorld(screenSpaceRect.BottomRight));
        }

        /// <summary>
        ///  Translate the camera center by the given amount.
        /// </summary>
        /// <param name="translation">Amount to translate the camera.</param>
        public void Translate(Vector2 distance)
        {
            Position += distance;
        }

        /// <summary>
        ///  Update camera.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // Do nothing.
        }

        /// <summary>
        ///  Get the index of the top left most tile that is visible to the camera.
        /// </summary>
        /// <param name="tilemap">Active tilemap.</param>
        /// <returns>The top left most visible tile.</returns>
        public Point2 LeftmostVisbileTile(TileMap tilemap)
        {
            var halfWidth = tilemap.TileWidth / 2.0f;
            var halfHeight = tilemap.TileHeight / 2.0f;

            var worldPosition = new Vector2(-halfWidth, -halfHeight);
            worldPosition = ScreenToWorld(worldPosition);

            return GetTileAtScreenCoordinate(worldPosition, tilemap);
        }

        /// <summary>
        ///  Get the index of the bottom right most tile that is visible to the camera.
        /// </summary>
        /// <param name="tilemap">Active tilemap.</param>
        /// <returns>Bottom right most visible tile.</returns>
        public Point2 GetBottomRightmostVisibleTile(TileMap tilemap)
        {
            var halfWidth = tilemap.TileWidth / 2.0f;
            var halfHeight = tilemap.TileHeight / 2.0f;

            var worldPosition = new Vector2(Viewport.Width + halfWidth, Viewport.Height + halfHeight);
            worldPosition = ScreenToWorld(worldPosition);

            return GetTileAtScreenCoordinate(worldPosition, tilemap);
        }

        /// <summary>
        ///  Get the index of a tile at the given screen space point.
        /// </summary>
        /// <param name="screenSpacePoint">Point in screen space.</param>
        /// <param name="tilemap">Active tilemap.</param>
        /// <returns>Tile index at given screen space point.</returns>
        public Point2 GetTileAtScreenCoordinate(Vector2 screenSpacePoint, TileMap tilemap)
        {
            return new Point2(
                MathHelper.Clamp((int)(screenSpacePoint.X / tilemap.TileWidth), 0, tilemap.Cols - 1),
                MathHelper.Clamp((int)(screenSpacePoint.Y / tilemap.TileHeight), 0, tilemap.Rows - 1));
        }

        /// <summary>
        ///  Get the world position for a tile point.
        /// </summary>
        /// <remarks>
        ///  The tile origin is in the center instead of the top left corner.
        /// </remarks>
        /// <param name="tilePoint">X/Y (column row) of the tile.</param>
        /// <returns>Tile center in world coordinate space.</returns>
        public Vector2 GetWorldPositionForTile(Point2 tilePoint, Tilemaps.TileSet tileset)
        {
            var tileWidth = tileset.TileWidth;
            var tileHeight = tileset.TileHeight;

            return new Vector2(
                tileWidth / 2 + tilePoint.X * tileWidth,
                tileHeight / 2 + tilePoint.Y * tileHeight);
        }
    }
}

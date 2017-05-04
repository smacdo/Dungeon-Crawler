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

namespace Scott.Forge.Graphics
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
        /// <param name="viewport">Viewport size in pixel units.</param>
        public Camera(SizeF viewport)
            : this(viewport, Vector2.Zero)
        {
        }

        /// <summary>
        ///  Camera constructor.
        /// </summary>
        /// <param name="viewport">Viewport size in pixel units.</param>
        /// <param name="centerInWorldSpace">Center of camera in pixel units in world space.</param>
        public Camera(SizeF viewport, Vector2 centerInWorldSpace)
        {
            CenterInWorldSpace = centerInWorldSpace;
            Viewport = viewport;
        }
        
        /// <summary>
        ///  Get or set the camera center position in world space.
        /// </summary>
        public Vector2 CenterInWorldSpace { get; set; }

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
            return worldSpaceVector - CenterInWorldSpace + new Vector2(mHalfViewportWidth, mHalfViewportHeight);
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
                worldSpaceRect.TopLeft - CenterInWorldSpace + new Vector2(mHalfViewportWidth, mHalfViewportHeight),
                worldSpaceRect.Size);
        }

        /// <summary>
        ///  Transform a vector in camera space into world space.
        /// </summary>
        /// <param name="screenSpaceVector">Screen space vector.</param>
        /// <returns>Same vector but in world space.</returns>
        public Vector2 ScreenToWorld(Vector2 screenSpaceVector)
        {
            return screenSpaceVector + CenterInWorldSpace - new Vector2(mHalfViewportWidth, mHalfViewportHeight);
        }

        /// <summary>
        ///  Translate the camera center by the given amount.
        /// </summary>
        /// <param name="translation">Amount to translate the camera.</param>
        public void Translate(Vector2 distance)
        {
            CenterInWorldSpace += distance;
        }

        /// <summary>
        ///  Update camera.
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <param name="deltaSeconds"></param>
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}

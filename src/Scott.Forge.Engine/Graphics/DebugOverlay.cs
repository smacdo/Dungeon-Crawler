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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Scott.Forge.Engine.Graphics
{
    /// <summary>
    /// Assists in drawing debug objects. Makes debugging stuff MUCH easier.
    /// </summary>
    public interface IDebugOverlay
    {
        /// <summary>
        /// Unload the debug renderer
        /// </summary>
        void Unload();

        /// <summary>
        ///  Draw a point on the screen.
        /// </summary>
        /// <param name="point">Center of the point in screen space.</param>
        /// <param name="color">Color of the point, optional.</param>
        /// <param name="sizeIn">Size of the point, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawPoint(
            Vector2 point,
            Color? color = null,
            float? sizeIn = null,
            TimeSpan? timeToLive = null);

        /// <summary>
        ///  Draws a rectangle border on the screen.
        /// </summary>
        /// <param name="dimensions">Rectangle dimensions in screen space.</param>
        /// <param name="borderColor">Color of the rectangle border. (Optional, default is white).</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawRectBorder(
            RectF dimensions,
            Color? borderColor = null,
            float? borderSize = null,
            TimeSpan? timeToLive = null);

        /// <summary>
        ///  Draws a rectangle border on the screen.
        /// </summary>
        /// <param name="dimensions">Rectangle dimensions in screen space.</param>
        /// <param name="borderColor">Color of the rectangle border, optional.</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawRectBorder(
            Rectangle dimensions,
            Color? borderColor = null,
            float? borderSize = null,
            TimeSpan? timeToLive = null);

        /// <summary>
        ///  Draws a filled rectangle on the screen.
        /// </summary>
        /// <param name="dimensions">Rectangle dimensions in screen space.</param>
        /// <param name="color">Color of the rectangle border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawFilledRect(
            RectF dimensions,
            Color? color = null,
            TimeSpan? timeToLive = null);

        /// <summary>
        ///  Draws a rectangle border representing a bounding rect region.
        /// </summary>
        /// <param name="rect">Rectangle dimensions in screen space.</param>
        /// <param name="borderColor">Color of the rectangle border, optional.</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawBoundingRect(
            BoundingRect rect,
            Color? borderColor = null,
            float? borderSize = null,
            TimeSpan? timeToLive = null);
        /// <summary>
        ///  Draws a border representing a bounding area region.
        /// </summary>
        /// <param name="bounds">Bounding area dimensions in screen space.</param>
        /// <param name="borderColor">Color of the bounding area border, optional.</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawBoundingArea(
            BoundingArea bounds,
            Color? borderColor = null,
            float? borderSize = null,
            TimeSpan? timeToLive = null);

        /// <summary>
        ///  Draws a line segment on the screen.
        /// </summary>
        /// <param name="start">Start of the line in screen space.</param>
        /// <param name="end">End of the line in screen space.</param>
        /// <param name="color">Color of the line. (Optional).</param>
        /// <param name="width">Width of the line. (Optional).</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawLine(
            Vector2 start,
            Vector2 end,
            Color? color = null,
            float? width = null,
            TimeSpan? timeToLive = null);

        /// <summary>
        ///  Draws text on the screen for debugging.
        /// </summary>
        /// <param name="text">Text to draw on the screen.</param>
        /// <param name="pos">Postion to draw the text in screen space coordinates.</param>
        /// <param name="color">Color to draw the text.</param>
        /// <param name="backgroundColor">Text rectangle background color.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        void DrawTextBox(
            string text,
            Vector2 pos,
            Color? textColor,
            Color? backgroundColor,
            TimeSpan? timeToLive = null);

        /// <summary>
        ///  This should be called at the start of each update cycle, which allows the debug
        ///  manager to reset geometry primitives. If this is not called often enough before a draw
        ///  call then the manager will run out of geometry primitives.
        ///  
        ///  Best avoided by simply calling this method once at the start of every Update() call.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        void PreUpdate(GameTime gameTime);

        /// <summary>
        ///  Draws all queued debug draws.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        void Draw(GameTime renderTime);
    }

    /// <summary>
    ///  Standard debug overlay implementation.
    /// </summary>
    /// <remarks>
    ///  TODO: Convert primitives from list to array, with Count/Capacity tracking.
    ///  TODO: Convert primitives from class to struct to enable array of structs.
    ///   (This needs better deltee/prune logic).
    /// </remarks>
    public class StandardDebugOverlay : IDebugOverlay
    {
        private const float DefaultLineWidth = 1.0f;
        private const float DefaultPointSize = 1.0f;
        private const float DefaultBorderWidth = 1.0f;
        private static readonly Color DefaultColor = Color.HotPink;

        /// <summary>
        ///  Sprite font used to render text
        /// </summary>
        private SpriteFont mDebugFont;

        /// <summary>
        /// List of debug rectangles to draw
        /// </summary>
        private List<DebugRectangle> mRectsToDraw;

        /// <summary>
        /// List of lines to draw
        /// </summary>
        private List<DebugLine> mLinesToDraw;

        /// <summary>
        /// List of text to draw
        /// </summary>
        private List<DebugText> mTextToDraw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphics"></param>
        public StandardDebugOverlay( GraphicsDevice graphics, ContentManager content )
        {
            mDebugFont = content.Load<SpriteFont>(Path.Combine("fonts", "System10"));

            mRectsToDraw = new List<DebugRectangle>(250);
            mLinesToDraw = new List<DebugLine>(250);
            mTextToDraw = new List<DebugText>(250);

            PreAllocate( 50 );
        }

        /// <summary>
        /// Unload the debug renderer
        /// </summary>
        public void Unload()
        {
        }

        /// <summary>
        ///  Draw a point on the screen.
        /// </summary>
        /// <param name="point">Center of the point in screen space.</param>
        /// <param name="color">Color of the point, optional.</param>
        /// <param name="sizeIn">Size of the point, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawPoint(
            Vector2 point,
            Color? color = null,
            float? sizeIn = null,
            TimeSpan? timeToLive = null)
        {
            var r = FindNextUnused(mRectsToDraw);
            var size = sizeIn ?? DefaultPointSize;

            r.Enabled = true;
            r.TimeToLive = timeToLive ?? TimeSpan.Zero;
            r.FillColor = color ?? DefaultColor;
            r.BorderColor = null;
            
            r.Dimensions = new RectF(
                left: point.X - size * 0.5f,
                top: point.Y - size * 0.5f, 
                width: size,
                height: size);
        }

        /// <summary>
        ///  Draws a rectangle border on the screen.
        /// </summary>
        /// <param name="dimensions">Rectangle dimensions in screen space.</param>
        /// <param name="borderColor">Color of the rectangle border. (Optional, default is white).</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawRectBorder(
            RectF dimensions,
            Color? borderColor = null,
            float? borderSize = null,
            TimeSpan? timeToLive = null)
        {
            var r = FindNextUnused(mRectsToDraw);

            r.Enabled = true;
            r.BorderColor = borderColor ?? DefaultColor;
            r.BorderSize = borderSize ?? DefaultBorderWidth;
            r.FillColor = null;
            r.TimeToLive = timeToLive ?? TimeSpan.Zero;
            r.Dimensions = dimensions;
        }

        /// <summary>
        ///  Draws a rectangle border on the screen.
        /// </summary>
        /// <param name="dimensions">Rectangle dimensions in screen space.</param>
        /// <param name="borderColor">Color of the rectangle border, optional.</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawRectBorder(
            Rectangle dimensions,
            Color? borderColor = null,
            float? borderSize = null,
            TimeSpan? timeToLive = null)
        {
            var r = FindNextUnused(mRectsToDraw);

            r.Enabled = true;
            r.BorderColor = borderColor ?? DefaultColor;
            r.BorderSize = borderSize ?? DefaultBorderWidth;
            r.FillColor = null;
            r.TimeToLive = timeToLive ?? TimeSpan.Zero;

            r.Dimensions = new RectF(
                top: dimensions.Top,
                left: dimensions.Left,
                width: dimensions.Width,
                height: dimensions.Height);
        }

        /// <summary>
        ///  Draws a filled rectangle on the screen.
        /// </summary>
        /// <param name="dimensions">Rectangle dimensions in screen space.</param>
        /// <param name="color">Color of the rectangle border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawFilledRect(
            RectF dimensions,
            Color? color = null,
            TimeSpan? timeToLive = null)
        {
            var r = FindNextUnused(mRectsToDraw);

            r.Enabled = true;
            r.BorderColor = null;
            r.FillColor = color ?? DefaultColor;
            r.TimeToLive = timeToLive ?? TimeSpan.Zero;

            r.Dimensions = dimensions;
        }

        /// <summary>
        ///  Draws a rectangle border representing a bounding rect region.
        /// </summary>
        /// <param name="rect">Rectangle dimensions in screen space.</param>
        /// <param name="borderColor">Color of the rectangle border, optional.</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawBoundingRect(
            BoundingRect rect,
            Color? borderColor = null,
            float? borderSize = null,
            TimeSpan? timeToLive = null)
        {
            var r = FindNextUnused(mRectsToDraw);

            r.Enabled = true;
            r.BorderColor = borderColor ?? DefaultColor;
            r.BorderSize = borderSize ?? DefaultBorderWidth;
            r.FillColor = null;
            r.TimeToLive = timeToLive ?? TimeSpan.Zero;
            
            r.Dimensions = new RectF(
                topLeft: rect.MinPoint,
                rectSize: new SizeF(rect.Width, rect.Height));
        }

        /// <summary>
        ///  Draws a border representing a bounding area region.
        /// </summary>
        /// <param name="bounds">Bounding area dimensions in screen space.</param>
        /// <param name="borderColor">Color of the bounding area border, optional.</param>
        /// <param name="borderSize">Size of the border, optional.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawBoundingArea(
            BoundingArea bounds,
            Color? borderColor = null, 
            float? borderSize = null,
            TimeSpan? timeToLive = null)
        {
            DrawLine(bounds.UpperLeft, bounds.UpperRight, borderColor, borderSize, timeToLive);
            DrawLine(bounds.UpperLeft, bounds.LowerLeft, borderColor, borderSize, timeToLive);

            DrawLine(bounds.LowerRight, bounds.UpperRight, borderColor, borderSize, timeToLive);
            DrawLine(bounds.LowerRight, bounds.LowerLeft, borderColor, borderSize, timeToLive);
        }

        /// <summary>
        ///  Draws a line segment on the screen.
        /// </summary>
        /// <param name="start">Start of the line in screen space.</param>
        /// <param name="end">End of the line in screen space.</param>
        /// <param name="color">Color of the line. (Optional).</param>
        /// <param name="width">Width of the line. (Optional).</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawLine(
            Vector2 start,
            Vector2 end,
            Color? color = null,
            float? width = null,
            TimeSpan? timeToLive = null)
        {
            var l = FindNextUnused(mLinesToDraw);

            l.Enabled = true;
            l.Color = color ?? DefaultColor;
            l.Width = width ?? DefaultLineWidth;
            l.TimeToLive = timeToLive ?? TimeSpan.Zero;

            l.Start = start;
            l.Stop = end;
        }
        
        /// <summary>
        ///  Draws text on the screen for debugging.
        /// </summary>
        /// <param name="text">Text to draw on the screen.</param>
        /// <param name="pos">Postion to draw the text in screen space coordinates.</param>
        /// <param name="color">Color to draw the text.</param>
        /// <param name="backgroundColor">Text rectangle background color.</param>
        /// <param name="timeToLive">Amount of time to persist, optional.</param>
        public void DrawTextBox(
            string text,
            Vector2 pos,
            Color? textColor,
            Color? backgroundColor,
            TimeSpan? timeToLive = null)
        {
            var t = FindNextUnused(mTextToDraw);

            t.Enabled = true;
            t.Text = text;
            t.Position = pos;
            t.Color = textColor ?? DefaultColor;
            t.BackgroundColor = backgroundColor;
            t.TimeToLive = timeToLive ?? TimeSpan.Zero;
        }

        /// <summary>
        ///  This should be called at the start of each update cycle, which allows the debug
        ///  manager to reset geometry primitives. If this is not called often enough before a draw
        ///  call then the manager will run out of geometry primitives.
        ///  
        ///  Best avoided by simply calling this method once at the start of every Update() call.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void PreUpdate( GameTime gameTime )
        {
            PrunePrimitiveList(mRectsToDraw, gameTime);
            PrunePrimitiveList(mLinesToDraw, gameTime);
            PrunePrimitiveList(mTextToDraw, gameTime);
        }

        /// <summary>
        ///  Draws all queued debug draws.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Draw( GameTime renderTime )
        {
            GameRoot.Renderer.StartDrawing();

            // Draw all queued debug primitives.
            for (int i = 0; i < mRectsToDraw.Count; i++)
            {
                var o = mRectsToDraw[i];

                // TODO: Remove when Enabled is removed (and use Count+move to end).
                if (!o.Enabled) { break; }
                GameRoot.Renderer.DrawRectangle(
                    o.Dimensions,
                    fillColor: o.FillColor,
                    borderColor: o.BorderColor,
                    borderSize: o.BorderSize);
            }

            for (int i = 0; i < mLinesToDraw.Count; i++)
            {
                var o = mLinesToDraw[i];
                // TODO: Remove when Enabled is removed (and use Count+move to end).
                if (!o.Enabled) { break; }
                GameRoot.Renderer.DrawLine(o.Start, o.Stop, o.Color, o.Width);
            }
            
            for (int i = 0; i < mTextToDraw.Count; i++)
            {
                var o = mTextToDraw[i];
                // TODO: Remove when Enabled is removed (and use Count+move to end).
                if (!o.Enabled) { break; }
                GameRoot.Renderer.DrawText(o.Text, o.Position, mDebugFont, o.Color, o.BackgroundColor);
            }

            // TODO: Draw name in bottom right corner.
            // Draw game name.
            GameRoot.Renderer.DrawText(
                "Dungeon Crawler",
                Vector2.Zero,
                mDebugFont,
                Color.White);

            // Detect if the game is running slowly, and if so draw an indicator
            DrawSimulationTimeIndicator( renderTime );
            GameRoot.Renderer.FinishDrawing();
        }

        /// <summary>
        /// Tests how long the previous simulation update cycle took, and draws an angry square
        /// indicator if exceeded the ideal maximum threshold.
        /// </summary>
        /// <param name="renderTime"></param>
        private void DrawSimulationTimeIndicator( GameTime renderTime )
        {
            if ( renderTime.IsRunningSlowly )
            {
                GameRoot.Renderer.DrawRectangle(
                    new RectF(top: 0, left: 0, width: 20, height: 20),
                    fillColor: Color.Red);
            }
            else
            {
                TimeSpan runningSlow = renderTime.ElapsedGameTime - TimeSpan.FromMilliseconds( 10.0 );

                if ( runningSlow.TotalMilliseconds > 10.0 )
                {
                    // TODO: Actually calculate how close it is to 16ms and draw an appropriately
                    // shaded color
                    GameRoot.Renderer.DrawRectangle(
                        new RectF(top: 0, left: 22, width: 20, height: 20),
                        fillColor: Color.Yellow);
                }
            }
        }

        /// <summary>
        /// Finds the next unused (disabled) primitive in a list of debugging primtives. This
        /// allows use to cache the creation of primitives and avoid tons of allocations per
        /// frame.
        /// </summary>
        /// <typeparam name="T">Debug primtive type</typeparam>
        /// <param name="list">List of debug primitive type</param>
        /// <returns>An unused instance</returns>
        private T FindNextUnused<T>( List<T> list ) where T : DebugPrimitive, new()
        {
            // Look through the list of debugging primitives, and find the first one that is
            // disabled (unused)
            T item = null;

            for ( int i = 0; i < list.Count && item == null; ++i )
            {
                if ( ! list[i].Enabled )
                {
                    item = list[i];
                }
            }

            // Did we find one? If not allocate a new one and add it to the list
            if ( item == null )
            {
                Debug.Assert( list.Count < 500 );

                item = new T();
                list.Add( item );
            }

            return item;
        }

        /// <summary>
        ///  Disables all primitives that should not be active any longer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="gameTime"></param>
        private void PrunePrimitiveList<T>( List<T> list, GameTime gameTime ) where T : DebugPrimitive
        {
            // Move old (age expired) and disabled primitives to the back of the list.
            int writeIndex = 0;

            for (int readIndex = 0; readIndex < list.Count; readIndex++)
            {
                var o = list[readIndex];

                if (o.Enabled &&
                    o.TimeToLive > TimeSpan.Zero && o.TimeToLive < gameTime.TotalGameTime)
                {
                    if (readIndex == writeIndex)
                    {
                        // Copy nothing optimization.
                        writeIndex++;
                    }
                    else
                    {
                        list[writeIndex++] = list[readIndex];
                    }
                }
                else
                {
                    // TODO: Remove once Enabled is taken out.
                    o.Enabled = false;
                }
            }
        }

        /// <summary>
        ///  Pre-allocate the requested number of primitives, so we don't have to create them one
        ///  at a time.
        /// </summary>
        /// <param name="size">Number to preallocate</param>
        private void PreAllocate( int size )
        {
            for ( int i = 0; i < size; ++i )
            {
                mRectsToDraw.Add( new DebugRectangle() );
                mLinesToDraw.Add( new DebugLine() );
                mTextToDraw.Add( new DebugText() );
            }
        }

        /// <summary>
        /// Debugging primitive
        /// </summary>
        abstract class DebugPrimitive
        {
            public bool Enabled = false;
            public TimeSpan TimeToLive = TimeSpan.Zero;
        }
        
        /// <summary>
        /// Debugging rectangle
        /// </summary>
        class DebugRectangle : DebugPrimitive
        {
            public RectF Dimensions;
            public Color? BorderColor;
            public Color? FillColor;
            public float BorderSize = 1.0f;
        }

        /// <summary>
        /// Debugging line
        /// </summary>
        class DebugLine : DebugPrimitive
        {
            public Vector2 Start;
            public Vector2 Stop;
            public Color Color;
            public float Width = 1.0f;
        }

        /// <summary>
        /// Debugging text
        /// </summary>
        class DebugText : DebugPrimitive
        {
            public string Text;
            public Vector2 Position;
            public Color Color;
            public Color? BackgroundColor;
        }
    }
}

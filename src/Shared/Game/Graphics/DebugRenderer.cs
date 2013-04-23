using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scott.Common;
using Scott.Geometry;
using System.Diagnostics;
using System.IO;

namespace Scott.Game.Graphics
{
    /// <summary>
    /// Assists in drawing debug objects. Makes debugging stuff MUCH easier.
    /// </summary>
    public class DebugRenderer
    {
        /// <summary>
        /// A simple 1x1 texture that can be arbitrarily colored and stretched. Perfect
        /// for our debugging
        /// </summary>
        private Texture2D mWhitePixel;

        /// <summary>
        /// Debug font to render text
        /// </summary>
        private SpriteFont mFont;

        /// <summary>
        /// Our private sprite batch
        /// </summary>
        private SpriteBatch mSpriteBatch;

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
        public DebugRenderer( GraphicsDevice graphics, ContentManager content )
        {
            mWhitePixel = new Texture2D( graphics, 1, 1 );
            mWhitePixel.SetData( new[] { Color.White } );

            mFont = content.Load<SpriteFont>( Path.Combine( "fonts", "System10" ) );

            mSpriteBatch = new SpriteBatch( graphics );

            mRectsToDraw = new List<DebugRectangle>( 250 );
            mLinesToDraw = new List<DebugLine>( 250 );
            mTextToDraw = new List<DebugText>( 250 );

            PreAllocate( 50 );
        }

        /// <summary>
        /// Unload the debug renderer
        /// </summary>
        public void Unload()
        {
            mWhitePixel.Dispose();
            mWhitePixel = null;

            mSpriteBatch.Dispose();
            mSpriteBatch = null;
        }

        /// <summary>
        /// Draw a rectangle on the screen
        /// </summary>
        /// <param name="dimensions">Dimensions of rectangle</param>
        [Conditional("DEBUG")]
        public void DrawRect( Rectangle dimensions )
        {
            DrawRect( dimensions, Color.HotPink );
        }

        /// <summary>
        ///  Draws a debugging rectangle on the screen.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="color"></param>
        [Conditional( "DEBUG" )]
        public void DrawRect( RectangleF dimensions, Color color )
        {
            DebugRectangle r = FindNextUnused<DebugRectangle>( mRectsToDraw );

            r.Color = color;
            r.Filled = false;
            r.Dimensions = dimensions.ToRectangle();
            r.Enabled = true;
        }

        /// <summary>
        ///  Draws a debugging rectangle on the screen.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="color"></param>
        [Conditional( "DEBUG" )]
        public void DrawRect( Rectangle dimensions, Color color )
        {
            DebugRectangle r = FindNextUnused<DebugRectangle>( mRectsToDraw );

            r.Color = color;
            r.Filled = false;
            r.Dimensions = dimensions;
            r.Enabled = true;
        }

        /// <summary>
        ///  Draws a a filled debugging rectangle on the screen.
        /// </summary>
        /// <param name="dimensions">Size of the rectangle.</param>
        /// <param name="color">Color of the rectangle.</param>
        [Conditional( "DEBUG" )]
        public void DrawFilledRect( Rectangle dimensions, Color color )
        {
            DebugRectangle r = FindNextUnused<DebugRectangle>( mRectsToDraw );

            r.Color = color;
            r.Filled = true;
            r.Dimensions = dimensions;
            r.Enabled = true;
        }

        /// <summary>
        ///  Draws a bounding area on the screen for debugging.
        /// </summary>
        /// <param name="bounds">Size of the bounding area.</param>
        [Conditional( "DEBUG" )]
        public void DrawBoundingArea( BoundingArea bounds )
        {
            DrawRect( bounds.BroadPhaseRectangle, Color.Green );

            // Draw the bounding box that is possibly rotated
            if ( bounds.Rotation != 0.0f )
            {
                Vector2 pos = bounds.WorldPosition;

                DrawLine( bounds.UpperLeft + pos, bounds.UpperRight + pos, Color.Purple );
                DrawLine( bounds.UpperLeft + pos, bounds.LowerLeft + pos, Color.Purple );

                DrawLine( bounds.LowerRight + pos, bounds.UpperRight + pos, Color.Purple );
                DrawLine( bounds.LowerRight + pos, bounds.LowerLeft + pos, Color.Purple );
            }
        }

        /// <summary>
        ///  Draws a line segment on the screen for debugging.
        /// </summary>
        /// <param name="start">Start of the line in screen space.</param>
        /// <param name="end">End of the line in screen space.</param>
        [Conditional( "DEBUG" )]
        public void DrawLine( Vector2 start, Vector2 end )
        {
            DrawLine( start, end, Color.HotPink, 1 );
        }

        /// <summary>
        ///  Draws a line segment on the screen for debugging.
        /// </summary>
        /// <param name="start">Start of the line in screen space.</param>
        /// <param name="end">End of the line in screen space.</param>
        /// <param name="color">Color of the line.</param>
        [Conditional( "DEBUG" )]
        public void DrawLine( Vector2 start, Vector2 end, Color color )
        {
            DrawLine( start, end, color, 1 );
        }


        /// <summary>
        ///  Draws a line segment on the screen for debugging.
        /// </summary>
        /// <param name="start">Start of the line in screen space.</param>
        /// <param name="end">End of the line in screen space.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="width">Width of the line.</param>
        [Conditional( "DEBUG" )]
        public void DrawLine( Vector2 start, Vector2 end, Color color, int width )
        {
            DebugLine l = FindNextUnused<DebugLine>( mLinesToDraw );

            l.Color = color;
            l.Start = start;
            l.Stop = end;
            l.Enabled = true;
            l.Width = width;
        }

        /// <summary>
        ///  Draws text on the screen for debugging.
        /// </summary>
        /// <param name="text">Text to draw on the screen.</param>
        /// <param name="pos">Postion to draw the text in screen space coordinates.</param>
        [Conditional( "DEBUG" )]
        public void DrawText( string text, Vector2 pos )
        {
            DrawText( text, pos, Color.Black );
        }

        /// <summary>
        ///  Draws text on the screen for debugging.
        /// </summary>
        /// <param name="text">Text to draw on the screen.</param>
        /// <param name="pos">Postion to draw the text in screen space coordinates.</param>
        /// <param name="color">Color to draw the text.</param>
        [Conditional( "DEBUG" )]
        public void DrawText( string text, Vector2 pos, Color color )
        {
            DebugText t = FindNextUnused<DebugText>( mTextToDraw );

            t.Text = text;
            t.Position = pos;
            t.Color = color;
            t.DrawBackground = false;
            t.Enabled = true;
        }

        /// <summary>
        ///  Draws text on the screen for debugging.
        /// </summary>
        /// <param name="text">Text to draw on the screen.</param>
        /// <param name="pos">Postion to draw the text in screen space coordinates.</param>
        /// <param name="color">Color to draw the text.</param>
        /// <param name="backgroundColor">Text rectangle background color.</param>
        [Conditional( "DEBUG" )]
        public void DrawTextBox( string text, Vector2 pos, Color textColor, Color backgroundColor )
        {
            DebugText t = FindNextUnused<DebugText>( mTextToDraw );

            t.Text = text;
            t.Position = pos;
            t.Color = textColor;
            t.BackgroundColor = backgroundColor;
            t.DrawBackground = true;
            t.Enabled = true;
        }

        /// <summary>
        ///  This should be called at the start of each update cycle, which allows the debug
        ///  manager to reset geometry primitives. If this is not called often enough before a draw
        ///  call then the manager will run out of geometry primitives.
        ///  
        ///  Best avoided by simply calling this method once at the start of every Update() call.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        [Conditional( "DEBUG" )]
        public void PreUpdate( GameTime gameTime )
        {
            PrunePrimitiveList<DebugRectangle>( mRectsToDraw, gameTime );
            PrunePrimitiveList<DebugLine>( mLinesToDraw, gameTime );
            PrunePrimitiveList<DebugText>( mTextToDraw, gameTime );
        }

        /// <summary>
        ///  Draws all queued debug draws.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        [Conditional( "DEBUG" )]
        public void Draw( GameTime renderTime )
        {
            mSpriteBatch.Begin();

            // Draw our debugging primitives.
            DrawPrimitivesInList( mRectsToDraw );
            DrawPrimitivesInList( mLinesToDraw );
            DrawPrimitivesInList( mTextToDraw );

            // Draw game name.
            mSpriteBatch.DrawString( mFont, "Dungeon Crawler", Vector2.Zero, Color.White );

            mSpriteBatch.End();

            // Detect if the game is running slowly, and if so draw an indicator
            DrawSimulationTimeIndicator( renderTime );
        }

        /// <summary>
        /// Tests how long the previous simulation update cycle took, and draws an angry square
        /// indicator if exceeded the ideal maximum threshold.
        /// </summary>
        /// <param name="renderTime"></param>
        private void DrawSimulationTimeIndicator( GameTime renderTime )
        {
            mSpriteBatch.Begin();

            if ( renderTime.IsRunningSlowly )
            {
                mSpriteBatch.Draw( mWhitePixel, new Rectangle( 0, 0, 20, 20 ), Color.Red );
            }
            else
            {
                TimeSpan runningSlow = renderTime.ElapsedGameTime - TimeSpan.FromMilliseconds( 10.0 );

                if ( runningSlow.TotalMilliseconds > 10.0 )
                {
                    // TODO: Actually calculate how close it is to 16ms and draw an appropriately
                    // shaded color
                    mSpriteBatch.Draw( mWhitePixel, new Rectangle( 0, 0, 20, 20 ), Color.Yellow );
                }
            }

            mSpriteBatch.End();
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
        ///  Draws all enabled debug primitives in the given list.
        /// </summary>
        /// <typeparam name="T">Debug primtive type.</typeparam>
        /// <param name="list">The primtive list to render from.</param>
        private void DrawPrimitivesInList<T>( List<T> list ) where T : DebugPrimitive
        {
            foreach ( T t in list )
            {
                if ( t.Enabled )
                {
                    t.Draw( mSpriteBatch, mWhitePixel, mFont );
                }
            }
        }

        /// <summary>
        ///  Disables all primitives that should not be active any longer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="gameTime"></param>
        private void PrunePrimitiveList<T>( List<T> list, GameTime gameTime ) where T : DebugPrimitive
        {
            foreach ( T t in list )
            {
                if ( t.TimeToLive == TimeSpan.Zero || t.TimeToLive >= gameTime.TotalGameTime )
                {
                    t.Enabled = false;
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

            /// <summary>
            ///  Draw the primitive onto the screen.
            /// </summary>
            /// <param name="spriteBatch"></param>
            /// <param name="pixel"></param>
            /// <param name="font"></param>
            public abstract void Draw( SpriteBatch spriteBatch, Texture2D pixel, SpriteFont font );
        }
        
        /// <summary>
        /// Debugging rectangle
        /// </summary>
        class DebugRectangle : DebugPrimitive
        {
            public Rectangle Dimensions;
            public Color Color;
            public bool Filled;

            public override void Draw( SpriteBatch spriteBatch, Texture2D pixel, SpriteFont font )
            {
                if ( Filled )
                {
                    spriteBatch.Draw( pixel, Dimensions, Color );
                }
                else
                {
                    int left = Dimensions.X;
                    int top = Dimensions.Y;
                    int right = Dimensions.X + Dimensions.Width;
                    int bottom = Dimensions.Y + Dimensions.Height;
                    int width = Dimensions.Width;
                    int height = Dimensions.Height;

                    spriteBatch.Draw( pixel, new Rectangle( left, top, width, 1 ), Color );
                    spriteBatch.Draw( pixel, new Rectangle( left, top, 1, height ), Color );
                    spriteBatch.Draw( pixel, new Rectangle( left, bottom, width, 1 ), Color );
                    spriteBatch.Draw( pixel, new Rectangle( right, top, 1, height ), Color );
                }
            }
        }

        /// <summary>
        /// Debugging line
        /// </summary>
        class DebugLine : DebugPrimitive
        {
            public Vector2 Start;
            public Vector2 Stop;
            public Color Color;
            public int Width;

            public override void Draw( SpriteBatch spriteBatch, Texture2D pixel, SpriteFont font )
            {
                float angle  = (float) Math.Atan2( Stop.Y - Start.Y, Stop.X - Start.X );
                float length = (float) Vector2.Distance( Start, Stop );

                spriteBatch.Draw(
                    pixel,
                    Start,
                    null,
                    Color,
                    angle,
                    Vector2.Zero,
                    new Vector2( length, Width ),
                    SpriteEffects.None,
                    0 );
            }
        }

        /// <summary>
        /// Debugging text
        /// </summary>
        class DebugText : DebugPrimitive
        {
            public string Text;
            public Vector2 Position;
            public Color Color;
            public Color BackgroundColor;
            public bool DrawBackground;

            public override void Draw( SpriteBatch spriteBatch, Texture2D pixel, SpriteFont font )
            {
                if ( DrawBackground )
                {
                    // how big is this string?
                    Vector2 size = font.MeasureString( Text );

                    // Draw a rectangle filler that is slightly larger
                    Rectangle dims = new Rectangle( (int) ( Position.X - 2.0f ),
                                                    (int) ( Position.Y - 2.0f ),
                                                    (int) ( size.X + 4.0f ),
                                                    (int) ( size.Y + 4.0f ) );

                    spriteBatch.Draw( pixel, dims, BackgroundColor );
                }

                spriteBatch.DrawString( font, Text, Position, Color );
            }
        }
    }
}

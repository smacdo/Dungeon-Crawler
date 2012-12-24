using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scott.Dungeon.Data;

namespace Scott.Dungeon.Graphics
{
    /// <summary>
    /// Assists in drawing debug objects. Makes debugging stuff MUCH easier
    ///  
    /// TODO: Don't allocate nearly as much as we are. Cache stuff like crazy
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

            mFont = content.Load<SpriteFont>( "DebugFont" );

            mSpriteBatch = new SpriteBatch( graphics );

            mRectsToDraw = new List<DebugRectangle>( 100 );
            mLinesToDraw = new List<DebugLine>( 100 );
            mTextToDraw = new List<DebugText>( 100 );
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
        public void DrawRect( Rectangle dimensions )
        {
            DrawRect( dimensions, Color.HotPink );
        }

        /// <summary>
        /// Draw a rectangle on the screen
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="color"></param>
        public void DrawRect( Rectangle dimensions, Color color )
        {
            DebugRectangle r = FindNextUnused<DebugRectangle>( mRectsToDraw );

            r.Color = color;
            r.Filled = false;
            r.Dimensions = dimensions;
            r.Enabled = true;
        }

        public void DrawFilledRect( Rectangle dimensions, Color color )
        {
            DebugRectangle r = FindNextUnused<DebugRectangle>( mRectsToDraw );

            r.Color = color;
            r.Filled = true;
            r.Dimensions = dimensions;
            r.Enabled = true;
        }

        public void DrawBoundingBox( BoundingRect box, Color color )
        {
            DrawLine( box.UpperLeft, box.UpperRight, color );
            DrawLine( box.UpperLeft, box.LowerLeft, color );

            DrawLine( box.LowerRight, box.UpperRight, color );
            DrawLine( box.LowerRight, box.LowerLeft, color );

            Vector2 origin = box.Origin;
            DrawFilledRect( new Rectangle( (int) origin.X - 3, (int) origin.Y - 3, 6, 6 ), Color.Red );
        }

        public void DrawLine( Vector2 start, Vector2 end )
        {
            DrawLine( start, end, Color.HotPink );
        }

        public void DrawLine( Vector2 start, Vector2 end, Color color )
        {
            DebugLine l = FindNextUnused<DebugLine>( mLinesToDraw );

            l.Color = color;
            l.Start = start;
            l.Stop = end;
            l.Enabled = true;
        }

        public void DrawText( string text, Vector2 pos )
        {
            DrawText( text, pos, Color.Black );
        }

        public void DrawText( string text, Vector2 pos, Color color )
        {
            DebugText t = FindNextUnused<DebugText>( mTextToDraw );

            t.Text = text;
            t.Position = pos;
            t.Color = color;
            t.DrawBackground = false;
            t.Enabled = true;
        }

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
        /// Called before the rest of the system starts updating. Cleans up junk debug primitives
        /// before the next update cycle
        /// </summary>
        /// <param name="gameTime"></param>
        public void PreUpdate( GameTime gameTime )
        {
            PrunePrimitiveList<DebugRectangle>( mRectsToDraw, gameTime );
            PrunePrimitiveList<DebugLine>( mLinesToDraw, gameTime );
            PrunePrimitiveList<DebugText>( mTextToDraw, gameTime );
        }

        /// <summary>
        /// Performs any queued debugging primitives
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin();

            // Draw our debugging primitives.
            foreach ( DebugRectangle rect in mRectsToDraw )
            {
                if ( rect.Enabled )
                {
                    DrawItem( rect );
                }
            }

            foreach ( DebugLine line in mLinesToDraw )
            {
                if ( line.Enabled )
                {
                    DrawItem( line );
                }
            }

            foreach ( DebugText text in mTextToDraw )
            {
                if ( text.Enabled )
                {
                    DrawItem( text );
                }
            }

            mSpriteBatch.End();
        }

        private void DrawItem( DebugPrimitive primitive )
        {
            Console.WriteLine( "This should never get called" );
        }

        /// <summary>
        /// Draws a rectangle on the screen
        /// </summary>
        /// <param name="r">Rectangle to draw</param>
        private void DrawItem( DebugRectangle r )
        {
            if ( r.Filled )
            {
                mSpriteBatch.Draw( mWhitePixel,
                                   r.Dimensions,
                                   r.Color );
            }
            else
            {
                int left = r.Dimensions.X;
                int top = r.Dimensions.Y;
                int right = r.Dimensions.X + r.Dimensions.Width;
                int bottom = r.Dimensions.Y + r.Dimensions.Height;
                int width = r.Dimensions.Width;
                int height = r.Dimensions.Height;

                mSpriteBatch.Draw( mWhitePixel,
                                   new Rectangle( left, top, width, 1 ),
                                   r.Color );

                mSpriteBatch.Draw( mWhitePixel,
                                   new Rectangle( left, top, 1, height ),
                                   r.Color );

                mSpriteBatch.Draw( mWhitePixel,
                                   new Rectangle( left, bottom, width, 1 ),
                                   r.Color );

                mSpriteBatch.Draw( mWhitePixel,
                                   new Rectangle( right, top, 1, height ),
                                   r.Color );
            }

        }

        /// <summary>
        /// Draws a line on the screen
        /// </summary>
        /// <param name="line"></param>
        private void DrawItem( DebugLine line )
        {
            float angle  = (float) Math.Atan2( line.Stop.Y - line.Start.Y, line.Stop.X - line.Start.X );
            float length = (float) Vector2.Distance( line.Start, line.Stop );

            mSpriteBatch.Draw(
                mWhitePixel,
                line.Start,
                null,
                line.Color,
                angle,
                Vector2.Zero,
                new Vector2( length, 1 ),
                SpriteEffects.None,
                0 );
        }

        /// <summary>
        /// Draws text on the screen
        /// </summary>
        /// <param name="text">Text on the screen</param>
        private void DrawItem( DebugText text )
        {
            if ( text.DrawBackground )
            {
                // how big is this string?
                Vector2 size = mFont.MeasureString( text.Text );

                // Draw a rectangle filler that is slightly larger
                Rectangle dims = new Rectangle( (int) ( text.Position.X - 2.0f ),
                                                (int) ( text.Position.Y - 2.0f ),
                                                (int) ( size.X + 4.0f ),
                                                (int) ( size.Y + 4.0f ) );

                mSpriteBatch.Draw( mWhitePixel,
                                   dims,
                                   text.BackgroundColor );
            }

            mSpriteBatch.DrawString( mFont, text.Text, text.Position, text.Color );
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
                if ( list.Count > 50 )
                {
                    Console.Out.WriteLine( "ITS HUGE" );
                }

                item = new T();
                list.Add( item );
            }

            return item;
        }

        private void DrawPrimitivesInList<T>( List<T> list ) where T : DebugPrimitive
        {
            foreach ( T t in list )
            {
                if ( t.Enabled )
                {
                    DrawItem( t );
                }
            }
        }

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
        /// Debugging primitive
        /// </summary>
        class DebugPrimitive
        {
            public bool Enabled = false;
            public TimeSpan TimeToLive = TimeSpan.Zero;
        }
        
        /// <summary>
        /// Debugging rectangle
        /// </summary>
        class DebugRectangle : DebugPrimitive
        {
            public Rectangle Dimensions;
            public Color Color;
            public bool Filled;
        }

        /// <summary>
        /// Debugging line
        /// </summary>
        class DebugLine : DebugPrimitive
        {
            public Vector2 Start;
            public Vector2 Stop;
            public Color Color;
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
        }
    }
}

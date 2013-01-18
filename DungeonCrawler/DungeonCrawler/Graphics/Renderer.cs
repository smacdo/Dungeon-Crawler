using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.Graphics
{
    /// <summary>
    /// It draws the game
    /// </summary>
    public class Renderer
    {
        /// <summary>
        /// Information on how to draw the requested sprite
        /// </summary>
        private struct SpriteRenderInfo
        {
            public Sprite Sprite;
            public Vector2 Position;

            public SpriteRenderInfo( Sprite sprite, Vector2 position )
                : this()
            {
                Sprite = sprite;
                Position = position;
            }
        }

        private GraphicsDevice mGraphicsDevice;
        private List<SpriteRenderInfo> mSpritesToDraw;
        private SpriteBatch mSpriteBatch;

        /// <summary>
        /// A simple 1x1 texture that can be arbitrarily colored and stretched. Perfect for little boxes
        /// </summary>
        private Texture2D mSinglePIxel;

        public Renderer( GraphicsDevice graphics )
        {
            mGraphicsDevice = graphics;
            mSpritesToDraw = new List<SpriteRenderInfo>( 4096 );

            mSpriteBatch = new SpriteBatch( mGraphicsDevice );

            mSinglePIxel = new Texture2D( mGraphicsDevice, 1, 1 );
            mSinglePIxel.SetData( new[] { Color.White } );
        }

        /// <summary>
        /// Clear any pending items that are still queued for drawing
        /// </summary>
        public void ClearQueuedItems()
        {
            mSpritesToDraw.Clear();
        }

        public void Draw( Sprite sprite, Vector2 position )
        {
            mSpritesToDraw.Add( new SpriteRenderInfo( sprite, position ) );
        }

        public void DrawScreen( GameTime renderTime )
        {
            mGraphicsDevice.Clear( Color.CornflowerBlue );

            // Draw all requested sprites
            mSpriteBatch.Begin();

            foreach ( SpriteRenderInfo renderInfo in mSpritesToDraw )
            {
                Sprite sprite = renderInfo.Sprite;

                if ( sprite.Visible )
                {
                    mSpriteBatch.Draw( sprite.CurrentAtlasTexture,
                                       sprite.DrawOffset + renderInfo.Position,
                                       sprite.CurrentAtlasOffset,
                                       Color.White );
                }
            }

            mSpriteBatch.End();

            // Perform any post rendering tasks
            PostDrawScreen();
        }

        private void PostDrawScreen()
        {
            mSpritesToDraw.Clear();
        }
    }
}

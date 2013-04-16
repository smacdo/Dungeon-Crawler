using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Scott.Game.Graphics
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
            public Texture2D TextureAtlas;
            public Rectangle OffsetRect;
            public Vector2 Position;

            public SpriteRenderInfo( Texture2D atlas, Rectangle offset, Vector2 pos )
                : this()
            {
                TextureAtlas = atlas;
                OffsetRect = offset;
                Position = pos;
            }
        }

        private GraphicsDevice mGraphicsDevice;
        private List< SpriteRenderInfo > mSpritesToDraw;
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

        public void Draw( Texture2D atlas, Rectangle offset, Vector2 position )
        {
            Debug.Assert( atlas != null, "Texture must be valid" );
            mSpritesToDraw.Add( new SpriteRenderInfo( atlas, offset, position ) );
        }

        public void DrawScreen( GameTime renderTime )
        {
            mGraphicsDevice.Clear( Color.CornflowerBlue );

            // Draw all requested sprites
            mSpriteBatch.Begin();

            foreach ( SpriteRenderInfo sprite in mSpritesToDraw )
            {
                mSpriteBatch.Draw( sprite.TextureAtlas,
                                   sprite.Position,
                                   sprite.OffsetRect,
                                   Color.White );
            }

            mSpriteBatch.End();

            // Perform any post rendering tasks
            PostDrawScreen();
        }

        private void PostDrawScreen()
        {
            ClearQueuedItems();
        }
    }
}

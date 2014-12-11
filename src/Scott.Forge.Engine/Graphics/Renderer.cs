/*
 * Copyright 2012-2014 Scott MacDonald
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

namespace Scott.Forge.Engine.Graphics
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
                                   sprite.Position.ToXnaVector2(),
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

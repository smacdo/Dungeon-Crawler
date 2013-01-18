using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scott.Dungeon.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Dungeon.Data;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// The sprite component represents a sprite visible to the camera
    /// </summary>
    public class SpriteComponent : AbstractGameObjectComponent
    {
        /// <summary>
        /// Represents a sprite that needs to be drawn
        /// </summary>
        private class SpriteItem
        {
            /// <summary>
            /// The current texture that should be displayed for the sprite
            /// </summary>
            public Texture2D AtlasTexture;

            /// <summary>
            /// A rectangle describing the offset and dimensions of the current animation frame
            /// inside of the texture atlas
            /// </summary>
            public Rectangle AtlasSpriteRect;

            /// <summary>
            /// Offset from the standard top left origin
            ///   SpriteData.OriginOffset
            /// </summary>
            public Vector2 DrawOffset;

        }

        private List<SpriteItem> mItems;
        public Layer Layer { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
            mItems = new List<SpriteItem>( 1 );
            Layer  = Layer.Default;
        }

        public int AddSprite( SpriteData sprite )
        {
            SpriteItem item = new SpriteItem();

            item.AtlasTexture    = sprite.Texture;
            item.AtlasSpriteRect = new Rectangle( 0, 0, sprite.Texture.Width, sprite.Texture.Height );
            item.DrawOffset      = sprite.OriginOffset;

            mItems.Add( item );

            return mItems.Count - 1;
        }

        public void SetLayer( int layerIndex,
                              Texture2D atlasTexture,
                              Rectangle atlasSpriteRect,
                              Vector2 drawOffset )
        {
            mItems[layerIndex].AtlasTexture = atlasTexture;
            mItems[layerIndex].AtlasSpriteRect = atlasSpriteRect;
            mItems[layerIndex].DrawOffset = drawOffset;
        }

        /// <summary>
        /// Send update to the sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            if ( Enabled )
            {
                for ( int i = 0; i < mItems.Count; ++i )
                {
                    GameRoot.Renderer.Draw( Layer,
                                            mItems[i].AtlasTexture,
                                            mItems[i].AtlasSpriteRect,
                                            mItems[i].DrawOffset + Owner.Position );
                }
                

                if ( Owner.Bounds != null )
                {
                    GameRoot.Debug.DrawBoundingArea( Owner.Bounds );
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scott.Dungeon.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// The sprite component represents a sprite visible to the camera
    /// </summary>
    public class SpriteComponent : AbstractGameObjectComponent
    {
        /// <summary>
        /// The current texture that should be displayed for the sprite
        /// </summary>
        public Texture2D AtlasTexture { get; set; }

        /// <summary>
        /// A rectangle describing the offset and dimensions of the current animation frame
        /// inside of the texture atlas
        /// </summary>
        public Rectangle AtlasSpriteRect { get; set; }

        /// <summary>
        /// Offset from the standard top left origin
        ///   SpriteData.OriginOffset
        /// </summary>
        public Vector2 DrawOffset { get; set; }

        public Layer Layer { get; set; }

        /// <summary>
        /// Width of the sprite
        /// </summary>
        public int Width
        {
            get
            {
                return AtlasSpriteRect.Width;
            }
        }

        /// <summary>
        /// Height of the sprite
        /// </summary>
        public int Height
        {
            get
            {
                return AtlasSpriteRect.Height;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
            AtlasTexture    = null;
            AtlasSpriteRect = new Rectangle( 0, 0, 0, 0 );
            DrawOffset      = Vector2.Zero;
            Layer           = Layer.Default;
        }

        /// <summary>
        /// Send update to the sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            if ( Enabled )
            {
                GameRoot.Renderer.Draw( Layer, AtlasTexture, AtlasSpriteRect, DrawOffset + Owner.Position );

                if ( Owner.Bounds != null )
                {
                    GameRoot.Debug.DrawBoundingArea( Owner.Bounds );
                }
            }
        }
    }
}

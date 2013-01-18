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
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
            Sprite = null;
        }

        /// <summary>
        /// Send update to the sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            if ( Enabled )
            {
                Sprite.Draw( Owner.Position );

                if ( Owner.Bounds != null )
                {
                    GameRoot.Debug.DrawBoundingArea( Owner.Bounds );
                }
            }
        }
    }
}

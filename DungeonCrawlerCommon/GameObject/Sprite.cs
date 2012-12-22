using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace scott.dungeon.gameobject
{
    /// <summary>
    /// The sprite class is the base class used for rendering animated 2d images on
    /// screen. It uses a flywheel pattern to make sprites lightweight, where the sprite
    /// class itself stores current animation information, and references all the static
    /// sprite data from the SpriteData class.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Sprite data that this sprite is using
        /// </summary>
        private SpriteData mSpriteData;

        /// <summary>
        /// The animation that is currently playing
        /// </summary>
        private AnimationData mCurrentAnimation;

        /// <summary>
        /// The current frame being animated
        /// </summary>
        private int mCurrentFrame;

        /// <summary>
        /// The time that the current frame was first displayed
        /// </summary>
        private TimeSpan mFrameStartTime;

        /// <summary>
        /// The current texture that should be displayed for the sprite
        /// </summary>
        public Texture2D CurrentAtlasTexture { get; private set; }

        /// <summary>
        /// A rectangle describing the offset and dimensions of the current animation frame
        /// inside of the texture atlas
        /// </summary>
        public Rectangle CurrentAtlasOffset { get; private set; }

        public int Width
        {
            get
            {
                return CurrentAtlasOffset.Width;
            }
        }

        public int Height
        {
            get
            {
                return CurrentAtlasOffset.Height;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteData">The sprite data that this sprite will be rendering</param>
        public Sprite( SpriteData spriteData )
        {
            mSpriteData       = spriteData;
            mCurrentAnimation = spriteData.Animations[spriteData.DefaultAnimationName];
            mCurrentFrame     = 0;
            mFrameStartTime   = TimeSpan.MinValue;

            CurrentAtlasTexture = spriteData.Texture;
            CurrentAtlasOffset = mCurrentAnimation.Frames[0];
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public void Update( GameTime gameTime )
        {
            TimeSpan frameTime = TimeSpan.FromSeconds( 0.1 );

            // Check if this is the first time we've updated this sprite. If so, initialize our
            // animation values for the next call to update. Otherwise proceed as normal
            if ( mFrameStartTime != TimeSpan.MinValue )
            {
                // Is it time to move to the next frame of animation?
                if ( mFrameStartTime.Add( frameTime ) <= gameTime.TotalGameTime )
                {
                    mCurrentFrame      = ( mCurrentFrame + 1 ) % mCurrentAnimation.FrameCount;
                    CurrentAtlasOffset = mCurrentAnimation.Frames[mCurrentFrame];
                    mFrameStartTime    = gameTime.TotalGameTime;
                }
            }
            else
            {
                mFrameStartTime = gameTime.TotalGameTime;
            }
        }

        /// <summary>
        /// Plays the requested animation
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation( string animationName )
        {
            AnimationData animation = null;

            if ( mSpriteData.Animations.TryGetValue( animationName, out animation ) )
            {
                mCurrentAnimation = animation;
                mCurrentFrame = 0;
                mFrameStartTime = TimeSpan.MinValue;

                CurrentAtlasOffset = mCurrentAnimation.Frames[mCurrentFrame];
            }
            else
            {
                throw new AnimationException( "Failed to find animation named " + animationName,
                                              this,
                                              animationName,
                                              0 );
            }
        }
    }
}

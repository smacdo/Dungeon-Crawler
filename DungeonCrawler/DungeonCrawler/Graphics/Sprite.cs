using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Dungeon.ComponentModel;
using Scott.Dungeon.Data;

namespace Scott.Dungeon.Graphics
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
        /// True if an animation is being played, false if it is not. (False also means
        /// a current animation is frozen).
        /// </summary>
        private bool mIsAnimating;

        /// <summary>
        /// Controls what happens when the current animation completes
        /// </summary>
        private AnimationEndingAction mAnimationEndingAction;

        /// <summary>
        /// The current texture that should be displayed for the sprite
        /// </summary>
        public Texture2D CurrentAtlasTexture { get; private set; }

        /// <summary>
        /// A rectangle describing the offset and dimensions of the current animation frame
        /// inside of the texture atlas
        /// </summary>
        public Rectangle CurrentAtlasOffset { get; private set; }

        /// <summary>
        /// Offset from the standard top left origin
        /// </summary>
        public Vector2 DrawOffset
        {
            get
            {
                return mSpriteData.OriginOffset;
            }
        }

        /// <summary>
        /// Width of the sprite
        /// </summary>
        public int Width
        {
            get
            {
                return CurrentAtlasOffset.Width;
            }
        }

        /// <summary>
        /// Height of the sprite
        /// </summary>
        public int Height
        {
            get
            {
                return CurrentAtlasOffset.Height;
            }
        }

        /// <summary>
        /// If the sprite should be animated and displayed
        /// </summary>
        public bool Visible { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteData">The sprite data that this sprite will be rendering</param>
        public Sprite( SpriteData spriteData, bool visible = true )
        {
            mSpriteData       = spriteData;
            mCurrentAnimation = spriteData.Animations[spriteData.DefaultAnimationName];
            mCurrentFrame     = 0;
            mFrameStartTime   = TimeSpan.MinValue;
            mIsAnimating      = false;

            CurrentAtlasTexture = spriteData.Texture;
            CurrentAtlasOffset = mCurrentAnimation.Frames[0];
            Visible = visible;
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public void Update( GameTime gameTime )
        {
            TimeSpan frameTime = TimeSpan.FromSeconds( 0.1 );       // this needs to go.. should be read in via XML per animation

            // Don't update if we are not visible
            if ( !Visible )
            {
                return;
            }

            // Check if this is the first time we've updated this sprite. If so, initialize our
            // animation values for the next call to update. Otherwise proceed as normal
            if ( mFrameStartTime == TimeSpan.MinValue )        // start the clock
            {
                mFrameStartTime = gameTime.TotalGameTime;
            }
            else if ( mIsAnimating && mFrameStartTime.Add( frameTime ) <= gameTime.TotalGameTime )
            {
                // Are we at the end of this animation?
                if ( mCurrentFrame + 1 == mCurrentAnimation.FrameCount )
                {
                    switch ( mAnimationEndingAction )
                    {
                        case AnimationEndingAction.Loop:
                            mCurrentFrame = 0;
                            break;

                        case AnimationEndingAction.Stop:
                            mIsAnimating = false;
                            break;

                        case AnimationEndingAction.StopAndReset:
                            mCurrentFrame = 0;
                            mIsAnimating = false;
                            break;
                    }
                }
                else
                {
                    // We're not at the end... just increment the frame and continue
                    mCurrentFrame++;
                }

                // Update state
                CurrentAtlasOffset = mCurrentAnimation.Frames[mCurrentFrame];
                mFrameStartTime = gameTime.TotalGameTime;
            }
        }

        /// <summary>
        /// Plays the requested animation
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        /// <param name="endingAction">Action to take when the animation ends</param>
        public void PlayAnimation( string animationName, AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset )
        {
            AnimationData animation = null;

            // Do not animate if this sprite is not visible
            if ( !Visible )
            {
                return;
            }

            // Attempt to retrieve the requested animation. If the animation exists, go ahead and
            // start playing it
            if ( mSpriteData.Animations.TryGetValue( animationName, out animation ) )
            {
                mCurrentAnimation = animation;
                mCurrentFrame = 0;
                mFrameStartTime = TimeSpan.MinValue;

                CurrentAtlasOffset = mCurrentAnimation.Frames[mCurrentFrame];
                mAnimationEndingAction = endingAction;
                mIsAnimating = true;
            }
            else
            {
                throw new AnimationException( "Failed to find animation named " + animationName,
                                              this,
                                              animationName,
                                              0 );
            }
        }

        /// <summary>
        /// Play a requested animation and have it loop until interrupted
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        public void PlayAnimationLooping( string animationName )
        {
            PlayAnimation( animationName, AnimationEndingAction.Loop );
        }

        public bool IsPlayingAnimation( string animationName )
        {
            return ( mCurrentAnimation != null && mCurrentAnimation.Name == animationName );
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Scott.Dungeon.Graphics;
using Scott.Dungeon.Data;
using Microsoft.Xna.Framework.Graphics;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// An animation component manages a sprite's animation
    /// </summary>
    public class AnimationComponent : AbstractGameObjectComponent
    {
        /// <summary>
        /// Sprite data that this sprite is using
        /// </summary>
        private SpriteData mRootSpriteAnimation;        // TODO: Redo this class
        private List<SpriteData> mSpriteAnimations;

        private AnimationData mCurrentAnimation;

        /// <summary>
        /// The current frame being animated
        /// </summary>
        public int mCurrentFrame { get; private set; }

        public string CurrentBaseAnimationName { get; private set; }
        public Direction CurrentAnimationDirection { get; private set; }

        /// <summary>
        /// The time that the current frame was first displayed
        /// </summary>
        public TimeSpan mFrameStartTime { get; private set; }

        /// <summary>
        /// True if an animation is being played, false if it is not. (False also means
        /// a current animation is frozen).
        /// </summary>
        public bool mIsAnimating { get; private set; }

        /// <summary>
        /// Controls what happens when the current animation completes
        /// </summary>
        public AnimationEndingAction mAnimationEndingAction { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AnimationComponent()
        {
            Reset();
        }

        private void Reset()
        {
            mRootSpriteAnimation = null;
            mSpriteAnimations = new List<SpriteData>();
            mCurrentFrame = 0;
            mFrameStartTime = TimeSpan.MinValue;
            mIsAnimating = false;
            mAnimationEndingAction = AnimationEndingAction.Stop;
        }

        public void AddSpriteAnimation( SpriteData spriteData )
        {
            // Mark the root sprite
            if ( mRootSpriteAnimation == null )
            {
                mRootSpriteAnimation = spriteData;
            }

            mSpriteAnimations.Add( spriteData );

            // Update the sprite component with our new animation
            SpriteComponent sprite = Owner.GetComponent<SpriteComponent>();
            Debug.Assert( sprite != null, "GameObject must have a sprite component" );

            sprite.AddSprite( spriteData );
        }

        private void UpdateSpriteComponent()
        {
            // Update our sprite info
            SpriteComponent sprite = Owner.GetComponent<SpriteComponent>();
            Debug.Assert( sprite != null, "GameObject must have a sprite component" );

            // Apply current animation to all our sprites
            if ( mRootSpriteAnimation != null )
            {
                string animation = ( mCurrentAnimation == null ? mRootSpriteAnimation.DefaultAnimationName : mCurrentAnimation.Name );

                for ( int layer = 0; layer < mSpriteAnimations.Count; ++layer )
                {
                    sprite.SetLayer( layer,
                                     mSpriteAnimations[layer].Texture,
                                     mSpriteAnimations[layer].Animations[animation].Frames[mCurrentFrame],
                                     mSpriteAnimations[layer].OriginOffset );
                }
            }
        }

        /// <summary>
        /// Plays the requested animation
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        /// <param name="endingAction">Action to take when the animation ends</param>
        public void PlayAnimation( string baseAnimationName,
                                   Direction direction,
                                   AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset )
        {
            string animationName    = baseAnimationName + Enum.GetName( typeof( Direction ), direction );
            AnimationData animation = null;

            // Attempt to retrieve the requested animation. If the animation exists, go ahead and
            // start playing it
            Debug.Assert( mRootSpriteAnimation != null, "Missing sprite aniation data" );

            if ( mRootSpriteAnimation.Animations.TryGetValue( animationName, out animation ) )
            {
                mCurrentAnimation         = animation;
                mCurrentFrame             = 0;
                mFrameStartTime           = TimeSpan.MinValue;
                CurrentBaseAnimationName  = baseAnimationName;
                CurrentAnimationDirection = direction;
                
                mAnimationEndingAction = endingAction;
                mIsAnimating = true;

                UpdateSpriteComponent();
            }
            else
            {
                throw new AnimationException( "Failed to find animation named " + animationName,
                                              mRootSpriteAnimation.Name,
                                              animationName,
                                              0 );
            }
        }



        /// <summary>
        /// Play a requested animation and have it loop until interrupted
        /// </summary>
        /// <param name="baseAnimationName">Name of the animation to play</param>
        public void PlayAnimationLooping( string baseAnimationName, Direction direction )
        {
            PlayAnimation( baseAnimationName, direction, AnimationEndingAction.Loop );
        }

        /// <summary>
        /// Check if an animation is playing
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation( string animationName )
        {
            return ( CurrentBaseAnimationName == animationName );
        }

        public bool IsPlayingAnimation( string animationName, Direction direction )
        {
            return ( CurrentBaseAnimationName == animationName && CurrentAnimationDirection == direction );
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public override void Update( GameTime gameTime )
        {
            TimeSpan frameTime = TimeSpan.FromSeconds( 0.1 );       // this needs to go.. should be read in via XML per animation

            // Don't update if we are not visible
            if ( !Enabled )
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
                UpdateSpriteComponent();

                mFrameStartTime = gameTime.TotalGameTime;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Scott.Game.Graphics;
using Scott.Common;

namespace Scott.Game.Entity.Graphics
{
    /// <summary>
    /// The sprite component represents a sprite visible to the camera
    /// </summary>
    public class SpriteComponent : Component, IDrawable
    {
        class AnimationState
        {
            public int Frame;
            public string Name;
            public AnimationData Data;
            public Direction Direction;
            public TimeSpan FrameStart;
            public AnimationEndingAction Ending;

            public AnimationState()
            {
            }

            public void SwitchTo( AnimationData data,
                                  string animationName,
                                  Direction dir,
                                  AnimationEndingAction action )
            {
                Reset();

                Name = animationName;
                Data = data;
                Direction = dir;
                Ending = action;
                FrameStart = TimeSpan.MinValue;
            }

            public void Reset()
            {
                Name = String.Empty;
                Frame = 0;
                FrameStart = TimeSpan.MinValue;
            }
        }

        private AnimationState CurrentAnimation;
        private bool mIsIdleAnimation = false;

        private string mDefaultAnimationName;

        /// <summary>
        /// True if an animation is being played, false if it is not. (False also means
        /// a current animation is frozen).
        /// </summary>
        public bool IsAnimating { get; private set; }


        /// <summary>
        /// Sprite data that this sprite is using
        /// </summary>
        private SpriteData mRootSprite;

        private List<SpriteItem> mSpriteList;
        private Dictionary<string, SpriteItem> mSpriteTable;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
            CurrentAnimation = new AnimationState();

            mSpriteList = new List<SpriteItem>( 1 );
            mSpriteTable = new Dictionary<string, SpriteItem>();
        }

        public void AssignRootSprite( SpriteData spriteData )
        {
            mDefaultAnimationName = spriteData.DefaultAnimationName;
            mRootSprite           = spriteData;

            CurrentAnimation.SwitchTo(
                spriteData.Animations[mDefaultAnimationName],
                mDefaultAnimationName,
                spriteData.DefaultAnimationDirection,
                AnimationEndingAction.StopAndReset  );

            // Add the initial root sprite
            mSpriteList.Add( new SpriteItem( spriteData ) );
        }

        /// <summary>
        ///  Adds a child sprite to this sprite.
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="spriteData"></param>
        public void AddLayer( string layerName, SpriteData spriteData )
        {
            AddLayer( layerName, spriteData, true );
        }

        /// <summary>
        ///  Adds a child sprite to this sprite.
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="spriteData"></param>
        public void AddLayer( string layerName, SpriteData spriteData, bool enabled )
        {
            SpriteItem item = new SpriteItem( spriteData );
            item.Enabled = enabled;

            mSpriteList.Add( item );
            mSpriteTable.Add( layerName, item );

            SyncAllSprites();
        }

        /// <summary>
        ///  Enables or disables a sprite layer.
        /// </summary>
        /// <param name="layerName"></param>
        public void EnableLayer( string layerName, bool isEnabled )
        {
            SpriteItem item = null;

            if ( mSpriteTable.TryGetValue( layerName, out item ) )
            {
                item.Enabled = isEnabled;
            }
            else
            {
                throw new SpriteException( "No such sprite layer {0}".With( layerName ) );
            }
        }

        /// <summary>
        /// Plays the requested animation
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        /// <param name="endingAction">Action to take when the animation ends</param>
        public void PlayAnimation( string animationName,
                                   Direction direction,
                                   AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset )
        {
            AnimationData data = null;

            // Are we about to abort a currently playing animation?
            if ( IsAnimating )
            {
                AbortCurrentAnimation( false );
            }

            // Attempt to retrieve the requested animation. If the animation exists, go ahead and
            // start playing it
            Debug.Assert( mRootSprite != null, "Missing sprite animation data" );

            if ( mRootSprite.Animations.TryGetValue( animationName, out data ) )
            {
                CurrentAnimation.SwitchTo( data, animationName, direction, endingAction );
                IsAnimating = true;

                SyncAllSprites();
            }
            else
            {
                throw new AnimationException( "Failed to find animation named " + animationName,
                                              mRootSprite.Name,
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
            return ( IsAnimating && animationName == CurrentAnimation.Name );
        }

        /// <summary>
        /// Check if an animation is playing
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation( string animationName, Direction direction )
        {
            return ( IsAnimating &&
                     animationName == CurrentAnimation.Name &&
                     direction == CurrentAnimation.Direction );
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public override void Update( GameTime gameTime )
        {
            // Don't update if we are not visible or not playing an animation.
            if ( !Enabled || !IsAnimating )
            {
                return;
            }

            // Check if this is the first time we've updated this sprite. If so, initialize our
            // animation values for the next call to update. Otherwise proceed as normal
            if ( CurrentAnimation.FrameStart == TimeSpan.MinValue )        // start the clock
            {
                CurrentAnimation.FrameStart = gameTime.TotalGameTime;
            }

            // How long does each frame last? When did we last flip a frame?
            TimeSpan lastFrameTime = CurrentAnimation.FrameStart;
            TimeSpan lengthOfFrame = TimeSpan.FromSeconds( CurrentAnimation.Data.FrameTime );

            // Update the current frame index by seeing how much time has passed, and then
            // moving to the correct frame.
            lastFrameTime = lastFrameTime.Add( lengthOfFrame );     // account for the current frame.

            while ( lastFrameTime <= gameTime.TotalGameTime )
            {
                // Move to the next frame.
                CurrentAnimation.Frame += 1;

                // Are we at the end of this animation?
                if ( CurrentAnimation.Frame == CurrentAnimation.Data.FrameCount )
                {
                    switch ( CurrentAnimation.Ending )
                    {
                        case AnimationEndingAction.Loop:
                            CurrentAnimation.Frame = 0;
                            OnAnimationLooped( CurrentAnimation );
                            break;

                        case AnimationEndingAction.Stop:
                            IsAnimating = false;
                            OnAnimationComplete( CurrentAnimation );
                            break;

                        case AnimationEndingAction.StopAndReset:
                            IsAnimating = false;
                            OnAnimationComplete( CurrentAnimation );
                            break;
                    }
                }

                // Update all of the sprites to reflect our new animation frame
                SyncAllSprites();

                // Update the time when this animation frame was first displayed
                CurrentAnimation.FrameStart = gameTime.TotalGameTime;
                lastFrameTime = lastFrameTime.Add( lengthOfFrame ); 
            }
        }

        /// <summary>
        /// Synchronizes all sprites contained in this sprite component, and makes them match the
        /// currently playing animation.
        /// </summary>
        private void SyncAllSprites()
        {
            int frameIndex      = CurrentAnimation.Frame;
            Direction direction = CurrentAnimation.Direction;
            string name         = CurrentAnimation.Name;

            foreach ( SpriteItem item in mSpriteList )
            {
                if ( item.Enabled )
                {
                    AnimationData animation = item.SpriteData.Animations[name];
                    item.AtlasRect = animation.GetSpriteRectFor( direction, frameIndex );
                }
            }
        }

        /// <summary>
        /// Send update to the sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw( GameTime gameTime )
        {
            if ( Enabled )
            {
                for ( int i = 0; i < mSpriteList.Count; ++i )
                {
                    SpriteItem item = mSpriteList[i];

                    if ( !item.Enabled )
                    {
                        continue;
                    }

                    GameRoot.Renderer.Draw( item.Texture,
                                            item.AtlasRect,
                                            item.OriginOffset + Owner.Transform.Position );
                }
            }
        }

        /// <summary>
        ///  Kills the current animation.
        /// </summary>
        /// <param name="shouldSwitchToIdle"></param>
        private void AbortCurrentAnimation( bool shouldSwitchToIdle )
        {
            IsAnimating = false;
            OnAnimationAborted( CurrentAnimation );
        }

        /// <summary>
        ///  Called when an animation starts playing.
        /// </summary>
        /// <param name="animation"></param>
        private void OnAnimationStarted( AnimationState animation )
        {

        }

        /// <summary>
        ///  Called if an animation is terminated before it normally completes.
        /// </summary>
        /// <param name="animation"></param>
        private void OnAnimationAborted( AnimationState animation )
        {

        }

        /// <summary>
        ///  Called when an animation completes a cycles and is about to begin animating in a loop
        ///  again.
        /// </summary>
        /// <param name="animation"></param>
        private void OnAnimationLooped( AnimationState animation )
        {

        }

        /// <summary>
        ///  Called when an animation completes.
        /// </summary>
        /// <param name="animation"></param>
        private void OnAnimationComplete( AnimationState animation )
        {
            // Start our idle animation.
            PlayAnimationLooping( mDefaultAnimationName, CurrentAnimation.Direction );
        }

        private class SpriteItem
        {
            public SpriteData SpriteData;
            public Texture2D Texture;
            public Rectangle AtlasRect;
            public Vector2 OriginOffset;
            public bool Enabled;

            public SpriteItem( SpriteData sprite )
            {
                SpriteData  = sprite;
                Texture      = sprite.Texture;
                OriginOffset = sprite.OriginOffset;
                Enabled      = true;
            }
        }
    }
}

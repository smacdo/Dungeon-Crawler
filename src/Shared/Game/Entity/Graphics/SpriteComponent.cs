using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.GameContent;
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
        /// <summary>
        /// The animation that is currently playing
        /// </summary>
        public AnimationData CurrentAnimation { get; set; }

        /// <summary>
        /// The current frame being animated
        /// </summary>
        public int CurrentFrame { get; private set; }

        /// <summary>
        /// Name of the animation that is currently playing
        /// </summary>
        public string CurrentAnimationName
        {
            get
            {
                return CurrentAnimation.Name;
            }
        }

        /// <summary>
        /// Direction of the animation that is currently playing
        /// </summary>
        public Direction CurrentAnimationDirection { get; private set; }

        /// <summary>
        /// The time that the current frame was first displayed
        /// </summary>
        public TimeSpan FrameStartTime { get; private set; }

        /// <summary>
        /// True if an animation is being played, false if it is not. (False also means
        /// a current animation is frozen).
        /// </summary>
        public bool IsAnimating { get; private set; }

        /// <summary>
        /// Controls what happens when the current animation completes
        /// </summary>
        public AnimationEndingAction AnimationEndingAction { get; private set; }

        /// <summary>
        /// Sprite data that this sprite is using
        /// </summary>
        private SpriteData mRootSpriteAnimation;

        private List<SpriteItem> mSpriteList;
        private Dictionary<string, SpriteItem> mSpriteTable;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
        }

        public void AssignRootSprite( SpriteData spriteData )
        {
            CurrentAnimation = spriteData.Animations[spriteData.DefaultAnimationName];
            CurrentFrame = 0;
            CurrentAnimationDirection = spriteData.DefaultAnimationDirection;
            FrameStartTime = TimeSpan.MinValue;
            IsAnimating = false;
            AnimationEndingAction = AnimationEndingAction.Stop;

            mRootSpriteAnimation = spriteData;

            mSpriteList = new List<SpriteItem>( 1 );
            mSpriteTable = new Dictionary<string, SpriteItem>();

            // Add the initial root sprite
            mSpriteList.Add( new SpriteItem( spriteData,
                                             CurrentAnimationDirection,
                                             CurrentAnimationName,
                                             CurrentFrame ) );
        }


        /// <summary>
        ///  Adds a child sprite to this sprite.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spriteData"></param>
        public void AddLayer( string layerName, SpriteData spriteData )
        {
            AddLayer( layerName, spriteData, true );
        }

        /// <summary>
        ///  Adds a child sprite to this sprite.
        /// </summary>
        /// <param name="name"></param>
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
            AnimationData animation = null;

            // Attempt to retrieve the requested animation. If the animation exists, go ahead and
            // start playing it
            Debug.Assert( mRootSpriteAnimation != null, "Missing sprite animation data" );

            if ( mRootSpriteAnimation.Animations.TryGetValue( animationName, out animation ) )
            {
                CurrentAnimation          = animation;
                CurrentFrame              = 0;
                CurrentAnimationDirection = direction;
                FrameStartTime            = TimeSpan.MinValue;
                IsAnimating               = true;
                AnimationEndingAction     = endingAction;

                SyncAllSprites();
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
            return CurrentAnimationName == animationName;
        }

        /// <summary>
        /// Check if an animation is playing
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation( string animationName, Direction direction )
        {
            return ( CurrentAnimationName == animationName && CurrentAnimationDirection == direction );
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public override void Update( GameTime gameTime )
        {
            // Don't update if we are not visible
            if ( !Enabled )
            {
                return;
            }

            // How long does each frame last?
            TimeSpan frameTime = TimeSpan.FromSeconds( CurrentAnimation.FrameTime );

            // Check if this is the first time we've updated this sprite. If so, initialize our
            // animation values for the next call to update. Otherwise proceed as normal
            if ( FrameStartTime == TimeSpan.MinValue )        // start the clock
            {
                FrameStartTime = gameTime.TotalGameTime;
            }
            else if ( IsAnimating && FrameStartTime.Add( frameTime ) <= gameTime.TotalGameTime )
            {
                // Are we at the end of this animation?
                if ( CurrentFrame + 1 == CurrentAnimation.FrameCount )
                {
                    switch ( AnimationEndingAction )
                    {
                        case AnimationEndingAction.Loop:
                            CurrentFrame = 0;
                            break;

                        case AnimationEndingAction.Stop:
                            IsAnimating = false;
                            break;

                        case AnimationEndingAction.StopAndReset:
                            CurrentFrame = 0;
                            IsAnimating = false;
                            break;
                    }
                }
                else
                {
                    // We're not at the end... just increment the frame and continue
                    CurrentFrame++;
                }

                // Update all of the sprite items to reflect our new animation frame
                SyncAllSprites();

                // Update the time when this animation frame was first displayed
                FrameStartTime = gameTime.TotalGameTime;
            }
        }

        /// <summary>
        /// Synchronizes all sprites contained in this sprite component, and makes them match the
        /// currently playing animation.
        /// </summary>
        private void SyncAllSprites()
        {
            foreach ( SpriteItem item in mSpriteList )
            {
                if ( item.Enabled )
                {
                    AnimationData animation = item.SourceSprite.Animations[CurrentAnimationName];
                    item.AtlasSpriteRect = animation.GetSpriteRectFor( CurrentAnimationDirection, CurrentFrame );
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

                    GameRoot.Renderer.Draw( Layer.Default,
                                            item.AtlasTexture,
                                            item.AtlasSpriteRect,
                                            item.OriginOffset + Owner.Position );
                }
            }
        }

        private class SpriteItem
        {
            public SpriteData SourceSprite;
            public Texture2D AtlasTexture;
            public Rectangle AtlasSpriteRect;
            public Vector2 OriginOffset;
            public bool Enabled;


            public SpriteItem( SpriteData sprite )
            {
                SourceSprite = sprite;
                AtlasTexture = sprite.Texture;
                OriginOffset = sprite.OriginOffset;
                Enabled = false;
            }

            public SpriteItem( SpriteData sprite,
                               Direction currentDirection,
                               string currentAnimation,
                               int currentFrame )
            {
                AnimationData animation = sprite.Animations[currentAnimation];

                SourceSprite    = sprite;
                AtlasTexture    = sprite.Texture;
                AtlasSpriteRect = animation.GetSpriteRectFor( currentDirection, currentFrame );
                OriginOffset    = sprite.OriginOffset;
                Enabled         = true;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scott.Dungeon.Graphics;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// An animation component manages a sprite's animation
    /// TODO: Pull the animation code out of Sprite and put it in here
    /// </summary>
    public class AnimationComponent : AbstractGameObjectComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AnimationComponent()
        {
        }

        /// <summary>
        /// Plays the requested animation
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        /// <param name="endingAction">Action to take when the animation ends</param>
        public void PlayAnimation( string animationName, AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset )
        {
            SpriteComponent spriteComponent = Owner.GetComponent<SpriteComponent>();
            spriteComponent.Sprite.PlayAnimation( animationName, endingAction );
        }

        /// <summary>
        /// Play a requested animation and have it loop until interrupted
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        public void PlayAnimationLooping( string animationName )
        {
            PlayAnimation( animationName, AnimationEndingAction.Loop );
        }

        /// <summary>
        /// Check if an animation is playing
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation( string animationName )
        {
            SpriteComponent spriteComponent = Owner.GetComponent<SpriteComponent>();
            return spriteComponent.Sprite.IsPlayingAnimation( animationName );
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public override void Update( GameTime gameTime )
        {
            if ( Enabled )
            {
                SpriteComponent spriteComponent = Owner.GetComponent<SpriteComponent>();
                spriteComponent.Sprite.Update( gameTime );
            }
        }
    }
}

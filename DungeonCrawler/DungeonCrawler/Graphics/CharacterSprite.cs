using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.Graphics
{
    /// <summary>
    /// A character sprite is a complex collection of sprites that are designed to
    /// work together to provide a standard set of animations along with layered 
    /// equipment.
    /// </summary>
    public class CharacterSprite
    {
        public Sprite BodySprite { get; set; }
        public Sprite WeaponSprite { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bodySprite"></param>
        /// <param name="weaponSprite"></param>
        public CharacterSprite( Sprite bodySprite, Sprite weaponSprite )
        {
            BodySprite = bodySprite;
            WeaponSprite = weaponSprite;
        }

        /// <summary>
        /// Plays the requested animation
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        /// <param name="endingAction">Action to take when the animation ends</param>
        public void PlayAnimation( string animationName, AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset )
        {
            // Might want to filter all valid animations into a list to make this less
            // obnoxious
            if ( BodySprite != null )
            {
                BodySprite.PlayAnimation( animationName, endingAction );
            }

            if ( WeaponSprite != null )
            {
                WeaponSprite.PlayAnimation( animationName, endingAction );
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

        /// <summary>
        /// Check if an animation is playing
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation( string animationName )
        {
            // Keep checking until we find a non-null animation
            if ( BodySprite != null )
            {
                return BodySprite.IsPlayingAnimation( animationName );
            }
            else if ( WeaponSprite != null )
            {
                return WeaponSprite.IsPlayingAnimation( animationName );
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public void Update( GameTime gameTime )
        {
            // Might want to filter all valid animations into a list to make this less
            // obnoxious
            if ( BodySprite != null )
            {
                BodySprite.Update( gameTime );
            }

            if ( WeaponSprite != null )
            {
                WeaponSprite.Update( gameTime );
            }
        }
    }
}

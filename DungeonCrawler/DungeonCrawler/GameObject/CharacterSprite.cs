using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scott.Dungeon.Graphics;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// A character sprite is a complex collection of sprites that are designed to
    /// work together to provide a standard set of animations along with layered 
    /// equipment.
    /// </summary>
    public class CharacterSprite : IGameObjectComponent
    {
        public enum SubSpriteIndex
        {
            Back = 0,
            Body,
            Head,
            Torso,
            Hands,
            Shoulder,
            Belt,
            Legs,
            Feet,
            Weapon,
            Max
        }

        public List<Sprite> SubSprites { get; private set; }

        public Sprite Body
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Body];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Body] = value;
            }
        }

        public Sprite Head
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Head];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Head] = value;
            }
        }

        public Sprite Weapon
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Weapon];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Weapon] = value;
            }
        }

        public Sprite Torso
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Torso];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Torso] = value;
            }
        }

        public Sprite Back
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Back];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Back] = value;
            }
        }

        public Sprite Hands
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Hands];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Hands] = value;
            }
        }

        public Sprite Shoulder
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Shoulder];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Shoulder] = value;
            }
        }

        public Sprite Belt
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Belt];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Belt] = value;
            }
        }

        public Sprite Legs
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Legs];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Legs] = value;
            }
        }

        public Sprite Feet
        {
            get
            {
                return SubSprites[(int) SubSpriteIndex.Feet];
            }
            set
            {
                SubSprites[(int) SubSpriteIndex.Feet] = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterSprite()
        {
            // Allocate a list of sprites to render, and set each entry to null
            int spriteIndex = (int) SubSpriteIndex.Max;
            SubSprites = new List<Sprite>( spriteIndex );

            for ( int i = 0; i < spriteIndex; ++i )
            {
                SubSprites.Add( null );
            }
        }

        /// <summary>
        /// Plays the requested animation
        /// </summary>
        /// <param name="animationName">Name of the animation to play</param>
        /// <param name="endingAction">Action to take when the animation ends</param>
        public void PlayAnimation( string animationName, AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset )
        {
            for ( int i = 0; i < SubSprites.Count; ++i )
            {
                Sprite s = SubSprites[i];

                if ( s != null && s.Visible )
                {
                    s.PlayAnimation( animationName, endingAction );
                }
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
            // Check if any of the sprites are playing an animation
            bool isPlayingAnimation = false;

            for ( int i = 0; i < SubSprites.Count && (!isPlayingAnimation); ++i )
            {
                Sprite s = SubSprites[i];
                isPlayingAnimation = ( s != null && s.IsPlayingAnimation( animationName ) );
            }

            return isPlayingAnimation;
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <param name="gameTime">Current rendering time</param>
        public override void Update( GameTime gameTime )
        {
            if ( !Enabled )
            {
                return;
            }

            // Update all sub-sprites
            for ( int i = 0; i < SubSprites.Count; ++i )
            {
                Sprite s = SubSprites[i];

                if ( s != null )
                {
                    s.Update( gameTime );
                    s.Draw( Owner.Position );
                }
            }
        }
    }
}

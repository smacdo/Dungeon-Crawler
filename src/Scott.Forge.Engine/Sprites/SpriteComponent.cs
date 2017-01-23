/*
 * Copyright 2012-2015 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  The sprite component represents a sprite visible to the camera. Supports a single renderable sprite, or a
    ///  multiple sprites composited together.
    /// </summary>
    public class SpriteComponent : Component
    {
        private SpriteDefinition[] mSprites;
        private Rectangle[] mSpriteRects;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
            mSprites = new SpriteDefinition[1];
            mSpriteRects = new Rectangle[1];
        }

        /// <summary>
        ///  Get the current animation frame index.
        /// </summary>
        /// <remarks>
        ///  This should be changed only by animation routines.
        /// </remarks>
        public int AnimationFrameIndex { get; internal set; }

        /// <summary>
        ///  Get the time that the current animation frame was first shown.
        /// </summary>
        public TimeSpan AnimationFrameStartTime { get; internal set; }

        /// <summary>
        ///  Get or set sprite animation set definition.
        /// </summary>
        public AnimationSetDefinition Animations { get; internal set; }

        /// <summary>
        ///  Get the current sprite animation definition.
        /// </summary>
        public AnimationDefinition CurrentAnimation { get; internal set; }

        /// <summary>
        ///  Get the current sprite direction.
        /// </summary>
        public DirectionName Direction { get; internal set; }

        /// <summary>
        ///  Get the action to take when an animation reaches the last frame.
        /// </summary>
        public AnimationEndingAction EndingAction { get; internal set; }

        /// <summary>
        ///  Get it the sprite is playing an animation.
        /// </summary>
        public bool IsAnimating
        {
            get { return CurrentAnimation != null; }
        }

        /// <summary>
        ///  Get list of composite sprites.
        /// </summary>
        internal SpriteDefinition[] Sprites { get { return mSprites; } }

        /// <summary>
        ///  Get list of composite sprites.
        /// </summary>
        internal Rectangle[] SpriteRects { get { return mSpriteRects; } }

        /// <summary>
        ///  Set a single sprite to be displayed.
        /// </summary>
        /// <param name="spriteDefinition">Sprite definition.</param>
        public void SetSprite(AnimatedSpriteDefinition spriteDefinition)
        {
            if (spriteDefinition == null)
            {
                throw new ArgumentNullException("spriteDefinition");
            }

            Animations = spriteDefinition.Animations;
            SetLayer(0, spriteDefinition.Sprite);
        }

        public void SetMultipleSpriteCount(int newCount)
        {
            if (newCount < 1)
            {
                throw new ArgumentException("Must have at least one sprite", "newCount");
            }

            Array.Resize(ref mSprites, newCount);
            Array.Resize(ref mSpriteRects, newCount);
        }

        /// <summary>
        ///  Adds a sprite for composite rendering.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="spriteData">Sprite definition.</param>
        /// <param name="enabled">True if layer is enabled by default, false otherwise.</param>
        public void SetLayer(int layerIndex, SpriteDefinition spriteDefinition)
        {
            if (layerIndex < 0 || layerIndex >= Sprites.Length)
            {
                throw new ArgumentOutOfRangeException("layerIndex");
            }

            if (spriteDefinition == null)
            {
                throw new ArgumentNullException("spriteDefinition");
            }

            Sprites[layerIndex] = spriteDefinition;
            mSpriteRects[layerIndex] = new Rectangle(
                (int) Sprites[layerIndex].StartingOffset.X,
                (int) Sprites[layerIndex].StartingOffset.Y,
                (int) Sprites[layerIndex].Size.Width,
                (int) Sprites[layerIndex].Size.Height);
        }

        /// <summary>
        ///  Plays the requested animation.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="direction">Sprite direction.</param>
        /// <param name="endingAction">Action to take when animation ends.</param>
        public void PlayAnimation(
            string animationName,
            DirectionName direction,
            AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset)
        {
            // Are we about to abort a currently playing animation?
            if (IsAnimating)
            {
                AbortCurrentAnimation();
            }

            // Look up the requested animation definition and begin playing it.
            AnimationDefinition animation = null;

            if (!Animations.Animations.TryGetValue(animationName, out animation))
            {
                throw new ArgumentException("Animation not found", "animation");
            }

            CurrentAnimation = animation;
            Direction = direction;
            EndingAction = endingAction;
            AnimationFrameStartTime = TimeSpan.MinValue;
        }

        /// <summary>
        /// Play a requested animation and have it loop until interrupted
        /// </summary>
        /// <param name="baseAnimationName">Name of the animation to play</param>
        public void PlayAnimationLooping(string baseAnimationName, DirectionName direction)
        {
            PlayAnimation(baseAnimationName, direction, AnimationEndingAction.Loop);
        }

        /// <summary>
        ///  Update cached sprite atlas rects.
        /// </summary>
        internal void SetAtlasRects(Vector2 atlasOffset)
        {
            for (int i = 0; i < mSpriteRects.Length; i++)
            {
                mSpriteRects[i] = new Rectangle(
                    (int) atlasOffset.X,
                    (int) atlasOffset.Y,
                    (int) Sprites[i].Size.Width,
                    (int) Sprites[i].Size.Height);
            }
        }

        /// <summary>
        ///  Kills the current animation.
        /// </summary>
        public void AbortCurrentAnimation()
        {
            CurrentAnimation = null;
        }
    }
}

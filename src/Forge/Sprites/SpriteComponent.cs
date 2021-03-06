﻿/*
 * Copyright 2012-2017 Scott MacDonald
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
using Forge.GameObjects;

namespace Forge.Sprites
{
    /// <summary>
    ///  The sprite component represents a sprite visible to the camera. Supports a single renderable sprite, or a
    ///  multiple sprites composited together.
    /// </summary>
    public class SpriteComponent : Component
    {
        private SpriteDefinition[] mSprites;
        private RectF[,] mSpriteRects;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
            mSprites = new SpriteDefinition[1];
            mSpriteRects = new RectF[1, Constants.DirectionCount];
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="sprite">Sprite definition to use when initializing.</param>
        public SpriteComponent(SpriteDefinition sprite)
            : this()
        {
            mSprites[0] = sprite ?? throw new ArgumentNullException(nameof(sprite));

            for (int i = 0; i < Constants.DirectionCount; i++)
            {
                mSpriteRects[0, i] = new RectF(
                    left: sprite.AtlasPosition.X,
                    top: sprite.AtlasPosition.Y,
                    width: sprite.Size.Width,
                    height: sprite.Size.Height);
            }
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="animation"></param>
        public SpriteComponent(AnimatedSpriteDefinition sprite)
            : this(sprite?.Sprite)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            SetSprite(sprite);
        }

        /// <summary>
        ///  Event notification for when sprite completes an animation.
        /// </summary>
        public EventHandler<AnimationCompletedEventArgs> AnimationCompleted;

        /// <summary>
        ///  Notification when a data driven animation event is fired.
        /// </summary>
        public EventHandler<AnimationEvent> AnimationEventFired;

        /// <summary>
        ///  Get or set the current animation frame index.
        /// </summary>
        /// <remarks>
        ///  This field should only be set by the Sprite componenet processor.
        /// </remarks>
        public int AnimationFrameIndex { get; internal set; }

        /// <summary>
        ///  Get or set the number of seconds that the current animation has been playing.
        /// </summary>
        /// <remarks>
        ///  This field should only be set by the Sprite componenet processor.
        /// </remarks>
        public double AnimationSecondsActive { get; internal set; }

        /// <summary>
        ///  Get or set the number of seconds that the current animation frame has been shown.
        /// </summary>
        /// <remarks>
        ///  This field should only be set by the Sprite componenet processor.
        /// </remarks>
        public double AnimationFrameSecondsActive { get; internal set; }

        /// <summary>
        ///  Get or set sprite animation set definition.
        /// </summary>
        public AnimationSetDefinition Animations { get; internal set; }

        /// <summary>
        ///  Get or set the current sprite animation definition.
        /// </summary>
        /// <remarks>
        ///  This field should only be set by the Sprite componenet processor.
        /// </remarks>
        public AnimationDefinition CurrentAnimation { get; internal set; }

        /// <summary>
        ///  Get or set sprite rotational render method.
        /// </summary>
        public SpriteRotationRenderMethod RotationRenderMethod { get; set; }

        /// <summary>
        ///  Get the action to take when an animation reaches the last frame.
        /// </summary>
        public AnimationEndingAction EndingAction { get; internal set; }

        /// <summary>
        ///  Get or set the next requestion animation.
        /// </summary>
        internal AnimationRequest? RequestedAnimation { get; set; }

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
        /// <remarks>
        ///  [LayerIndex, Direction].
        /// </remarks>
        internal RectF[,] SpriteRects { get { return mSpriteRects; } }

        /// <summary>
        ///  Set a single sprite to be displayed.
        /// </summary>
        /// <param name="spriteDefinition">Sprite definition.</param>
        public void SetSprite(AnimatedSpriteDefinition spriteDefinition)
        {
            if (spriteDefinition == null)
            {
                throw new ArgumentNullException(nameof(spriteDefinition));
            }

            Animations = spriteDefinition.Animations;
            SetLayer(0, spriteDefinition.Sprite);
        }

        /// <summary>
        ///  Change the number of sprites that this sprite component can hold.
        /// </summary>
        /// <remarks>
        ///  Each sprite that is added to the sprite component must use the same animation definition, or udefined
        ///  results will occur.
        /// </remarks>
        /// <param name="newCount">Number of sprites.</param>
        public void SetMultipleSpriteCount(int newCount)
        {
            if (newCount < 1)
            {
                throw new ArgumentException("Must have at least one sprite", nameof(newCount));
            }

            Array.Resize(ref mSprites, newCount);
            mSpriteRects = new RectF[newCount, Constants.DirectionCount];
        }

        /// <summary>
        ///  Set a sprite for composite rendering.
        /// </summary>
        /// <remarks>
        ///  Each sprite that is added to the sprite component must use the same animation definition, or udefined
        ///  results will occur.
        /// </remarks>
        /// <param name="layerIndex">Layer index.</param>
        /// <param name="spriteData">Sprite definition.</param>
        public void SetLayer(int layerIndex, SpriteDefinition spriteDefinition)
        {
            if (layerIndex < 0 || layerIndex >= Sprites.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(layerIndex));
            }

            Sprites[layerIndex] = spriteDefinition ?? throw new ArgumentNullException(nameof(spriteDefinition));
            
            // Initialize sprite rects to default (first frame) atlas position. No animation is assumed at this point.
            for (int i = 0; i < Constants.DirectionCount; i++)
            {
                mSpriteRects[layerIndex, i] = new RectF(
                    Sprites[layerIndex].AtlasPosition,
                    Sprites[layerIndex].Size);
            }   
        }

        /// <summary>
        ///  Plays the requested animation.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="endingAction">Action to take when animation ends.</param>
        public void PlayAnimation(
            string animationName,
            AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset)
        {
            RequestedAnimation = new AnimationRequest
            {
                Name = animationName,
                EndingAction = endingAction
            };
        }

        /// <summary>
        /// Play a requested animation and have it loop until interrupted
        /// </summary>
        /// <param name="baseAnimationName">Name of the animation to play</param>
        public void PlayAnimationLooping(string baseAnimationName)
        {
            PlayAnimation(baseAnimationName, AnimationEndingAction.Loop);
        }

        internal struct AnimationRequest
        {
            public string Name;
            public AnimationEndingAction EndingAction;
        }
    }

    /// <summary>
    ///  Defines how a sprite should be drawn when it is rotated.
    /// </summary>
    public enum SpriteRotationRenderMethod
    {
        Default,        /// Default (rotation)
        Rotated,        /// Draw sprite image rotated.
        FourWay,        /// Select image from four cardinal directions, do not rotate.
    }

    /// <summary>
    ///  Animation completed argument details.
    /// </summary>
    public struct AnimationCompletedEventArgs
    {
        /// <summary>
        ///  Get or set the name of the animation that completed.
        /// </summary>
        public string AnimationName;
    }
}

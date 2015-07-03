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
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  The sprite component represents a sprite visible to the camera. Supports a single renderable sprite, or a
    ///  multiple sprites composited together.
    /// </summary>
    /// <remarks>
    ///  TODO: Consider breaking this class apart into SpriteComponent and CompositeSpriteComponent.
    /// </remarks>
    public class SpriteComponent : Component
    {
        // Sprite data that this sprite is using
        private Sprite mRootSprite;

        private List<Sprite> mSpriteList;
        private Dictionary<string, Sprite> mSpriteLayers;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpriteComponent()
        {
            mSpriteList = new List<Sprite>(1);
        }

        /// <summary>
        ///  Set a single sprite to be displayed.
        /// </summary>
        /// <param name="spriteData">Sprite definition.</param>
        public void SetSprite(SpriteDefinition spriteData)
        {
            mRootSprite = new Sprite(spriteData);
            mSpriteList.Add(mRootSprite);
        }

        /// <summary>
        ///  Adds a sprite for composite rendering.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="spriteData">Sprite definition.</param>
        /// <param name="enabled">True if layer is enabled by default, false otherwise.</param>
        public void AddLayer(
            string layerName,
            SpriteDefinition spriteData,
            bool enabled = true)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                throw new ArgumentNullException("layerName");
            }

            // Initialize layer dictionary if needed.
            if (mSpriteLayers == null)
            {
                mSpriteLayers = new Dictionary<string, Sprite>();
            }

            var sprite = new Sprite(spriteData);
            sprite.Enabled = enabled;

            mSpriteList.Add(sprite);
            mSpriteLayers.Add(layerName, sprite);

            // TODO: Make sure sprite is playing animation in tune with other sprites. Might be tricky.
        }

        /// <summary>
        ///  Enable or disable a sprite layer.
        /// </summary>
        /// <param name="layerName">Name of layer to use.</param>
        /// <param name="isEnabled">True to enable the layer, false otherwise.</param>
        public void EnableLayer(string layerName, bool isEnabled)
        {
            Sprite sprite = null;

            if (!mSpriteLayers.TryGetValue(layerName, out sprite))
            {
                throw new SpriteComponentLayerNotFound(layerName);
            }

            sprite.Enabled = isEnabled;
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
            if (mRootSprite != null)
            {
                mRootSprite.PlayAnimation(animationName, direction, endingAction);
            }
            else
            {
                for (int i = 0; i < mSpriteList.Count; ++i)
                {
                    if (mSpriteList[i].Enabled)
                    {
                        mSpriteList[i].PlayAnimation(animationName, direction, endingAction);
                    }
                }
            }
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
        /// Check if an animation is playing.
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation(string animationName, DirectionName direction)
        {
            // TODO: Needs to handle multi-layered situations.
            return (
                mRootSprite.IsAnimating &&
                animationName == mRootSprite.CurrentAnimation.Name &&
                direction == mRootSprite.Direction);
        }

        /// <summary>
        /// Updates the sprite's animation
        /// </summary>
        /// <remarks>
        ///  Move this to SpriteProcessor.
        /// </remarks>
        /// <param name="gameTime">Current rendering time</param>
        public void Update(double currentTime, double deltaTime)
        {
            for (int i = 0; i < mSpriteList.Count; ++i)
            {
                if (mSpriteList[i].Enabled)
                {
                    mSpriteList[i].Update(currentTime, deltaTime);
                }
            }
        }

        /// <summary>
        /// Send update to the sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(double currentTime, double deltaTime)
        {
            var origin = Owner.Transform.Position;

            for (int i = 0; i < mSpriteList.Count; ++i)
            {
                if (mSpriteList[i].Enabled)
                {
                    mSpriteList[i].Draw(origin, currentTime, deltaTime);
                }
            }
        }        
    }
}

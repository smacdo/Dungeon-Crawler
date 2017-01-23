/*
 * Copyright 2012-2017 Scott MacDonald.
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

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  A sprite is a 2d image that can hold one or more animations.
    ///  TODO: Delete once this folded into SpriteComponent.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        ///  Instantiate a new Sprite instance.
        /// </summary>
        /// <param name="definition">Sprite definition.</param>
        public Sprite(AnimatedSpriteDefinition definition)
            : this(definition != null ? definition.Sprite : null, definition != null ? definition.Animations : null)
        {
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="sprite">Sprite definition.</param>
        /// <param name="animations">Animation sets definition.</param>
        public Sprite(SpriteDefinition sprite, AnimationSetDefinition animations)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException("sprite");
            }

            if (animations == null)
            {
                throw new ArgumentNullException("animations");
            }

            Animations = animations;
            Definition = sprite;

            AtlasRect = new Rectangle(
                (int) Definition.StartingOffset.X,
                (int) Definition.StartingOffset.Y,
                (int) Definition.Size.Width,
                (int) Definition.Size.Height);
        }

        /// <summary>
        ///  Get the current animation frame index.
        /// </summary>
        /// <remarks>
        ///  This should be changed only by animation routines.
        /// </remarks>
        public int AnimationFrameIndex { get; private set; }

        /// <summary>
        ///  Get the time that the current animation frame was first shown.
        /// </summary>
        public TimeSpan AnimationFrameStartTime;

        /// <summary>
        ///  Get the texture atlas rendering rectangle.
        /// </summary>
        public Rectangle AtlasRect { get; private set; }
        
        /// <summary>
        ///  Get or set sprite animation set definition.
        /// </summary>
        public AnimationSetDefinition Animations { get; private set; }

        /// <summary>
        ///  Get the current sprite animation definition.
        /// </summary>
        public AnimationDefinition CurrentAnimation { get; private set; }

        /// <summary>
        ///  Get or set sprite definition.
        /// </summary>
        public SpriteDefinition Definition { get; private set; }
        
        /// <summary>
        ///  Get the current sprite direction.
        /// </summary>
        /// <remarks>
        ///  This should be changed only by animation routines.
        /// </remarks>
        public DirectionName Direction { get; private set; }

        /// <summary>
        ///  Get the action to take when an animation reaches the last frame.
        /// </summary>
        public AnimationEndingAction EndingAction;

        /// <summary>
        ///  Get if the sprite is playing an animation.
        /// </summary>
        /// <remarks>
        /// True if an animation is being played, false if it is not. (If the current animation is frozen then false
        /// is returned as well).
        /// </remarks>
        public bool IsAnimating { get; private set; }

        /// <summary>
        ///  Play an animation on this sprite.
        /// </summary>
        /// <param name="animationName">Name of the animation to play.</param>
        /// <param name="direction">2d direction to use for animation.</param>
        /// <param name="endingAction">Action to take when the animation finishes.</param>
        public void PlayAnimation(
            string animationName,
            DirectionName direction,
            AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset)
        {
            // Are we about to abort a currently playing animation?
            if (IsAnimating)
            {
                AbortCurrentAnimation(false);
            }

            // Look up the requested animation definition and begin playing it.
            AnimationDefinition animation = null;

            if (!Animations.Animations.TryGetValue(animationName, out animation))
            {
                throw new SpriteAnimationNotFoundException(this, animationName, direction);
            }

            CurrentAnimation = animation;
            Direction = direction;
            EndingAction = endingAction;
            AnimationFrameStartTime = TimeSpan.MinValue;

            IsAnimating = true;
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
        /// Check if an animation is playing
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation(string animationName)
        {
            return (IsAnimating && animationName == CurrentAnimation.Name);
        }

        /// <summary>
        /// Check if an animation is playing
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsPlayingAnimation(string animationName, DirectionName direction)
        {
            return (
                IsAnimating &&
                animationName == CurrentAnimation.Name &&
                direction == Direction);
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
            var totalGameTime = TimeSpan.FromSeconds(currentTime);

            // Don't update if we are not visible or not playing an animation.
            if (!IsAnimating)
            {
                return;
            }

            // Check if this is the first time we've updated this sprite. If so, initialize our
            // animation values for the next call to update. Otherwise proceed as normal
            if (AnimationFrameStartTime == TimeSpan.MinValue)        // start the clock
            {
                AnimationFrameStartTime = totalGameTime;
                AnimationFrameIndex = 0;
            }

            // How long does each frame last? When did we last flip a frame?
            TimeSpan lastFrameTime = AnimationFrameStartTime;
            TimeSpan lengthOfFrame = TimeSpan.FromSeconds(CurrentAnimation.FrameSeconds);

            // Update the current frame index by seeing how much time has passed, and then
            // moving to the correct frame.
            lastFrameTime = lastFrameTime.Add(lengthOfFrame);     // account for the current frame.

            while (lastFrameTime <= totalGameTime)
            {
                // Move to the next frame.
                AnimationFrameIndex += 1;

                // Are we at the end of this animation?
                if (AnimationFrameIndex == CurrentAnimation.FrameCount)
                {
                    switch (EndingAction)
                    {
                        case AnimationEndingAction.Loop:
                            AnimationFrameIndex = 0;
                            break;

                        case AnimationEndingAction.Stop:
                            IsAnimating = false;
                            AnimationFrameIndex -= 1;
                            OnAnimationComplete();
                            break;

                        case AnimationEndingAction.StopAndReset:
                            IsAnimating = false;
                            AnimationFrameIndex = 0;
                            OnAnimationComplete();
                            break;
                    }
                }

                // Load the texture atlas rect for the next animtaion frame.
                var spriteRect = CurrentAnimation.GetSpriteFrame(Direction, AnimationFrameIndex);
                AtlasRect = new Rectangle(
                    (int) spriteRect.X,
                    (int) spriteRect.Y,
                    (int) Definition.Size.Width,
                    (int) Definition.Size.Height);

                // Update the time when this animation frame was first displayed
                AnimationFrameStartTime = totalGameTime;
                lastFrameTime = lastFrameTime.Add(lengthOfFrame);
            }
        }

        /// <summary>
        /// Send update to the sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(Vector2 position, double currentTime, double deltaTime)
        {
            GameRoot.Renderer.Draw(
                Definition.Texture,
                AtlasRect,
                position);
        }

        /// <summary>
        ///  Kills the current animation.
        /// </summary>
        /// <param name="shouldSwitchToIdle"></param>
        public void AbortCurrentAnimation(bool shouldSwitchToIdle)
        {
            IsAnimating = false;
        }

        /// <summary>
        ///  Called when an animation completes.
        /// </summary>
        /// <param name="animation"></param>
        private void OnAnimationComplete()
        {
            IsAnimating = false;
        }
    }
}

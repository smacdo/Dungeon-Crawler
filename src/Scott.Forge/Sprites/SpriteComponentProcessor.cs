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
using Scott.Forge.GameObjects;
using Scott.Forge.Graphics;
using Scott.Forge.Spatial;

namespace Scott.Forge.Sprites
{
    /// <summary>
    ///  Processes sprite components.
    ///  TODO: Don't update sprites that are not active.
    /// </summary>
    public class SpriteComponentProcessor : ComponentProcessor<SpriteComponent>
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public SpriteComponentProcessor(GameScene scene)
            : base(scene)
        {
        }

        /// <summary>
        ///  Update sprite component.
        /// </summary>
        protected override void UpdateComponent(
            SpriteComponent sprite,
            double currentTimeSeconds,
            double deltaTimeSeconds)
        {
            var totalGameTime = TimeSpan.FromSeconds(currentTimeSeconds);

            // Was there a request to start playing an animation?
            if (sprite.RequestedAnimation.HasValue)
            {
                ProcessAnimationRequest(sprite);
                sprite.RequestedAnimation = null;
            }

            // Do not update component if no animation is being played.
            if (sprite.CurrentAnimation == null)
            {
                return;
            }

            // Update animation timing.
            sprite.AnimationFrameSecondsActive += deltaTimeSeconds;
            sprite.AnimationSecondsActive += deltaTimeSeconds;

            // Update the animation by advancing as many animation frames as possible in the amount of time that has
            // elapsed since the last animation update.
            bool animationCompleted = false;

            while (sprite.AnimationFrameSecondsActive >= sprite.CurrentAnimation.FrameSeconds)
            {
                // Move to the next frame.
                sprite.AnimationFrameIndex += 1;

                // Are we at the end of this animation?
                if (sprite.AnimationFrameIndex == sprite.CurrentAnimation.FrameCount)
                {
                    // Animation has completed.
                    var animation = sprite.CurrentAnimation;

                    switch (sprite.EndingAction)
                    {
                        case AnimationEndingAction.Loop:
                            sprite.AnimationFrameIndex = 0;
                            break;

                        case AnimationEndingAction.Stop:
                            sprite.AnimationFrameIndex -= 1;
                            animationCompleted = true;
                            break;

                        case AnimationEndingAction.StopAndReset:
                            sprite.AnimationFrameIndex = 0;
                            animationCompleted = true;
                            break;
                    }
                }

                // Update sprite texture atlas positions for the next animation frame. 
                var atlasPosition = sprite.CurrentAnimation.GetAtlasPosition(
                    sprite.Direction,
                    sprite.AnimationFrameIndex);

                UpdateSpriteTextureAtlasPosition(sprite, atlasPosition);

                // Update the time when this animation frame was first displayed
                sprite.AnimationFrameSecondsActive -= sprite.CurrentAnimation.FrameSeconds;
            }

            // Reset animation state if the current animation has completed.
            if (animationCompleted)
            {
                sprite.CurrentAnimation = null;
                sprite.NotifyAnimationComplete();
            }
        }

        /// <summary>
        ///  Do the work of starting a new animation from an animation request.
        /// </summary>
        /// <param name="sprite"></param>
        private void ProcessAnimationRequest(SpriteComponent sprite)
        {
            var request = sprite.RequestedAnimation.Value;

            // Are we about to abort a currently playing animation?
            if (sprite.IsAnimating)
            {
                sprite.CurrentAnimation = null;
            }

            // Get the animation definition for the requested animation. Verify it has the requested direction before
            // playing.
            var animation = sprite.Animations.Get(request.Name);
            var directionInt = (int)request.Direction;

            if (animation.Frames.GetLength(0) <= directionInt)
            {
                throw new AnimationDirectionNotFoundException(request.Name, request.Direction);
            }

            // Update state of the sprite component to start playing animation.
            sprite.CurrentAnimation = animation;
            sprite.Direction = request.Direction;
            sprite.EndingAction = request.EndingAction;

            sprite.AnimationSecondsActive = 0.0;            // TODO: Should this be zero or time when request was made?
            sprite.AnimationFrameSecondsActive = 0.0;       //  ... request time is time at last update.

            sprite.AnimationFrameIndex = 0;

            // Initialize sprite frames to start of animation.
            var atlasPosition = sprite.CurrentAnimation.GetAtlasPosition(
                    sprite.Direction,
                    sprite.AnimationFrameIndex);

            UpdateSpriteTextureAtlasPosition(sprite, atlasPosition);
        }

        /// <summary>
        ///  Set all sprites in a sprite component to have the given texture atlas position.
        /// </summary>
        /// <param name="sprite">Sprite component to update.</param>
        /// <param name="atlasPosition">New texture atlas size.</param>
        private void UpdateSpriteTextureAtlasPosition(SpriteComponent sprite, Vector2 atlasPosition)
        {
            for (var layer = 0; layer < sprite.Sprites.Length; layer++)
            {
                sprite.SpriteRects[layer] = new RectF(atlasPosition, sprite.Sprites[layer].Size);
            }
        }

        /// <summary>
        ///  Draw all sprites from this component processor.
        /// </summary>
        /// <param name="currentTime">Current time in seconds.</param>
        /// <param name="deltaTime">Time since last draw call in seconds.</param>
        public void Draw(IGameRenderer renderer, Camera camera, double currentTime, double deltaTime)
        {
            // TODO: Move debug drawing into a DebugComponent.
            for (var index = 0; index < mComponents.Count; ++index)
            {
                if (!mComponents[index].Active)
                {
                    continue;
                }

                var component = mComponents[index];
                var transform = component.Owner.Transform;

                // Move sprite to camera space for rendering on the screen.
                var positionInCameraSpace = transform.WorldPosition;

                if (camera != null)
                {
                    positionInCameraSpace = camera.WorldToScreen(transform.WorldPosition);
                }
            
                // Draw each layer of the sprite.
                for (var layer = 0; layer < component.Sprites.Length; layer++)
                {
                    renderer.Draw(
                        component.Sprites[layer].Atlas,
                        component.SpriteRects[layer],
                        positionInCameraSpace,
                        (component.RendererIgnoreTransformRotation ? 0.0f : transform.WorldRotation));

#if DEBUG
                    // Draw sprite rectangles.
                    if (Globals.Settings.DrawSpriteDebug)
                    {
                        var spriteRect = new BoundingRect(
                            centerX: positionInCameraSpace.X,
                            centerY: positionInCameraSpace.Y,
                            halfWidth: component.SpriteRects[layer].Size.Width / 2,
                            halfHeight: component.SpriteRects[layer].Size.Height / 2);

                        Globals.Debug.DrawBoundingRect(spriteRect, Microsoft.Xna.Framework.Color.White);
                    }
#endif
                }
                
#if DEBUG
                // Draw transform position and location.
                if (Globals.Settings.DrawTransformDebug)
                {
                    Globals.Debug.DrawPoint(positionInCameraSpace, Microsoft.Xna.Framework.Color.Blue, 4.0f);
                    Globals.Debug.DrawLine(
                        positionInCameraSpace,
                        positionInCameraSpace + (transform.Forward * 16.0f),
                        color: Microsoft.Xna.Framework.Color.LightBlue);
                }
#endif

#if DEBUG
                // Draw collison information.
                if (Globals.Settings.DrawPhysicsDebug)
                {
                    var physics = component.Owner.Get<Physics.PhysicsComponent>();

                    if (physics != null && camera != null)
                    {
                        var pics = camera.WorldToScreen(physics.WorldBounds);

                        var spriteRect = new BoundingRect(
                            centerX: pics.X,
                            centerY: pics.Y,
                            halfWidth: physics.WorldBounds.HalfWidth,
                            halfHeight: physics.WorldBounds.HalfHeight);

                        Globals.Debug.DrawBoundingRect(spriteRect, Microsoft.Xna.Framework.Color.Yellow);
                    }
                }
            }
#endif
        }
    }
}

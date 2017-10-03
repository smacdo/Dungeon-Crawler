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
using Forge.GameObjects;
using Forge.Graphics;
using Forge.Spatial;

namespace Forge.Sprites
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
        protected override void UpdateComponent(SpriteComponent sprite, TimeSpan currentTime, TimeSpan deltaTime)
        {
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
            sprite.CurrentFrameTime += deltaTime;
            sprite.AnimationTimeActive += deltaTime;

            // Update the animation by advancing as many animation frames as possible in the amount of time that has
            // elapsed since the last animation update.
            bool animationCompleted = false;

            while (sprite.CurrentFrameTime >= sprite.CurrentAnimation.FrameTime)
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
                UpdateSpriteTextureAtlasPosition(sprite, sprite.CurrentAnimation, sprite.AnimationFrameIndex);

                // Update the time when this animation frame was first displayed
                sprite.CurrentFrameTime -= sprite.CurrentAnimation.FrameTime;
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
            
            // Update state of the sprite component to start playing animation.
            var animation = sprite.Animations.Get(request.Name);

            sprite.CurrentAnimation = animation;
            sprite.EndingAction = request.EndingAction;

            sprite.AnimationTimeActive = TimeSpan.Zero; // TODO: Should this be zero or time when request was made?
            sprite.CurrentFrameTime = TimeSpan.Zero;

            sprite.AnimationFrameIndex = 0;

            // Initialize sprite frames to start of animation.
            UpdateSpriteTextureAtlasPosition(sprite, sprite.CurrentAnimation, sprite.AnimationFrameIndex);
        }

        /// <summary>
        ///  Set all sprites in a sprite component to have the given texture atlas position.
        /// </summary>
        /// <param name="sprite">Sprite component to update.</param>
        /// <param name="atlasPosition">New texture atlas size.</param>
        private void UpdateSpriteTextureAtlasPosition(
            SpriteComponent sprite,
            AnimationDefinition animation,
            int animationFrameIndex)
        {
            for (var layer = 0; layer < sprite.Sprites.Length; layer++)
            {
                var size = sprite.Sprites[layer].Size;

                for (int i = 0; i < Constants.DirectionCount; i++)
                {
                    var position = animation.GetAtlasPosition((DirectionName)i, animationFrameIndex);
                    sprite.SpriteRects[layer, i] = new RectF(position, size);
                }
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
                    // Check if sprite should be rotated directly, or a directional image should be choosen to
                    // represent the sprite's current rotation.
                    var rotation = 0.0f;
                    var spriteRect = RectF.Empty;

                    switch (component.RotationRenderMethod)
                    {
                        case SpriteRotationRenderMethod.Default:
                        case SpriteRotationRenderMethod.Rotated:
                            rotation = transform.WorldRotation;
                            spriteRect = component.SpriteRects[layer, 0];
                            break;

                        case SpriteRotationRenderMethod.FourWay:
                            var dir = DirectionNameHelper.FromRotationRadians(transform.WorldRotation);
                            rotation = 0.0f;
                            spriteRect = component.SpriteRects[layer, (int)dir];
                            break;

                        default:
                            throw new InvalidOperationException("Unsupported rotation render method");
                    }

                    // Draw the sprite with the selected image and rotation.
                    renderer.Draw(
                        component.Sprites[layer].Atlas,
                        spriteRect,
                        positionInCameraSpace,
                        rotation);

#if DEBUG
                    // Draw sprite rectangles.
                    if (Globals.Settings.DrawSpriteDebug)
                    {
                        var spriteBoundRect = new BoundingRect(
                            centerX: positionInCameraSpace.X,
                            centerY: positionInCameraSpace.Y,
                            halfWidth: component.SpriteRects[layer, 0].Size.Width / 2,
                            halfHeight: component.SpriteRects[layer, 0].Size.Height / 2);

                        Globals.Debug.DrawBoundingRect(spriteBoundRect, Microsoft.Xna.Framework.Color.White);
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
#endif
            }
        }
    }
}

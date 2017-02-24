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

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  Processes sprite components.
    /// </summary>
    public class SpriteComponentProcessor : ComponentProcessor<SpriteComponent>
    {
        /// <summary>
        ///  Update sprite component.
        /// </summary>
        protected override void UpdateComponent(
            SpriteComponent sprite,
            double currentTimeSeconds,
            double deltaTimeSeconds)
        {
            var totalGameTime = TimeSpan.FromSeconds(currentTimeSeconds);

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
                var atlasOffset = Vector2.Zero;

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
                            OnAnimationComplete(sprite);

                            animationCompleted = true;
                            break;

                        case AnimationEndingAction.StopAndReset:
                            sprite.AnimationFrameIndex = 0;
                            OnAnimationComplete(sprite);

                            animationCompleted = true;
                            break;
                    }
                }

                // Load the texture atlas rect for the next animation frame. 
                atlasOffset = sprite.CurrentAnimation.GetSpriteFrame(
                    sprite.Direction,
                    sprite.AnimationFrameIndex);
                
                // Update sprite atlas rectangles for the renderer.
                for (var layer = 0; layer < sprite.Sprites.Length; layer++)
                {
                    sprite.SpriteRects[layer] = new Microsoft.Xna.Framework.Rectangle(
                        (int) atlasOffset.X,
                        (int) atlasOffset.Y,
                        (int) sprite.Sprites[layer].Size.Width,
                        (int) sprite.Sprites[layer].Size.Height);
                }

                // Update the time when this animation frame was first displayed
                sprite.AnimationFrameSecondsActive -= sprite.CurrentAnimation.FrameSeconds;
            }

            // Reset animation state if the current animation has completed.
            if (animationCompleted)
            {
                sprite.CurrentAnimation = null;
            }
        }

        public void Draw(double currentTime, double deltaTime)
        {
            for (var index = 0; index < mComponents.Count; ++index)
            {
                if (!mComponents[index].Active)
                {
                    continue;
                }

                var component = mComponents[index];
                var transform = component.Owner.Transform;

                for (var layer = 0; layer < component.Sprites.Length; layer++)
                {
                    GameRoot.Renderer.Draw(
                        component.Sprites[layer].Texture,
                        component.SpriteRects[layer],
                        transform.WorldPosition,
                        (component.RendererIgnoreTransformRotation ? 0.0f : transform.WorldRotation));

                    // Draw sprite rectangles.
                    var spriteRect = new BoundingRect(
                        transform.WorldPosition.X,
                        transform.WorldPosition.Y,
                        component.SpriteRects[layer].Size.X / 2,
                        component.SpriteRects[layer].Size.Y / 2);

                    GameRoot.Debug.DrawBoundingRect(spriteRect, Microsoft.Xna.Framework.Color.White);
                }

                // Draw transform position and location.
                GameRoot.Debug.DrawPoint(transform.WorldPosition, 4, Microsoft.Xna.Framework.Color.Blue);
                GameRoot.Debug.DrawLine(
                    transform.WorldPosition,
                    transform.WorldPosition + (transform.Forward * 16.0f),
                    Microsoft.Xna.Framework.Color.LightBlue);
            }
        }

        /// <summary>
        ///  Called when an animation completes.
        /// </summary>
        /// <param name="animation"></param>
        private void OnAnimationComplete(SpriteComponent component)
        {
        }
    }
}

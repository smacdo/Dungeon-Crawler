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
        protected override void UpdateComponent(SpriteComponent component, double currentTime, double deltaTime)
        {
            var totalGameTime = TimeSpan.FromSeconds(currentTime);

            // Don't update if component is not playing animation.
            if (component.CurrentAnimation == null)
            {
                return;
            }

            // Check if this is the first time we've updated this sprite. If so, initialize our
            // animation values for the next call to update. Otherwise proceed as normal.
            // TODO: Rewrite this.
            if (component.AnimationFrameStartTime == TimeSpan.MinValue)        // start the clock
            {
                component.AnimationFrameStartTime = totalGameTime;
                component.AnimationFrameIndex = 0;
            }

            // How long does each frame last? When did we last flip a frame?
            TimeSpan lastFrameTime = component.AnimationFrameStartTime;
            TimeSpan lengthOfFrame = TimeSpan.FromSeconds(component.CurrentAnimation.FrameSeconds);

            // Update the current frame index by seeing how much time has passed, and then
            // moving to the correct frame.
            lastFrameTime = lastFrameTime.Add(lengthOfFrame);     // account for the current frame.

            while (lastFrameTime <= totalGameTime)
            {
                // Move to the next frame.
                component.AnimationFrameIndex += 1;

                // Are we at the end of this animation?
                if (component.AnimationFrameIndex == component.CurrentAnimation.FrameCount)
                {
                    switch (component.EndingAction)
                    {
                        case AnimationEndingAction.Loop:
                            component.AnimationFrameIndex = 0;
                            break;

                        case AnimationEndingAction.Stop:
                            component.CurrentAnimation = null;
                            component.AnimationFrameIndex -= 1;
                            OnAnimationComplete(component);
                            break;

                        case AnimationEndingAction.StopAndReset:
                            component.CurrentAnimation = null;
                            component.AnimationFrameIndex = 0;
                            OnAnimationComplete(component);
                            break;
                    }
                }

                // Load the texture atlas rect for the next animtaion frame.
                var atlasOffset = component.CurrentAnimation.GetSpriteFrame(
                    component.Direction,
                    component.AnimationFrameIndex);

                for (var layer = 0; layer < component.Sprites.Length; layer++)
                {
                    component.SpriteRects[layer] = new Microsoft.Xna.Framework.Rectangle(
                        (int) atlasOffset.X,
                        (int) atlasOffset.Y,
                        (int) component.Sprites[layer].Size.Width,      // TODO: Have width on SPriteComponent.
                        (int) component.Sprites[layer].Size.Height);
                }

                // Update the time when this animation frame was first displayed
                component.AnimationFrameStartTime = totalGameTime;
                lastFrameTime = lastFrameTime.Add(lengthOfFrame);
            }
        }

        public void Draw(double currentTime, double deltaTime)
        {
            for (var index = 0; index < mComponents.Count; ++index)
            {
                var component = mComponents[index];
                var transform = component.Owner.Transform;

                for (var layer = 0; layer < component.Sprites.Length; layer++)
                {
                    GameRoot.Renderer.Draw(
                        component.Sprites[layer].Texture,
                        component.SpriteRects[layer],
                        transform.WorldPosition,
                        (component.RendererIgnoreTransformRotation ? 0.0f : transform.WorldRotation));
                }

                // Draw debug rotation.
                var rotationLineStart = transform.WorldPosition;
                var rotationLineEnd = transform.WorldPosition + (transform.Forward * 16.0f);

                GameRoot.Debug.DrawLine(rotationLineStart, rotationLineEnd, Microsoft.Xna.Framework.Color.Red);
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

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
    ///  Processes character agents in the world.
    /// </summary>
    public class SpriteProcessor : ComponentProcessor<SpriteComponent>
    {
        protected override void UpdateComponent(SpriteComponent component, double currentTime, double deltaTime)
        {
            // TODO: Make much better by moving SpriteComponent.Update to here.
            component.Update(currentTime, deltaTime);
        }

        public void Draw(double currentTime, double deltaTime)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < mComponents.Count; ++index)
            {
                mComponents[index].Draw(currentTime, deltaTime);
            }
        }
    }
}

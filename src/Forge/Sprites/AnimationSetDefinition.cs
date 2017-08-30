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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.Sprites
{
    /// <summary>
    ///  AnimationSetDefinition defines a set of animations contained in a sprite sheet.
    /// </summary>
    /// <remarks>
    ///  Does not define sprite specific data, like a texture to use or the size of the sprite. Animation sets contain
    ///  offset data and should be combined with sprite definition. This way one animation set definition can be used
    ///  with multiple sprites.
    /// </remarks>
    public class AnimationSetDefinition
    {
        private Dictionary<string, AnimationDefinition> mAnimations;

        /// <summary>
        ///  List of animations contained in the set.
        /// </summary>
        /// <param name="animationList">List of animations.</param>
        public AnimationSetDefinition(List<AnimationDefinition> animationList)
        {
            var animationCount = (animationList != null ? animationList.Count : 0);
            mAnimations = new Dictionary<string, AnimationDefinition>(animationCount);

            for (int i = 0; i < animationCount; i++)
            {
                var animation = animationList[i];

                if (animation == null)
                {
                    throw new ArgumentException("Animation list contains a null animation defintion", "animationList");
                }

                mAnimations.Add(animation.Name, animation);
            }
        }

        /// <summary>
        ///  Get animation by name.
        /// </summary>
        /// <param name="animationName">Name of animation.</param>
        /// <returns>Animation definition.</returns>
        public AnimationDefinition this[string animationName]
        {
            get
            {
                AnimationDefinition definition = null;

                if (!mAnimations.TryGetValue(animationName, out definition))
                {
                    throw new AnimationNotFoundException("animationName");
                }

                return definition;
            }
        }

        /// <summary>
        ///  Get animation definition by name.
        /// </summary>
        /// <param name="animationName">Name of animation.</param>
        /// <returns>Animation definition.</returns>
        public AnimationDefinition Get(string animationName)
        {
            AnimationDefinition definition = null;

            if (!mAnimations.TryGetValue(animationName, out definition))
            {
                throw new AnimationNotFoundException("animationName");
            }

            return definition;
        }
    }
}

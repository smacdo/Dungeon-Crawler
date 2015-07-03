/*
 * Copyright 2012-2013 Scott MacDonald.
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
namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  A sprite exception.
    /// </summary>
    public class SpriteException : GameEngineException
    {
        /// <summary>
        ///  Get the sprite triggering the exception.
        /// </summary>
        public Sprite Sprite { get; protected set; }

        public SpriteException()
            : base()
        {
        }

        public SpriteException(string message)
            : base(message)
        {
        }

        public SpriteException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// An exception that happens during sprite animation
    /// </summary>
    public class SpriteAnimationNotFoundException : SpriteException
    {
        public string AnimationName { get; private set; }

        public DirectionName Direction { get; private set; }

        public SpriteAnimationNotFoundException(
            Sprite sprite,
            string animationName,
            DirectionName direction)
            : base("Animation not found")
        {
            Sprite = sprite;
            AnimationName = animationName;
            Direction = direction;
        }
    }

    /// <summary>
    /// An exception that happens during sprite animation
    /// </summary>
    public class SpriteComponentLayerNotFound : SpriteException
    {
        public string LayerName { get; private set; }

        public SpriteComponentLayerNotFound(string layerName)
            : base("Sprite component layer name not found")
        {
            LayerName = layerName;
        }
    }
}

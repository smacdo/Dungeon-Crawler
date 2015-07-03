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
using System.Diagnostics;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  A more intelligent and complex version of the basic sprite component.
    ///  [TODO: Explain]
    /// </summary>
    public class ActorSpriteComponent : Component
    {
        public const int MaxLayerCount = 9;

        private List<Sprite> mSprites;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ActorSpriteComponent()
        {
            mSprites = new List<Sprite>(MaxLayerCount);

            for (int i = 0; i < MaxLayerCount; ++i)
            {
                mSprites.Add(null);
            }
        }

        /// <summary>
        ///  Get the belt sprite.
        /// </summary>
        public Sprite Belt
        {
            get { return mSprites[(int) ActorSpriteLayer.Belt]; }
            set { mSprites[(int) ActorSpriteLayer.Belt] = value; }
        }

        /// <summary>
        ///  Get the body sprite.
        /// </summary>
        public Sprite Body
        {
            get { return mSprites[(int) ActorSpriteLayer.Body]; }
            set { mSprites[(int) ActorSpriteLayer.Body] = value;  }
        }

        /// <summary>
        ///  Get the bracer sprite.
        /// </summary>
        public Sprite Bracer
        {
            get { return mSprites[(int) ActorSpriteLayer.Bracer]; }
            set { mSprites[(int) ActorSpriteLayer.Bracer] = value; }
        }

        /// <summary>
        ///  Get the feet sprite.
        /// </summary>
        public Sprite Feet
        {
            get { return mSprites[(int) ActorSpriteLayer.Feet]; }
            set { mSprites[(int) ActorSpriteLayer.Feet] = value; }
        }

        /// <summary>
        ///  Get if body rendering is enabled.
        /// </summary>
        public bool IsBodyEnabled { get; set; }

        /// <summary>
        ///  Get if clothing layers are enabled.
        /// </summary>
        public bool IsClothingEnabled { get; set; }

        /// <summary>
        ///  Get if weapon is enabled.
        /// </summary>
        public bool IsWeaponEnabled { get; set; }

        /// <summary>
        ///  Get the head sprite.
        /// </summary>
        public Sprite Head
        {
            get { return mSprites[(int) ActorSpriteLayer.Head]; }
            set { mSprites[(int) ActorSpriteLayer.Head] = value; }
        }

        /// <summary>
        ///  Get the leg sprite.
        /// </summary>
        public Sprite Legs
        {
            get { return mSprites[(int) ActorSpriteLayer.Legs]; }
            set { mSprites[(int) ActorSpriteLayer.Legs] = value; }
        }

        /// <summary>
        ///  Get the shoudler sprite.
        /// </summary>
        public Sprite Shoulder
        {
            get { return mSprites[(int) ActorSpriteLayer.Shoulder]; }
            set { mSprites[(int) ActorSpriteLayer.Shoulder] = value; }
        }

        /// <summary>
        ///  Get the torso sprite.
        /// </summary>
        public Sprite Torso
        {
            get { return mSprites[(int) ActorSpriteLayer.Torso]; }
            set { mSprites[(int) ActorSpriteLayer.Torso] = value; }
        }

        /// <summary>
        ///  Get the weapon sprite.
        /// </summary>
        public Sprite Weapon
        {
            get { return mSprites[(int) ActorSpriteLayer.Head]; }
            set { mSprites[(int) ActorSpriteLayer.Head] = value; }
        }

        /// <summary>
        ///  Plays an animation on the body and clothes.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="direction">Sprite direction.</param>
        /// <param name="endingAction">Action to take when animation ends.</param>
        public void PlayAnimationOnBodyAndClothes(
            string animationName,
            DirectionName direction,
            AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset)
        {
            if (IsBodyEnabled)
            {
                Body.PlayAnimation(animationName, direction, endingAction);
            }

            if (IsClothingEnabled)
            {
                Torso.PlayAnimation(animationName, direction, endingAction);
                Legs.PlayAnimation(animationName, direction, endingAction);
                Feet.PlayAnimation(animationName, direction, endingAction);
                Head.PlayAnimation(animationName, direction, endingAction);
                Bracer.PlayAnimation(animationName, direction, endingAction);
                Shoulder.PlayAnimation(animationName, direction, endingAction);
                Belt.PlayAnimation(animationName, direction, endingAction);
            }
        }

        /// <summary>
        ///  Plays an animation on the body and clothes.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="direction">Sprite direction.</param>
        /// <param name="endingAction">Action to take when animation ends.</param>
        public void PlayAnimationOnWeapon(
            string animationName,
            DirectionName direction,
            AnimationEndingAction endingAction = AnimationEndingAction.StopAndReset)
        {
            Debug.Assert(IsWeaponEnabled);
            Weapon.PlayAnimation(animationName, direction, endingAction);
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
            if (IsBodyEnabled)
            {
                Body.Update(currentTime, deltaTime);
            }

            if (IsClothingEnabled)
            {
                Torso.Update(currentTime, deltaTime);
                Legs.Update(currentTime, deltaTime);
                Feet.Update(currentTime, deltaTime);
                Head.Update(currentTime, deltaTime);
                Bracer.Update(currentTime, deltaTime);
                Shoulder.Update(currentTime, deltaTime);
                Belt.Update(currentTime, deltaTime);
            }

            if (IsWeaponEnabled)
            {
                Weapon.Update(currentTime, deltaTime);
            }
        }

        /// <summary>
        /// Send update to the sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(double currentTime, double deltaTime)
        {
            var origin = Owner.Transform.Position;

            if (IsBodyEnabled)
            {
                Body.Draw(origin, currentTime, deltaTime);
            }

            if (IsClothingEnabled)
            {
                Torso.Draw(origin, currentTime, deltaTime);
                Legs.Draw(origin, currentTime, deltaTime);
                Feet.Draw(origin, currentTime, deltaTime);
                Head.Draw(origin, currentTime, deltaTime);
                Bracer.Draw(origin, currentTime, deltaTime);
                Shoulder.Draw(origin, currentTime, deltaTime);
                Belt.Draw(origin, currentTime, deltaTime);
            }

            if (IsWeaponEnabled)
            {
                Weapon.Draw(origin, currentTime, deltaTime);
            }
        }
    }

    public enum ActorSpriteLayer
    {
        Body = 0,
        Torso = 1,
        Legs = 2,
        Feet = 3,
        Head = 4,
        Bracer = 5,
        Shoulder = 6,
        Belt = 7,
        Weapon = 8
    }
}

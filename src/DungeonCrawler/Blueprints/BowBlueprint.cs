/*
 * Copyright 2012-2017 Scott MacDonald
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
using Forge;
using Forge.Content;
using Forge.Sprites;
using System.Threading.Tasks;

namespace DungeonCrawler.Blueprints
{
    /// <summary>
    ///  Create an equippable sword.
    /// </summary>
    public class BowBlueprint : IBlueprint
    {
        private const string SpriteFilePath = "sprites/Weapon_Bow.sprite";

        /// <inheritdoc />
        public string[] AssetDependencies
        {
            get { return new string[] { SpriteFilePath }; }
        }

        /// <inheritdoc />
        public string Name { get { return BlueprintNames.Sword; } }

        /// <inheritdoc />
        public async Task<GameObject> Spawn(
            IBlueprintFactory blueprints,
            string gameObjectName,
            Vector2? position)
        {
            // Create the bow blueprint.
            var weapon = new GameObject(gameObjectName);

            // Create the sprite and set it up.
            var sprite = blueprints.Scene.Sprites.Create(weapon);
            sprite.SetSprite(await blueprints.Content.Load<AnimatedSpriteDefinition>(SpriteFilePath));
            sprite.RotationRenderMethod = SpriteRotationRenderMethod.FourWay;

            // Apply position.
            if (position.HasValue)
            {
                weapon.Transform.WorldPosition = position.Value;
            }

            return weapon;
        }
    }
}

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
using Scott.Forge;
using Scott.Forge.Content;
using Scott.Forge.Actors;
using Scott.Forge.Sprites;
using Scott.Forge.GameObjects;
using Scott.Forge.Physics;
using Scott.Forge.Engine;

namespace Scott.DungeonCrawler.GameObjects
{
    /// <summary>
    ///  Temporary blueprint provider. This is in place until we create a real one that actually
    ///  reads blueprints from a file.
    /// 
    ///  TODO: Generize this (engine/forge) since it has no dependency on game code... it just reads nested key/value
    ///        information and creates blueprints.
    /// </summary>
    public class DungeonCrawlerGameObjectFactory
    {
        public IContentManager Content { get; set; }

        public DungeonCrawlerGameObjectFactory(IContentManager content)
        {
            Content = content;
        }

        /// <summary>
        ///  Create a game object from the named blueprint.
        /// </summary>
        /// <param name="blueprintName"></param>
        /// <returns></returns>
        public GameObject Instantiate(GameScene scene, string blueprintName )
        {
            if ( blueprintName == "Player" )
            {
                return InstantiatePlayer(scene);
            }
            else if ( blueprintName == "Skeleton" )
            {
                return InstantiateSkeleton(scene);
            }
            else
            {
                throw new ArgumentException(
                    "Could not locate blueprint {0}".With( blueprintName ) );
            }
        }

        private GameObject InstantiatePlayer(GameScene scene)
        {
            // Create the player blue print.
            var player = new GameObject("player");

            // Create the sprite and set it up.
            var sprite = scene.Sprites.Create(player);

            sprite.RendererIgnoreTransformRotation = true;

            sprite.SetMultipleSpriteCount((int) ActorEquipmentSlot.Count);
            sprite.SetSprite(Content.Load<AnimatedSpriteDefinition>("sprites/Humanoid_Male"));

            sprite.SetLayer(
                (int) ActorEquipmentSlot.Torso,
                Content.Load<AnimatedSpriteDefinition>("sprites/Torso_Armor_Leather").Sprite);  // TODO: Add content processor for SpriteDefinition.
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Legs,
                Content.Load<AnimatedSpriteDefinition>("sprites/Legs_Pants_Green").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Feet,
                Content.Load<AnimatedSpriteDefinition>("sprites/Feet_Shoes_Brown").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Head,
                Content.Load<AnimatedSpriteDefinition>("sprites/Head_Helmet_Chain").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Bracer,
                Content.Load<AnimatedSpriteDefinition>("sprites/Bracer_Leather").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Shoulder,
                Content.Load<AnimatedSpriteDefinition>("sprites/Shoulder_Leather").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Belt,
                Content.Load<AnimatedSpriteDefinition>("sprites/Belt_Leather").Sprite);

            // Add actor component.
            var actor = scene.Actors.Create(player);

            // Add physics.
            var physics = scene.Physics.Create(player);

            physics.Size = new SizeF(16, 25);
            physics.CenterOffset = new Vector2(0, 6);

            // Create sword and center the object on the character.
            //  TODO: Don't hard code the values.
            var weaponGameObject = InstantiateSword(scene);
            weaponGameObject.Parent = player;

            weaponGameObject.Transform.LocalPosition = new Vector2(-192 / 2 + 32, -192 / 2 + 32);
            weaponGameObject.Active = false;

            return player;
        }

        private GameObject InstantiateSword(GameScene scene)
        {
            // Create the player blue print.
            var weapon = new GameObject("MeleeWeapon");

            // Create the sprite and set it up.
            var sprite = scene.Sprites.Create(weapon);
            sprite.SetSprite(Content.Load<AnimatedSpriteDefinition>("sprites/Weapon_Longsword"));

            //sprite.RendererIgnoreTransformRotation = true;

            return weapon;
        }

        private GameObject InstantiateSkeleton(GameScene scene)
        {
            var enemy = new GameObject("skeleton");
            var sprite = scene.Sprites.Create(enemy);

            sprite.SetSprite( Content.Load<AnimatedSpriteDefinition>( "sprites/Humanoid_Skeleton" ) );
            sprite.RendererIgnoreTransformRotation = true;

            scene.Actors.Create(enemy);
            scene.AI.Create(enemy);
            
            var collision = scene.Physics.Create(enemy);

            collision.Size = new SizeF(16, 25);
            collision.CenterOffset = new Vector2(0, 6);

            return enemy;
        }
    }
}

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
using Scott.Forge.Sprites;
using Scott.Forge.GameObjects;
using Scott.Forge.Actors;

namespace Scott.DungeonCrawler.GameObjects
{
    /// <summary>
    ///  Instantiates named game objects aka prefabs.
    /// </summary>
    /// <remarks>
    ///  Temporary blueprint provider. This is in place until we create a real one that actually
    ///  reads blueprints from a file.
    /// 
    ///  TODO: Generize this (engine/forge) since it has no dependency on game code... it just reads nested key/value
    ///        information and creates blueprints.
    ///        
    ///  TODO: May not be possible since a lot of blueprints have custom logic for event wiring and whatnot.
    /// </remarks>
    public class DungeonCrawlerGameObjectFactory
    {
        public IContentManager Content { get; set; }
        public GameScene Scene { get; set; }

        public DungeonCrawlerGameObjectFactory(IContentManager content, GameScene scene)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (scene == null)
            {
                throw new ArgumentNullException(nameof(scene));
            }

            Content = content;
            Scene = scene;
        }

        /// <summary>
        ///  Create a game object from the named blueprint.
        /// </summary>
        /// <param name="blueprintName"></param>
        /// <returns></returns>
        public GameObject Instantiate(string blueprintName, Vector2? position)
        {
            GameObject newObject = null;

            if (blueprintName == "Player")
            {
                newObject = InstantiatePlayer(position);
            }
            else if (blueprintName == "Skeleton")
            {
                newObject = InstantiateSkeleton(position);
            }
            else
            {
                throw new ArgumentException(
                    "Could not locate blueprint {0}".With(blueprintName));
            }

            Scene.Add(newObject);
            return newObject;
        }

        private GameObject InstantiatePlayer(Vector2? position)
        {
            // Create the player blue print.
            var player = new GameObject("player");

            // Create the sprite and set it up.
            var sprite = Scene.Sprites.Create(player);

            sprite.RendererIgnoreTransformRotation = true;

            sprite.SetMultipleSpriteCount((int) ActorEquipmentSlot.Count);
            sprite.SetSprite(Content.Load<AnimatedSpriteDefinition>("sprites/Humanoid_Male.sprite"));

            sprite.SetLayer(
                (int) ActorEquipmentSlot.Torso,
                Content.Load<AnimatedSpriteDefinition>("sprites/Torso_Armor_Leather.sprite").Sprite);  // TODO: Add content processor for SpriteDefinition.
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Legs,
                Content.Load<AnimatedSpriteDefinition>("sprites/Legs_Pants_Green.sprite").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Feet,
                Content.Load<AnimatedSpriteDefinition>("sprites/Feet_Shoes_Brown.sprite").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Head,
                Content.Load<AnimatedSpriteDefinition>("sprites/Head_Helmet_Chain.sprite").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Bracer,
                Content.Load<AnimatedSpriteDefinition>("sprites/Bracer_Leather.sprite").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Shoulder,
                Content.Load<AnimatedSpriteDefinition>("sprites/Shoulder_Leather.sprite").Sprite);
            sprite.SetLayer(
                (int) ActorEquipmentSlot.Belt,
                Content.Load<AnimatedSpriteDefinition>("sprites/Belt_Leather.sprite").Sprite);

            // Add actor component.
            var actor = Scene.Actors.Create(player);

            // Add physics.
            var physics = Scene.Physics.Create(player);

            physics.Size = new SizeF(16, 25);
            physics.CenterOffset = new Vector2(0, 6);

            // Apply position.
            if (position.HasValue)
            {
                player.Transform.WorldPosition = position.Value;
            }

            // Create sword and center the object on the character.
            //  TODO: Don't hard code the values.
            var weaponGameObject = InstantiateSword(position);
            weaponGameObject.Parent = player;

            weaponGameObject.Transform.LocalPosition = new Vector2(-192 / 2 + 32, -192 / 2 + 32);
            weaponGameObject.Active = false;

            return player;
        }

        private GameObject InstantiateSword(Vector2? position)
        {
            // Create the player blue print.
            var weapon = new GameObject("MeleeWeapon");

            // Create the sprite and set it up.
            var sprite = Scene.Sprites.Create(weapon);
            sprite.SetSprite(Content.Load<AnimatedSpriteDefinition>("sprites/Weapon_Longsword.sprite"));

            //sprite.RendererIgnoreTransformRotation = true;

            // Apply position.
            if (position.HasValue)
            {
                weapon.Transform.WorldPosition = position.Value;
            }

            return weapon;
        }

        private GameObject InstantiateSkeleton(Vector2? position)
        {
            var enemy = new GameObject("skeleton");
            var sprite = Scene.Sprites.Create(enemy);

            sprite.SetSprite( Content.Load<AnimatedSpriteDefinition>("sprites/Humanoid_Skeleton.sprite") );
            sprite.RendererIgnoreTransformRotation = true;

            Scene.Actors.Create(enemy);
            Scene.AI.Create(enemy);
            
            var collision = Scene.Physics.Create(enemy);

            collision.Size = new SizeF(16, 25);
            collision.CenterOffset = new Vector2(0, 6);

            if (position.HasValue)
            {
                enemy.Transform.WorldPosition = position.Value;
            }

            // temp hack
            var initialDirection = (DirectionName)GameRoot.Random.Next(0, 3);

            var actor = enemy.Get<ActorComponent>();
            actor.Direction = initialDirection;

            enemy.Get<SpriteComponent>().PlayAnimation("Walk", initialDirection, AnimationEndingAction.Loop);

            return enemy;
        }
    }
}

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
using Forge.Physics;

namespace DungeonCrawler.Blueprints
{
    /// <summary>
    ///  Create a skeletal enemy.
    /// </summary>
    public class PlayerBlueprint : IBlueprint
    {
        private const string BodySpritePath = "sprites/Humanoid_Male.sprite";
        private const string TorsoSpritePath = "sprites/Torso_Armor_Leather.sprite";
        private const string LegsSpritePath = "sprites/Legs_Pants_Green.sprite";
        private const string FeetSpritePath = "sprites/Feet_Shoes_Brown.sprite";
        private const string HeadSpritePath = "sprites/Head_Helmet_Chain.sprite";
        private const string BracerSpritePath = "sprites/Bracer_Leather.sprite";
        private const string ShoulderSpritePath = "sprites/Shoulder_Leather.sprite";
        private const string BeltSpritePath = "sprites/Belt_Leather.sprite";

        private const float CollisionWidth = 10;
        private const float CollisionHeight = 10;
        private const float CollisionOffsetX = 0;
        private const float CollisionOffsetY = 18;

        /// <inheritdoc />
        public string[] AssetDependencies
        {
            get
            {
                return new string[]
                {
                    BodySpritePath, TorsoSpritePath, LegsSpritePath, FeetSpritePath, HeadSpritePath, BracerSpritePath,
                    ShoulderSpritePath, BeltSpritePath
                };
            }
        }

        /// <inheritdoc />
        public string Name { get { return BlueprintNames.Player; } }
        
        /// <inheritdoc />
        public async Task<GameObject> Spawn(
            IBlueprintFactory blueprints,
            string name,
            Vector2? position)
        {
            var self = new GameObject(name);

            var content = blueprints.Content;
            var scene = blueprints.Scene;

            // Create the sprite and set it up.
            var sprite = scene.Sprites.Create(self);

            sprite.RotationRenderMethod = SpriteRotationRenderMethod.FourWay;

            sprite.SetMultipleSpriteCount((int)ActorEquipmentSlot.Count);
            sprite.SetSprite(await content.Load<AnimatedSpriteDefinition>(BodySpritePath));

            sprite.SetLayer(
                (int)ActorEquipmentSlot.Torso,
                await LoadSprite(content, TorsoSpritePath));  // TODO: Add content processor for SpriteDefinition.
            sprite.SetLayer(
                (int)ActorEquipmentSlot.Legs,
                await LoadSprite(content, LegsSpritePath));
            sprite.SetLayer(
                (int)ActorEquipmentSlot.Feet,
                await LoadSprite(content, FeetSpritePath));
            sprite.SetLayer(
                (int)ActorEquipmentSlot.Head,
                await LoadSprite(content, HeadSpritePath));
            sprite.SetLayer(
                (int)ActorEquipmentSlot.Bracer,
                await LoadSprite(content, BracerSpritePath));
            sprite.SetLayer(
                (int)ActorEquipmentSlot.Shoulder,
                await LoadSprite(content, ShoulderSpritePath));
            sprite.SetLayer(
                (int)ActorEquipmentSlot.Belt,
                await LoadSprite(content, BeltSpritePath));

            // Add actor component.
            var actor = scene.Actors.Create(self);

            // Add physics.
            var physics = scene.Physics.Create(self);

            physics.Size = new SizeF(CollisionWidth, CollisionHeight);
            physics.CenterOffset = new Vector2(CollisionOffsetX, CollisionOffsetY);

            // Apply position.
            if (position.HasValue)
            {
                self.Transform.WorldPosition = position.Value;
            }

            // Create sword and center the object on the character.
            //  TODO: Don't hard code the values.
            var weaponGameObject = await blueprints.Spawn(BlueprintNames.Sword, "Sword", null);
            weaponGameObject.Parent = self;

            weaponGameObject.Transform.LocalPosition = new Vector2(-192 / 2 + 32, -192 / 2 + 32);
            weaponGameObject.Active = true;

            return self;
        }

        private async Task<SpriteDefinition> LoadSprite(IContentManager content, string spritePath)
        {
            var animatedSprite = await content.Load<AnimatedSpriteDefinition>(spritePath);
            return animatedSprite.Sprite;
        }
    }
}

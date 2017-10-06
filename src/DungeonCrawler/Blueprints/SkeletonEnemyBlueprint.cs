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
using DungeonCrawler.Components;

namespace DungeonCrawler.Blueprints
{
    /// <summary>
    ///  Create a skeletal enemy.
    /// </summary>
    public class SkeletonEnemyBlueprint : IBlueprint
    {
        private const string SpriteFilePath = "sprites/Humanoid_Skeleton.sprite";

        private const float CollisionWidth = 10;
        private const float CollisionHeight = 10;
        private const float CollisionOffsetX = 0;
        private const float CollisionOffsetY = 18;

        /// <inheritdoc />
        public string[] AssetDependencies
        {
            get { return new string[] { SpriteFilePath }; }
        }

        /// <inheritdoc />
        public string Name { get { return BlueprintNames.Skeleton; } }

        /// <inheritdoc />
        public async Task<GameObject> Spawn(
            IBlueprintFactory blueprints,
            string gameObjectName,
            Vector2? position)
        {
            var self = new GameObject(gameObjectName);
            var scene = blueprints.Scene;

            // Create sprite.
            var sprite = scene.Sprites.Create(self);

            sprite.SetSprite(await blueprints.Content.Load<AnimatedSpriteDefinition>(SpriteFilePath));
            sprite.RotationRenderMethod = SpriteRotationRenderMethod.FourWay;

            // Spawn new actor and AI brain.
            scene.Locomotors.Create(self);
            scene.Actions.Create(self);
            scene.AI.Create(self);

            // Set up collision information.
            var collision = scene.Physics.Create(self);

            collision.Size = new SizeF(CollisionWidth, CollisionHeight);
            collision.CenterOffset = new Vector2(CollisionOffsetX, CollisionOffsetY);

            if (position.HasValue)
            {
                self.Transform.LocalPosition = position.Value;
            }

            // TODO: Randomize direction.
            self.Get<SpriteComponent>().PlayAnimation("Walk", AnimationEndingAction.Loop);

            // Configure health and damages.
            self.Add(new HealthComponent(20, 0, 20));
            self.Add(new DamageComponent());

            return self;
        }
    }
}

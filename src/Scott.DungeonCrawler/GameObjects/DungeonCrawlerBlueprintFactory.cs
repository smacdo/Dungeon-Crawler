/*
 * Copyright 2012-2014 Scott MacDonald
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
using Scott.Forge.Engine.Actors;
using Scott.Forge.Engine.Ai;
using Scott.Forge.Engine.Content;
using Scott.Forge.Engine.Graphics;
using Scott.Forge.Engine.Movement;
using Scott.Forge.GameObjects;

namespace Scott.DungeonCrawler.GameObjects
{
    /// <summary>
    ///  Temporary blueprint provider. This is in place until we create a real one that actually
    ///  reads blueprints from a file.
    /// 
    ///  TODO: Generize this (engine/forge) since it has no dependency on game code... it just reads nested key/value
    ///        information and creates blueprints.
    /// </summary>
    public class DungeonCrawlerGameObjectFactory : IBlueprintFactory
    {
        public IContentManager Content { get; set; }
        public SpriteProcessor SpriteProcessor { get; set; }        // TODO: Remove.
        public MovementProcessor MovementProcessor { get; set; }    // TODO: Remove.
        public ActorProcessor ActorProcessor { get; set; }          // TODO: Remove.
        public AiProcessor AiProcessor { get; set; }                // TODO: Remove.

        public DungeonCrawlerGameObjectFactory(IContentManager content)
        {
            Content = content;
        }

        /// <summary>
        ///  Create a game object from the named blueprint.
        /// </summary>
        /// <param name="blueprintName"></param>
        /// <returns></returns>
        public GameObject Instantiate( string blueprintName )
        {
            if ( blueprintName == "Player" )
            {
                return InstantiatePlayer();
            }
            else if ( blueprintName == "Skeleton" )
            {
                return InstantiateSkeleton();
            }
            else
            {
                throw new ArgumentException(
                    "Could not locate blueprint {0}".With( blueprintName ) );
            }
        }

        private GameObject InstantiatePlayer()
        {
            // Create the player blue print.
            var player = new GameObject("player");

            // Create the sprite and set it up.
            var sprite = SpriteProcessor.Add(player);

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Male" ) );

            sprite.AddLayer( "Torso", Content.Load<SpriteData>( "sprites/Torso_Armor_Leather" ) );
            sprite.AddLayer( "Legs", Content.Load<SpriteData>( "sprites/Legs_Pants_Green" ) );
            sprite.AddLayer( "Feet", Content.Load<SpriteData>( "sprites/Feet_Shoes_Brown" ) );
            sprite.AddLayer( "Head", Content.Load<SpriteData>( "sprites/Head_Helmet_Chain" ) );
            sprite.AddLayer( "Bracer", Content.Load<SpriteData>( "sprites/Bracer_Leather" ) );
            sprite.AddLayer( "Shoulder", Content.Load<SpriteData>( "sprites/Shoulder_Leather" ) );
            sprite.AddLayer( "Belt", Content.Load<SpriteData>( "sprites/Belt_Leather" ) );
            sprite.AddLayer( "Weapon", Content.Load<SpriteData>( "sprites/Weapon_Longsword" ), false );

            // Add movement component.
            var movement = MovementProcessor.Add(player);
            movement.MoveBox = new RectF( new Vector2( 15, 32 ), new Vector2( 32, 32 ) );

            // Add actor component.
            var actor = ActorProcessor.Add(player);

            return player;
        }

        private GameObject InstantiateSkeleton()
        {
            var enemy = new GameObject("skeleton");
            var sprite = SpriteProcessor.Add(enemy);

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Skeleton" ) );

            ActorProcessor.Add(enemy);
            AiProcessor.Add(enemy);

            var movement = MovementProcessor.Add(enemy);
            movement.MoveBox = new RectF( new Vector2( 15, 32 ), new Vector2( 32, 32 ) );

            return enemy;
        }
    }
}

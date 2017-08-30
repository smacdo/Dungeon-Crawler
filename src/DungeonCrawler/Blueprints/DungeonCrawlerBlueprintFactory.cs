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
using Forge;
using Forge.Content;
using Forge.GameObjects;
using Scott.DungeonCrawler.Blueprints;

namespace Scott.DungeonCrawler.Blueprints
{
    /// <summary>
    ///  Constructs game objects from blueprints.
    ///  TODO: Add support for auto-registration via attribute and then put this class in the engine since its generic.
    /// </summary>
    public class DungeonCrawlerBlueprintFactory : DefaultBlueprintFactory
    {
        public DungeonCrawlerBlueprintFactory(IContentManager content, GameScene scene)
            : base(content, scene)
        {
            Blueprints[BlueprintNames.Player] = new PlayerBlueprint();
            Blueprints[BlueprintNames.Skeleton] = new SkeletonEnemyBlueprint();
            Blueprints[BlueprintNames.Sword] = new SwordBlueprint();        // TODO: Rename melee weapon.
        }
    }

    /// <summary>
    ///  List of blueprints required by the game.
    /// </summary>
    public static class BlueprintNames
    {
        public static string Player { get { return "Player"; } }
        public static string Skeleton { get { return "Skeleton"; } }
        public static string Sword { get { return "Sword"; } }
    }
}

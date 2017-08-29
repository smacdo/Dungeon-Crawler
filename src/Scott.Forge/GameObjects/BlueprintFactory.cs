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
using System.Threading.Tasks;
using System.Collections.Generic;
using Scott.Forge.Content;

namespace Scott.Forge.GameObjects
{
    /// <summary>
    ///  Constructs game objects from blueprints.
    /// </summary>
    public interface IBlueprintFactory
    {
        /// <summary>
        ///  Get the content manager associated with this blueprint factory.
        /// </summary>
        IContentManager Content { get; }

        /// <summary>
        ///  Get the scene associated with this blueprint factory.
        /// </summary>
        GameScene Scene { get; }

        /// <summary>
        ///  Spawn a game object from a blueprint.
        /// </summary>
        /// <param name="blueprintName">Name of the blueprint to use.</param>
        /// <param name="gameObjectName">Optional name to assign to the new game object.</param>
        /// <param name="position">Optional local position to give to the game object.</param>
        /// <returns>New game object.</returns>
        Task<GameObject> Spawn(
            string blueprintName,
            string gameObjectName,
            Vector2? position);
    }

    /// <summary>
    ///  Constructs game objects from blueprints.
    /// </summary>
    public class DefaultBlueprintFactory : IBlueprintFactory
    {
        /// <summary>
        ///  Get or set the content manager associated with this blueprint factory.
        /// </summary>
        public IContentManager Content { get; set; }

        /// <summary>
        ///  Get or set the game scene associated with this blueprint factory.
        /// </summary>
        public GameScene Scene { get; set; }

        /// <summary>
        ///  Get the blueprints registered with this blueprint factory.
        /// </summary>
        public IDictionary<string, IBlueprint> Blueprints { get; }

        // TODO: Remove and use World.Randomor something else. This is a hack for the moment. 
        private System.Random mRandom = new System.Random();

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="content">Content manager.</param>
        /// <param name="scene">Scene.</param>
        public DefaultBlueprintFactory(IContentManager content, GameScene scene)
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

            Blueprints = new Dictionary<string, IBlueprint>();
        }

        /// <summary>
        ///  Spawn a game object from a blueprint.
        /// </summary>
        /// <param name="blueprintName">Name of the blueprint to use.</param>
        /// <param name="gameObjectName">Optional name to assign to the new game object.</param>
        /// <param name="position">Optional local position to give to the game object.</param>
        /// <returns>New game object.</returns>
        public async Task<GameObject> Spawn(
            string blueprintName,
            string gameObjectName,
            Vector2? position)
        {
            // Blueprint names must be provided (otherwise what is the point?).
            if (string.IsNullOrEmpty(blueprintName))
            {
                throw new ArgumentException(paramName: blueprintName, message: "Blueprint name required");
            }

            // Look up the blueprint by name and throw an exception if the blueprint could not be located.
            IBlueprint blueprint = null;

            if (!Blueprints.TryGetValue(blueprintName, out blueprint))
            {
                throw new BlueprintNotFoundException(blueprintName);
            }

            // Instantiate blueprint and return it to the caller.
            var newObject = await blueprint.Spawn(this, gameObjectName, position);
            Scene.Add(newObject);

            return newObject;
        }
    }

    /// <summary>
    ///  Exception thrown when a blueprint cannot be located by name.
    /// </summary>
    public class BlueprintNotFoundException : Exception
    {
        public BlueprintNotFoundException(string blueprintName)
            : base($"Blueprint '{blueprintName}' not found")
        {
            BlueprintName = blueprintName;
        }

        public string BlueprintName { get; }
    }
}

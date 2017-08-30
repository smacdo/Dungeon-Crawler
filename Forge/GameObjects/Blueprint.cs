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

namespace Forge.GameObjects
{
    /// <summary>
    ///  Interface for blueprints that can construct new game objects.
    /// </summary>
    /// <remarks>
    ///  TODO: Explain philosophy behind blueprints and how they work.
    /// </remarks>
    public interface IBlueprint
    {
        /// <summary>
        ///  Get a list of assets that this blueprint depends on.
        /// </summary>
        string[] AssetDependencies { get; }

        /// <summary>
        ///  Get the name of the blueprint.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///  Instantiate this blueprint as a new game object.
        /// </summary>
        /// <param name="blueprints">Blueprint factory.</param>
        /// <param name="name">Name of the new game object.</param>
        /// <param name="position">Optional position relative to parent.</param>
        /// <returns>Game object that was spawned from this blueprint.</returns>
        Task<GameObject> Spawn(
            IBlueprintFactory blueprints,
            string name,
            Vector2? position);
    }
}

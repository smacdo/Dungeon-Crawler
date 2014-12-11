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

namespace Scott.Forge.GameObjects
{
    /// <summary>
    ///  Interface for game objects.
    /// </summary>
    public interface IGameObject
    {
        void AddComponent<T>( T instance ) where T : IComponent;
        void DeleteComponent<T>() where T : IComponent;
        string DumpDebugInfoToString();
        T GetComponent<T>() where T : IComponent;
        bool HasComponent<T>() where T : IComponent;
        Guid Id { get; }
        string Name { get; }
        bool Enabled { get; set; }
        TransformComponent Transform { get; }
    }
}

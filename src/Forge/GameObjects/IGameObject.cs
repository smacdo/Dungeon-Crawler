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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.GameObjects
{
    /// <summary>
    ///  IGameObject defines the interface for Forge game objects.
    /// </summary>
    /// <remarks>
    ///  Forge uses game objects to implement the component entity design pattern. This pattern allows game developers
    ///  to create objects that are composed of arbitrary groups of component data.
    ///  
    ///  For more information on this design pattern visit: 
    ///    http://en.wikipedia.org/wiki/Entity_component_system
    /// </remarks>
    public interface IGameObject : IDisposable
    {
        /// <summary>
        ///  Get or set if this game object is active.
        /// </summary>
        /// <remarks>
        ///  Changing the value of this property will propagate this change to all children. A game object may continue
        ///  to be inactive even if this property is set to true because an ancestor is still disabled.
        /// </remarks>
        bool Active { get; set; }

        /// <summary>
        ///  Get if the GameObject is active in the current scene.
        /// </summary>
        bool ActiveInHierarchy { get; }

        /// <summary>
        ///  Get the local active state of the GameObject.
        /// </summary>
        bool ActiveSelf { get; }

        /// <summary>
        ///  Get the first child of this game object.
        /// </summary>
        IGameObject FirstChild { get; }

        /// <summary>
        ///  Get a unique identifier for this game object.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///  Get a string identifier for this game object.
        /// </summary>
        /// <remarks>
        ///  The name value does not have to globally unique.
        /// </remarks>
        string Name { get; }

        /// <summary>
        ///  Get the next sibling of this game object.
        /// </summary>
        IGameObject NextSibling { get; }

        /// <summary>
        ///  Get or set the parent of a game object.
        /// </summary>
        /// <remarks>
        ///  Changing the parent can trigger a cascade of updates which is a moderately slow operation.
        /// </remarks>
        IGameObject Parent { get; set; }

        /// <summary>
        ///  Get component containing information on this game object's physical location in the game world.
        /// </summary>
        TransformComponent Transform { get; }

        /// <summary>
        ///  Add a component to this game object.
        /// </summary>
        /// <remarks>
        ///  Only one component of each type can be added to the game object.
        /// </remarks>
        /// <typeparam name="TComponent">Component type to add.</typeparam>
        /// <param name="component">Component component to add.</param>
        void Add<TComponent>(TComponent component) where TComponent : IComponent;

        /// <summary>
        ///  Check if this game object has a component of the specified type.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to check for.</typeparam>
        /// <returns>True if the game object has the component type, false otherwise.</returns>
        bool Contains<TComponent>() where TComponent : IComponent;

        /// <summary>
        ///  Get a component from this game object. Will throw an exception if the component was not added.
        /// </summary>
        /// <typeparam name="TComponent">Type of game object to get.</typeparam>
        /// <returns>Component that was stored in this game object.</returns>
        TComponent Get<TComponent>() where TComponent : IComponent;

        /// <summary>
        ///  Find the first child that matches the given name.
        /// </summary>
        /// <param name="name">Name of the child game object.</param>
        /// <returns>The child game object if located otherwise null.</returns>
        IGameObject FindChildByName(string name);

        /// <summary>
        ///  Remove a component from this game object.
        /// </summary>
        /// <remarks>
        ///  Removing the transform component will also set the .Transform property to null. Generally, if the calling
        ///  code is not destroying the game object, removing the transform component will have unpredicable
        ///  consequences.
        /// </remarks>
        /// <typeparam name="TComponent">Component type to remove.</typeparam>
        bool Remove<TComponent>() where TComponent : IComponent;

        /// <summary>
        ///  Get a component from the game object. This will return null if the component was not added.
        /// </summary>
        /// <typeparam name="TComponent">Type of game object to get.</typeparam>
        /// <returns>Component that was stored in this game object.</returns>
        TComponent TryGet<TComponent>() where TComponent : class, IComponent;
    }
}

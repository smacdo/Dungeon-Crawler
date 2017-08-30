/*
 * Copyright 2012-2015 Scott MacDonald
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

namespace Forge.GameObjects
{
    /// <summary>
    ///  General component exception.
    /// </summary>
    public class GameComponentException : ForgeException
    {
        public GameComponentException()
        {
        }

        public GameComponentException(string message)
            : base(message)
        {
        }

        public GameComponentException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }        
    }

    /// <summary>
    ///  Game component reparenting not allowed exception.
    /// </summary>
    public class CannotChangeComponentOwnerException : GameComponentException
    {
        public CannotChangeComponentOwnerException(
            IComponent component,
            IGameObject currentOwner,
            IGameObject newOwner)
            : base("Cannot change ownership of game object component once set.")
        {
            Component = component;
            CurrentOwner = currentOwner;
            NewOwner = newOwner;
        }

        public IComponent Component { get; private set; }
        public IGameObject CurrentOwner { get; private set; }
        public IGameObject NewOwner { get; private set; }
    }

    /// <summary>
    ///  General game object exception.
    /// </summary>
    public class GameObjectException : ForgeException
    {
        public GameObjectException()
        {
        }

        public GameObjectException(string message)
            : base(message)
        {
        }

        public GameObjectException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    ///  Game object already has a component of this type.
    /// </summary>
    public class ComponentAlreadyAddedException : GameObjectException
    {
        public IGameObject GameObject { get; private set; }
        public System.Type ComponentType { get; private set; }

        public ComponentAlreadyAddedException(IGameObject gameObject, IComponent componentToAdd)
            : base("A component of the same type was already added to this game object")
        {
            GameObject = gameObject;

            if (componentToAdd != null)
            {
                ComponentType = componentToAdd.GetType();
            }
        }
    }

    /// <summary>
    ///  Game object already has a component of this type.
    /// </summary>
    public class ComponentDoesNotExistException : GameObjectException
    {
        public IGameObject GameObject { get; private set; }
        public System.Type ComponentType { get; private set; }

        public ComponentDoesNotExistException(IGameObject gameObject, System.Type componentType)
            : base("A component of the requested type does not exist in this game object")
        {
            GameObject = gameObject;
            ComponentType = componentType;
        }
    }
}

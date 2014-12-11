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
using System.Collections.Generic;
using System.Text;

namespace Scott.Forge.GameObjects
{
    /// <summary>
    /// Represents a physical object in the game world
    /// </summary>
    public class GameObject : IGameObject
    {
        private const int DEFAULT_COMPONENT_COUNT = 7;

        private string mName = String.Empty;
        private Guid mId = Guid.Empty;
        private GameObject mParent = null;
        private bool mEnabled = false;
        private Dictionary< System.Type, IComponent > mComponents = new Dictionary<Type, IComponent>( DEFAULT_COMPONENT_COUNT );
        private TransformComponent mTransform = new TransformComponent();

        /// <summary>
        ///  Creates a new empty game object that is not associated with any collection.
        /// </summary>
        public GameObject()
            : this ( "GameObject-NONAME")
        {
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="name">This game object's name</param>
        /// <param name="position">Position of the game object</param>
        /// <param name="direction">Direction of the game object</param>
        public GameObject(string name)
        {
            mName = name;
            mEnabled = true;

            mId = Guid.NewGuid();
        }

        /// <summary>
        ///  Gets the name of this game object.
        /// </summary>
        public string Name { get { return mName; } }

        /// <summary>
        ///  Gets the unique identifier for this game object.
        /// </summary>
        public Guid Id { get { return mId; } }

        /// <summary>
        ///  Check if game object is enabled or disabled. Disable objects are neither updated nor
        ///  displayed to the player.
        /// </summary>
        public bool Enabled { get { return mEnabled; } set { mEnabled = false; } }

        /// <summary>
        ///  Gets the transform for this game object.
        /// </summary>
        public TransformComponent Transform { get { return mTransform; } }

        /// <summary>
        ///  Adds a game component to this game object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddComponent<T>( T instance ) where T : IComponent
        {
            // Make sure the game component isn't already added
            if ( mComponents.ContainsKey( typeof( T ) ) )
            {
                throw new GameObjectException(
                    "This game object already has a component of type " + typeof( T ).Name,
                    this );
            }
            else
            {
                mComponents.Add( typeof( T ), instance );
            }
        }

        /// <summary>
        /// Removes the requested game component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DeleteComponent<T>() where T : IComponent
        {
            // Make sure the game component actually exists
            if ( mComponents.ContainsKey( typeof( T ) ) )
            {
                mComponents.Remove( typeof( T ) );
            }
            else
            {
                throw new GameObjectException(
                    "This game object does not have a component of type " + typeof( T ).Name,
                    this );
            }
        }

        /// <summary>
        /// Returns the game object's component. Use the non-generic GetComponent for a (slightly)
        /// faster look up.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : IComponent
        {
            IComponent component = null;
            mComponents.TryGetValue( typeof( T ), out component );

            return (T) component;       // TODO: Can this break if null?
        }

        public bool HasComponent<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Display debugging information about this game object.
        /// </summary>
        /// <returns></returns>
        public string DumpDebugInfoToString()
        {
            var debugText = new StringBuilder();

            debugText.Append( "\t{\n" );
            debugText.Append( "\t\tid:     {0}\n".With( Id ) );
            debugText.Append( "\t\tname:   \"{0}\"\n".With( Name ) );
            debugText.Append( "\t\ttransform:    {0}\n".With( Transform ) );
            debugText.Append( "\t\tcomponents: [\n" );

            // List the components attached to this game object (only the basics)
            foreach ( KeyValuePair<System.Type, IComponent> pair in mComponents )
            {
                debugText.Append(pair);
            }

            debugText.Append( "\t\t]\n" );
            debugText.Append( "\t},\n" );
            return debugText.ToString();
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Scott.Dungeon.Actor;
using Scott.Dungeon.AI;
using Scott.Dungeon.Graphics;
using Scott.Dungeon.Data;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Represents a physical object in the game world
    /// </summary>
    public class GameObject
    {
        private const int DEFAULT_COMPONENT_COUNT = 7;

        /// <summary>
        /// This game object's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Location of the game object 
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Get the direciton this object is facing
        /// </summary>
        public Direction Direction { get; set; }

        public BoundingRect Bounds { get; private set; }

        public int Width { get; set; }      // should we also adjust sprite width, or at least warn?
        public int Height { get; set; }     // same question

        private GameObjectCollection mParentCollection;
        private ulong mId;

        /// <summary>
        /// TODO: THIS NEEDS TO BE COMMENTED AND EXPLAINED
        /// </summary>
        private Dictionary< System.Type, IGameObjectComponent > mComponents;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject( string name, GameObjectCollection parentCollection, ulong id )
            : this( name, parentCollection, id, new Vector2( 0, 0 ), Direction.South, 0, 0 )
        {
        }

        /// <summary>
        /// Game object constructor
        /// </summary>
        /// <param name="name">This game object's name</param>
        /// <param name="position">Position of the game object</param>
        /// <param name="direction">Direction of the game object</param>
        /// <param name="width">Game object width</param>
        /// <param name="height">Game object height</param>
        public GameObject( string name,
                           GameObjectCollection parentCollection,
                           ulong id,
                           Vector2 position,
                           Direction direction,
                           int width, int height )
        {
            Name = name;
            Position = position;
            Direction = direction;

            Width = width;
            Height = height;

            mParentCollection = parentCollection;
            mComponents = new Dictionary<Type, IGameObjectComponent>( DEFAULT_COMPONENT_COUNT );
            mId = id;
        }

        /// <summary>
        /// Adds a game component to this game object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddComponent<T>( T instance ) where T : IGameObjectComponent
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
        public void DeleteComponent<T>() where T : IGameObjectComponent
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
        public T GetComponent<T>() where T : IGameObjectComponent
        {
            IGameObjectComponent component = null;
            mComponents.TryGetValue( typeof( T ), out component );

            return (T) component;
        }

        public string DumpDebugInfoToString()
        {
            StringBuilder debugText = new StringBuilder();

            debugText.Append(
                String.Format( "\t{{ id: {0}, name: \"{1}\", pos: {2}, dir: \"{3}\", w: {4}, h: {5}, components: [\n",
                               mId,
                               Name,
                               Position,
                               Direction.ToString(),
                               Width,
                               Height ) );

            // List the components attached to this game object (only the basics)
            foreach ( KeyValuePair<System.Type, IGameObjectComponent> pair in mComponents )
            {
                debugText.Append(
                    String.Format( "\t\t{{ type: \"{0}\", id: {1}, enabled: {2} }}\n",
                                   pair.Value.GetType().Name,
                                   pair.Value.Id,
                                   pair.Value.Enabled ) );
            }

            debugText.Append( "\t]},\n" );
            return debugText.ToString();
        }
    }
}

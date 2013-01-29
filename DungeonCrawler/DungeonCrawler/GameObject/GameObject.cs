﻿using Microsoft.Xna.Framework;
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
        /// Parent of this game object
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return mParent;
            }
            set
            {
                Reparent( value );
            }
        }

        /// <summary>
        /// List of children
        /// </summary>
        public List<GameObject> Children { get; private set; }

        /// <summary>
        /// This game object's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Unique identifier for this game object
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Location of the game object in world coordinates
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                SetPosition( value );
            }
        }

        public BoundingArea Bounds { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// Get the direciton this object is facing
        /// </summary>
        public Direction Direction { get; set; }

        private GameObject mParent;
        private Vector2 mPosition;
        private GameObjectCollection mParentCollection;

        /// <summary>
        /// TODO: THIS NEEDS TO BE COMMENTED AND EXPLAINED
        /// </summary>
        private Dictionary< System.Type, IGameObjectComponent > mComponents;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject( string name, GameObjectCollection parentCollection )
            : this( name, parentCollection, new Vector2( 0, 0 ), Direction.South )
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
                           Vector2 position,
                           Direction direction )
        {
            Name = name;
            mPosition = position;
            mParent = null;
            Direction = direction;
            Children = new List<GameObject>();
            Enabled = true;

            mParentCollection = parentCollection;
            mComponents = new Dictionary<Type, IGameObjectComponent>( DEFAULT_COMPONENT_COUNT );
            Id = Guid.NewGuid();
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
        public void DeleteComponent<T>() where T : AbstractGameObjectComponent
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

        /// <summary>
        /// Set the position of the game object and updates the position of all it's children
        /// game objects.
        /// </summary>
        /// <param name="position">The new position</param>
        public void SetPosition( Vector2 position )
        {
            Vector2 delta = position - mPosition;
            mPosition     = position;

            // Update our bounding area with the new position
            if ( Bounds != null )
            {
                Bounds.Move( delta );
            }

            // Update any attached child game objects with our changed position
            foreach ( GameObject child in Children )
            {
                child.Position += delta;
            }
        }

        public void AddChild( GameObject child )
        {
            child.Reparent( this );
        }

        /// <summary>
        /// Changes the parent of this game object
        /// </summary>
        /// <param name="newParent">Reference to the new parent (null for none)</param>
        private void Reparent( GameObject newParent )
        {
            // We cannot parent to ourselves
            if ( ReferenceEquals( this, newParent ) )
            {
                throw new GameObjectException( "Cannot set the parent to itself", this );
            }

            // Don't do anything special if we are not changing parents
            if ( ReferenceEquals( mParent, newParent ) )
            {
                return;
            }

            // Do we need to remove ourself from the parent's children list?
            if ( mParent != null )
            {
                mParent.Children.Remove( newParent );
            }

            // Do we need to add ourself as a child to this parent?
            if ( newParent != null )
            {
                newParent.Children.Add( this );
            }

            // Save the new parent... and we're done!
            mParent = newParent;
        }

        public string DumpDebugInfoToString()
        {
            StringBuilder debugText = new StringBuilder();

            debugText.Append( "\t{\n" );
            debugText.Append( String.Format( "\t\tid:     {0}\n",     Id ) );
            debugText.Append( String.Format( "\t\tname:   \"{0}\"\n", Name ) );
            debugText.Append( String.Format( "\t\tpos:    {0}\n",     Position ) );
            debugText.Append( String.Format( "\t\tbounds: {0}\n",     Bounds.ToString() ) );
            debugText.Append( String.Format( "\t\tdir:    \"{0}\"\n", Direction.ToString() ) );
            debugText.Append( "\t\tcomponents: [\n" );

            // List the components attached to this game object (only the basics)
            foreach ( KeyValuePair<System.Type, IGameObjectComponent> pair in mComponents )
            {
                debugText.Append(
                    String.Format( "\t\t\t{{ type: \"{0}\", id: {1}, enabled: {2} }}\n",
                                   pair.Value.GetType().Name,
                                   pair.Value.Id,
                                   pair.Value.Enabled ) );
            }

            debugText.Append( "\t\t]\n" );
            debugText.Append( "\t},\n" );
            return debugText.ToString();
        }
    }
}

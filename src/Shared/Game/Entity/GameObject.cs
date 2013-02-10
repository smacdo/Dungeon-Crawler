using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Scott.Game.Graphics;
using Scott.Geometry;
using Scott.GameContent;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    /// Represents a physical object in the game world
    /// </summary>
    public class GameObject : Scott.Game.Entity.IGameObject
    {
        private const int DEFAULT_COMPONENT_COUNT = 7;

        /// <summary>
        /// This game object's name
        /// </summary>
        public string Name
        {
            get
            {
                return mName;
            }
        }

        /// <summary>
        /// Unique identifier for this game object
        /// </summary>
        public Guid Id
        {
            get
            {
                return mId;
            }
        }

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

        /// <summary>
        ///  Bounding area for the game object.
        ///   TODO: Remove this and put it into another component.
        /// </summary>
        public BoundingArea Bounds
        {
            get
            {
                return mBoundingArea;
            }
            set
            {
                mBoundingArea = value;
            }
        }

        /// <summary>
        ///  Check if game object is enabled or disabled;
        /// </summary>
        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = false;
                // TODO: something?
            }
        }

        /// <summary>
        /// Get the direciton this object is facing
        /// </summary>
        public Direction Direction
        {
            get
            {
                return mDirection;
            }
            set
            {
                mDirection = value;
            }
        }

        private string mName = String.Empty;
        private Guid mId = Guid.Empty;
        private GameObject mParent;
        private Vector2 mPosition;
        private BoundingArea mBoundingArea = null;
        private bool mEnabled = false;
        private Direction mDirection = Direction.South;
        private GameObjectCollection mCollection;
        private Dictionary< System.Type, IComponent > mComponents = new Dictionary<Type, IComponent>( DEFAULT_COMPONENT_COUNT );

        /// <summary>
        ///  Creates a new empty game object that is not associated with any collection.
        /// </summary>
        public GameObject()
        {
        }

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
            mName = name;
            mPosition = position;
            mParent = null;
            mDirection = direction;
            mEnabled = true;

            mCollection = parentCollection;
            mId = Guid.NewGuid();
        }

        /// <summary>
        /// Adds a game component to this game object
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
        public void DeleteComponent<T>() where T : Component
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
            foreach ( KeyValuePair<System.Type, IComponent> pair in mComponents )
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

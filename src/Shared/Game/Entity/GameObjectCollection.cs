using Microsoft.Xna.Framework;
using Scott.Game.Entity;
using Scott.Game.Entity.Actor;
using Scott.Game.Entity.AI;
using Scott.Game.Entity.Graphics;
using Scott.Game.Entity.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    ///  Manages the creation, destruction and execution of multiple game objects and their
    ///  components.
    /// </summary>
    public class GameObjectCollection
    {
        private const int DEFAULT_CAPACITY = 4096;

        public List<GameObject> GameObjects { get; private set; }

        public Dictionary<Type, IComponentCollection> mComponentProviders;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObjectCollection()
        {
            GameObjects = new List<GameObject>( DEFAULT_CAPACITY );
            mComponentProviders = new Dictionary<Type, IComponentCollection>();

            AddComponentProvider( typeof( AiController ), typeof( ComponentCollection<AiController> ) );
            AddComponentProvider( typeof( ActorController ), typeof( ComponentCollection<ActorController> ) );
            AddComponentProvider( typeof( MovementComponent ), typeof( MovementProvider ) );
            AddComponentProvider( typeof( SpriteComponent ), typeof( ComponentCollection<SpriteComponent> ) );
        }

        /// <summary>
        ///  Adds a new component type of component that can be added to game objects in this
        ///  collection.
        ///  
        ///  TODO: Create a new version of the component collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddComponentProvider( Type componentType, Type componentCollectionType )
        {
            // Create a new instance of the component collection
            IComponentCollection provider =
                (IComponentCollection) Activator.CreateInstance( componentCollectionType );

            // Register it in our list of providers
            mComponentProviders.Add( componentType, provider );
        }

        /// <summary>
        ///  Create a new component and attach it to the game object.
        /// </summary>
        /// <typeparam name="T">Component type to create.</typeparam>
        /// <param name="gameObject">Game object to attach the game object to.</param>
        /// <returns>Reference to the component if it needs to be configured.</returns>
        public T Attach<T>( IGameObject gameObject ) where T : class, IComponent
        {
            IComponentCollection collection = mComponentProviders[typeof( T )];
            return collection.Create( gameObject ) as T;
        }

        /// <summary>
        /// Creates and returns a new game object instance. This game object is tracked by this 
        /// collection until removed.
        /// </summary>
        /// <param name="name">Unique name for the game object.</param>
        /// <returns>Reference to the newly created game object.</returns>
        public GameObject Create( string name )
        {
            return Create( name, Vector2.Zero, Direction.South );
        }

        /// <summary>
        /// Creates and returns a new game object instance. This game object is tracked
        /// and updated until it is deleted from the collection.
        /// </summary>
        /// <param name="position">World position of the game objet</param>
        /// <param name="direction">Direciton of the game object</param>
        /// <param name="width">Width of the game object</param>
        /// <param name="height">Height of the game object</param>
        /// <returns></returns>
        public GameObject Create( string name, Vector2 position, Direction direction )
        {
            GameObject gameObject = new GameObject( name, this );
            GameObjects.Add( gameObject );

            gameObject.Transform.Position = position;
            gameObject.Transform.Direction = direction;

            return gameObject; 
        }

        /// <summary>
        ///  Updates all components of the requested type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="simulationTime"></param>
        public void Update<T>( GameTime simulationTime ) where T : IComponent
        {
            IComponentCollection collection = mComponentProviders[typeof( T )];
            collection.Update( simulationTime );
        }

        public void Draw<T>( GameTime gameTime ) where T : IComponent
        {
            // TODO: Horrible hack that needs to get fixed correctly.
            IComponentCollection collection = mComponentProviders[typeof( T )];
            ComponentCollection<SpriteComponent> drawables = (ComponentCollection<SpriteComponent>) collection;

            foreach ( SpriteComponent sprite in drawables )
            {
                sprite.Draw( gameTime );
            }
        }

       
        /// <summary>
        /// Dumps a debugging information about the current game object collection a string, suitable
        /// for disable in a console or a text file.
        /// </summary>
        /// <returns></returns>
        public string DumpDebugInfoToString()
        {
            StringBuilder debugText = new StringBuilder();

            // First dump all created game objects to disk
            debugText.Append( "GameObjects: [\n" );

            foreach ( GameObject gameObject in GameObjects )
            {
                debugText.Append( gameObject.DumpDebugInfoToString() );
            }

            debugText.Append( "],\n\n" );

            // Now dump our component managers to disk
            foreach ( KeyValuePair<Type, IComponentCollection> pair in mComponentProviders )
            {
                debugText.Append( pair.Value.DumpDebugInfoToString() );
                debugText.Append( "\n\n" );
            }

            return debugText.ToString();
        }
    }
}

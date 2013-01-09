using Microsoft.Xna.Framework;
using Scott.Dungeon.Actor;
using Scott.Dungeon.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Manages the creation, destruction and execution of multiple game objects and their
    /// components.
    /// </summary>
    public class GameObjectCollection
    {
        private const int DEFAULT_CAPACITY = 4096;

        public List<GameObject> GameObjects { get; private set; }
        public ComponentManager<CharacterSprite> CharacterSprites { get; private set; }
        public ComponentManager<AiController> AiControllers { get; private set; }
        public ComponentManager<ActorController> ActorControllers { get; private set; }
        public ComponentManager<Movement> Movements { get; private set; }
        public ComponentManager<Collider> Colliders { get; private set; }

        private UniqueIdManager mIdManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObjectCollection()
        {
            GameObjects = new List<GameObject>( DEFAULT_CAPACITY );
            CharacterSprites = new ComponentManager<CharacterSprite>( DEFAULT_CAPACITY );
            AiControllers = new ComponentManager<AiController>( DEFAULT_CAPACITY );
            ActorControllers = new ComponentManager<ActorController>( DEFAULT_CAPACITY );
            Movements = new ComponentManager<Movement>( DEFAULT_CAPACITY );
            Colliders = new ComponentManager<Collider>( DEFAULT_CAPACITY );

            mIdManager = new UniqueIdManager();
        }

        /// <summary>
        /// Creates and returns a new game object instance. This game object is tracked
        /// and updated until it is deleted from the collection.
        /// </summary>
        /// <returns>Copy of the newly created game object</returns>
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
            GameObject gameObject = new GameObject( name,
                                                    this,
                                                    mIdManager.AllocateId(),
                                                    position,
                                                    direction );
            GameObjects.Add( gameObject );

            return gameObject; 
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
            debugText.Append( CharacterSprites.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            debugText.Append( AiControllers.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            debugText.Append( Movements.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            debugText.Append( Colliders.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            return debugText.ToString();
        }
    }
}

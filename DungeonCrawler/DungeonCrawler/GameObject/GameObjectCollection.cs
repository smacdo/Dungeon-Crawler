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
        public ComponentManager<AiController> AiControllers { get; private set; }
        public ComponentManager<ActorController> ActorControllers { get; private set; }
        public ComponentManager<MovementComponent> Movements { get; private set; }
        public ComponentManager<ColliderComponent> Colliders { get; private set; }
        public ComponentManager<SpriteComponent> Sprites { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObjectCollection()
        {
            GameObjects = new List<GameObject>( DEFAULT_CAPACITY );
            AiControllers = new ComponentManager<AiController>( DEFAULT_CAPACITY );
            ActorControllers = new ComponentManager<ActorController>( DEFAULT_CAPACITY );
            Movements = new ComponentManager<MovementComponent>( DEFAULT_CAPACITY );
            Colliders = new ComponentManager<ColliderComponent>( DEFAULT_CAPACITY );
            Sprites = new ComponentManager<SpriteComponent>( DEFAULT_CAPACITY );
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
                                                    position,
                                                    direction );
            GameObjects.Add( gameObject );

            return gameObject; 
        }

        public void Update( GameTime simulationTime )
        {
            // We resolve movement and collision first, before the player or AI gets chance
            // to do anything. Hence the current position of all objects (and collision)
            // that is displayed is actually one frame BEFORE this update
            Movements.Update( simulationTime );
            PerformCollisionDetection();

            Colliders.Update( simulationTime );

            // Update game ai and character actions
            AiControllers.Update( simulationTime );
            ActorControllers.Update( simulationTime );

            // Make sure animations are primed and updated (we need to trigger the
            // correct animation events even if we are not drawwing)
            Sprites.Update( simulationTime );
        }

        /// <summary>
        /// TODO: Handle things htat move really fast and wouldn't get detected in this fashion
        /// (eg, need to do movements together with collision detection... and calculate where
        /// they are ending up)
        /// </summary>
        /// <param name="simulationTime"></param>
        private void PerformCollisionDetection()
        {
            // this is deliciously horrible
            foreach ( GameObject outterObject in GameObjects )
            {
                // Reject objects that do not have boundaries or are not set up to receive
                // collisions
                ColliderComponent collider = outterObject.GetComponent<ColliderComponent>();

                if ( collider == null || outterObject.Bounds == null )
                {
                    continue;
                }
                else
                {
                    // Unmark any collisions that happened on the last update cycle
                    collider.HadCollision = false;
                }

                // Test this object against all the other game objects in this scene
                foreach ( GameObject innerObject in GameObjects )
                {
                    // don't test if this object does not have bounds, or is the same
                    // object
                    if ( innerObject.Bounds == null || innerObject == outterObject )
                    {
                        continue;
                    }

                    // do they collide with each other?
                    if ( outterObject.Bounds.Intersects( innerObject.Bounds ) )
                    {
                        collider.HadCollision = true;
                    }
                }
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
            debugText.Append( AiControllers.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            debugText.Append( ActorControllers.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            debugText.Append( Movements.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            debugText.Append( Colliders.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            debugText.Append( Sprites.DumpDebugDumpDebugInfoToString() );
            debugText.Append( "\n\n" );

            return debugText.ToString();
        }
    }
}

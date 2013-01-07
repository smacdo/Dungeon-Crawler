using Microsoft.Xna.Framework;
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
        public ComponentManager<CharacterSprite> CharacterSprites;
        public ComponentManager<AiController> AiControllers;
        public MovementManager Movements;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObjectCollection()
        {
            CharacterSprites = new ComponentManager<CharacterSprite>();
            AiControllers = new ComponentManager<AiController>();
            Movements = new MovementManager();
        }

        /// <summary>
        /// Creates and returns a new game object instance. This game object is tracked
        /// and updated until it is deleted from the collection.
        /// </summary>
        /// <returns>Copy of the newly created game object</returns>
        public GameObject Create()
        {
            return Create( Vector2.Zero, Direction.South, 0, 0 );
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
        public GameObject Create( Vector2 position, Direction direction, int width, int height )
        {
            return new GameObject( position, direction, width, height );
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace scott.dungeon
{
    /// <summary>
    /// Represents a interactive character
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// The game object that this component belongs to
        /// </summary>
        private GameObject mGameObject;

        /// <summary>
        /// Constructor
        /// </summary>
        public Actor( GameObject gameObject )
        {
            mGameObject = gameObject;
        }

        public void Move( Direction direction, int speed )
        {
            // TODO: Turn this into an action
            Movement movement = mGameObject.Movement;
            Debug.Assert( movement != null );

            movement.Speed = speed;
            movement.Heading = direction;
        }

        /// <summary>
        /// Updates the state of the game actor
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime )
        {

        }
    }
}

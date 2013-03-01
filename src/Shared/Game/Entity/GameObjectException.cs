using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    /// Keeps track of game object exceptions
    /// </summary>
    public class GameObjectException : Exception
    {
        public GameObject GameObject { get; private set; }

        public GameObjectException( string message, GameObject gameObject )
            : base( message )
        {
            GameObject = gameObject;
        }
    }
}

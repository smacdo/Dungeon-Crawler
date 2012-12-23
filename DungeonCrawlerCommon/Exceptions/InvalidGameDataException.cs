using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon
{
    /// <summary>
    /// Thrown when the game encounters a situation that was caused by bad game data (or misconfiguration).
    /// </summary>
    public class InvalidGameDataException : SystemException
    {
        public InvalidGameDataException() : base() { }
        public InvalidGameDataException( string message ) : base( message ) { }
        public InvalidGameDataException( string message, System.Exception inner ) : base( message, inner ) { }
    }
}

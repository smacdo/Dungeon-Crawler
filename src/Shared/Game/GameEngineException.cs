using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game
{
    /// <summary>
    ///  Represents an exception with the game engine.
    /// </summary>
    public class GameEngineException : System.Exception
    {
        public GameEngineException( string message )
            : base( message )
        {
            // empty
        }

        public GameEngineException( string message, System.Exception innerException )
            : base( message, innerException )
        {
            // empty
        }
    }
}

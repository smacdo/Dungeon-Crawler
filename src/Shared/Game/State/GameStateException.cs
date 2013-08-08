using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.State
{
    /// <summary>
    ///  Represents a runtime error with the game state system.
    /// </summary>
    public class GameStateException : GameEngineException
    {
        public GameStateException( string message )
            : base( message )
        {
            // empty
        }
    }
}

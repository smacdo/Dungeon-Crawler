using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Input
{
    /// <summary>
    ///  Represents a runtime error with the input system.
    /// </summary>
    public class InputException : GameEngineException
    {
        public InputException( string message )
            : base( message )
        {
            // empty
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common
{
    /// <summary>
    /// Thrown when the game encounters a situation that was caused by bad game data (or misconfiguration).
    /// </summary>
    public class ParentingException : Exception
    {
        public ParentingException() : base() { }
        public ParentingException( string message ) : base( message ) { }
        public ParentingException( string message, System.Exception inner ) : base( message, inner ) { }
    }
}

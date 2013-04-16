using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Content
{
    /// <summary>
    ///  Represents an exception while loading game data.
    /// </summary>
    public class GameContentException : GameEngineException
    {
        /// <summary>
        ///  Create a new GameDataException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public GameContentException( string message )
            : base( message )
        {
            // Empty
        }

        /// <summary>
        ///  Create a new GameDataException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="filename">Name of the content file causing the exception.</param>
        public GameContentException( string message, string filename )
            : base ( "Error reading {0}: {1}".With( filename, message ) )
        {
            // Empty
        }

        /// <summary>
        ///  Create a new GameDataException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="filename">Name of the content file causing the exception.</param>
        public GameContentException( string message, string filename, Exception inner )
            : base( "Error reading {0}: {1}".With( filename, message ), inner )
        {
            // Empty
        }
    }
}

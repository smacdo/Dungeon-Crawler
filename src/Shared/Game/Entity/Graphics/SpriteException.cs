using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity.Graphics
{
    public class SpriteException : GameEngineException
    {
        public SpriteException( string message )
            : base( message )
        {
            // empty
        }
    }
}

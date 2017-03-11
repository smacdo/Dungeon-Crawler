using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.Forge.Engine
{
    /// <summary>
    ///  Runtime game settings.
    /// </summary>
    public class GameSettings
    {
        public bool DrawPhysicsDebug { get; set; } = true;
        public bool DrawSpriteDebug { get; set; } = true;
        public bool DrawTransformDebug { get; set; } = true;
    }
}

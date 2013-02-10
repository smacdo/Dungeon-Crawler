using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    ///  Interface for components
    /// </summary>
    public interface IComponentCollection
    {
        IComponent Create( IGameObject owner );
        void Update( GameTime simulationTime );
        string DumpDebugDumpDebugInfoToString();
    }
}

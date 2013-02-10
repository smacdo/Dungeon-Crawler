using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    ///  Interface for game object components
    /// </summary>
    public interface IComponent
    {
        Guid Id { get; set; }
        IGameObject Owner { get; set; }
        bool Enabled { get; set; }

        void Update( GameTime simulationTime );

        string ToString();
        string DumpDebugInfo();
    }
}

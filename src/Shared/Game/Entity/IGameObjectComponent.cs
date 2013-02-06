using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    /// Interface for game object components
    /// </summary>
    public interface IGameObjectComponent
    {
        Guid Id { get; }
        GameObject Owner { get; set; }
        bool Enabled { get; set; }

        void Update( GameTime time );

        void ResetComponent( GameObject gameObject, bool enabled );

        string ToString();
        string DumpDebugInfo();
    }
}

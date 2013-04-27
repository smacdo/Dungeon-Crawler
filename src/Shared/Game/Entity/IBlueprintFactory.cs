using Scott.Game.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity
{
    /// <summary>
    ///  Creates GameObjects from blueprints.
    /// </summary>
    public interface IBlueprintFactory
    {
        GameObjectCollection Collection { get; set; }
        ContentManagerX Content { get; set; }
        GameObject Instantiate( string blueprintName );
    }
}

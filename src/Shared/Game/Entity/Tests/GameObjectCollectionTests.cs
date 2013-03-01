using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity.Tests
{
    /// <summary>
    ///  Tests for GameObjectCollection
    /// </summary>
    [TestFixture]
    [Category( "Entity" )]
    public class GameObjectCollectionTests
    {
        /// <summary>
        ///  Create empty collection just to see if it works
        /// </summary>
        [Test]
        public void CreateNewCollection()
        {
            GameObjectCollection collection = new GameObjectCollection();
        }
    }
}

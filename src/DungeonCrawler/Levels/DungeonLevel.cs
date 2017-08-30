using Forge;
using Forge.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.DungeonCrawler.Levels
{
    public class DungeonLevel
    {
        public TileMap TileMap { get; set; }
        public Point2 StairsUpPoint { get; set; }
        public List<Point2> SpawnPoints { get; set; } = new List<Point2>();
    }
}

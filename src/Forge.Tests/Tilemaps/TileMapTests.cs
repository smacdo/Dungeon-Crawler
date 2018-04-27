using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.Graphics;
using Forge.Tilemaps;
using Forge.Spatial;

namespace Forge.Tests.Tilemaps
{
    [TestClass]
    public class TileMapTests
    {
        [TestMethod]
        public void Get_Tile_Edges_When_Camera_Is_In_Center_Of_Map()
        {
            // This method generates a tile grid that is larger than the camera view, and the camera is placed
            // close to the center of the tile grid. No part of the camera's view is past the boundaries of the
            // grid.
            var tilemap = new TileMap(new TileSet(null, 10, 8), new Grid<Tile>(20, 20));
            var camera = new Camera(new SizeF(60, 32), new Vector2(100, 80));

            // Check that the sceen space top left and bottom right values are correct.
            var worldViewRect = camera.ScreenToWorld(new RectF(0, 0, camera.Viewport.Width, camera.Viewport.Height));

            Assert.AreEqual(worldViewRect.TopLeft, new Vector2(70, 64));
            Assert.AreEqual(worldViewRect.BottomRight, new Vector2(130, 96));

            // Check that the top left and bottom right grid cells are correct.
            var topLeftTileIndex = camera.LeftmostVisbileTile(tilemap);
            Assert.AreEqual(new Point2(6, 7), topLeftTileIndex);

            var bottomRightTileIndex = camera.GetBottomRightmostVisibleTile(tilemap);
            Assert.AreEqual(new Point2(13, 12), bottomRightTileIndex);
        }
        
        [TestMethod]
        public void Get_Tile_Edges_When_Camera_On_Top_Left_Corner_And_Clamps_To_Zero()
        {
            var tilemap = new TileMap(new TileSet(null, 10, 8), new Grid<Tile>(20, 20));
            var camera = new Camera(new SizeF(40, 40), new Vector2(-10, -4));

            // Check that the top left and bottom right grid cells are correct.
            var topLeftTileIndex = camera.LeftmostVisbileTile(tilemap);
            Assert.AreEqual(new Point2(0, 0), topLeftTileIndex);

            var bottomRightTileIndex = camera.GetBottomRightmostVisibleTile(tilemap);
            Assert.AreEqual(new Point2(1, 2), bottomRightTileIndex);
        }

        // TODO: Test more edge conditions.
    }
}

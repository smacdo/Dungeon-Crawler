using System;
using System.Collections.Generic;
using Forge.Graphics;
using Forge.Tilemaps;
using Forge;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Forge.Tests.TestMocks
{
    /// <summary>
    ///  Special testable implementation of game renderer.
    /// </summary>
    public class TestableGameRenderer : IGameRenderer
    {
        public IList<DrawCallDetails> Draws = new List<DrawCallDetails>();

        public void Clear()
        {
            Draws.Clear();
        }

        public void Draw(Texture2D atlas, RectF atlasRect, Forge.Vector2 screenPosition, float rotation = 0)
        {
            Draws.Add(new DrawCallDetails
            {
                Atlas = atlas,
                AtlasRect = atlasRect,
                ScreenPosition = screenPosition,
                Rotation = rotation
            });
        }

        public void DrawCollisionMap(Camera camera, TileMap tilemap)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Forge.Vector2 startPosition, Forge.Vector2 endPosition, Color? color, float? width)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(RectF rect, Color? fillColor = default(Color?), Color? borderColor = default(Color?), float? borderSize = default(float?))
        {
            throw new NotImplementedException();
        }

        public void DrawText(string text, Forge.Vector2 position, SpriteFont font, Color? textColor = default(Color?), Color? backgroundColor = default(Color?))
        {
            throw new NotImplementedException();
        }

        public void DrawTilemap(TileMap tilemap)
        {
            throw new NotImplementedException();
        }

        public void DrawTilemap(Camera camera, TileMap tilemap)
        {
            throw new NotImplementedException();
        }

        public void FinishDrawing()
        {
            throw new NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void StartDrawing(bool clearScreen = false)
        {
            throw new NotImplementedException();
        }
        
        public struct DrawCallDetails
        {
            public Microsoft.Xna.Framework.Graphics.Texture2D Atlas;
            public RectF AtlasRect;
            public Forge.Vector2 ScreenPosition;
            public float Rotation;
        }
    }
}

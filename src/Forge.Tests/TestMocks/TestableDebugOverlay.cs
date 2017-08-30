using System;
using Microsoft.Xna.Framework;
using Forge.Graphics;
using Forge.Spatial;
using Forge;

namespace Forge.Tests.TestMocks
{
    public class TestableDebugOverlay : IDebugOverlay
    {
        public void Draw(GameTime renderTime, IGameRenderer renderer)
        {
            // Empty.
        }

        public void DrawBoundingArea(BoundingArea bounds, Color? borderColor = default(Color?), float? borderSize = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawBoundingRect(BoundingRect rect, Color? borderColor = default(Color?), float? borderSize = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawFilledRect(RectF dimensions, Color? color = default(Color?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawLine(Forge.Vector2 start, Forge.Vector2 end, Color? color = default(Color?), float? width = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawPoint(Forge.Vector2 point, Color? color = default(Color?), float? sizeIn = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawRectBorder(Rectangle dimensions, Color? borderColor = default(Color?), float? borderSize = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawRectBorder(RectF dimensions, Color? borderColor = default(Color?), float? borderSize = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawTextBox(string text, Forge.Vector2 pos, Color? textColor, Color? backgroundColor, TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void PreUpdate(GameTime gameTime)
        {
            // Empty.
        }

        public void Unload()
        {
            // Empty.
        }
    }
}

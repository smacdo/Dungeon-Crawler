using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Scott.Forge.Engine.Graphics;

namespace Scott.Forge.Engine.Tests.TestMocks
{
    public class TestableDebugOverlay : IDebugOverlay
    {
        public void Draw(GameTime renderTime)
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

        public void DrawLine(Vector2 start, Vector2 end, Color? color = default(Color?), float? width = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
        {
            // Empty.
        }

        public void DrawPoint(Vector2 point, Color? color = default(Color?), float? sizeIn = default(float?), TimeSpan? timeToLive = default(TimeSpan?))
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

        public void DrawTextBox(string text, Vector2 pos, Color? textColor, Color? backgroundColor, TimeSpan? timeToLive = default(TimeSpan?))
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

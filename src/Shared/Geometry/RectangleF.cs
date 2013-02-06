using Microsoft.Xna.Framework;
using System;

namespace Scott.Geometry
{
    /// <summary>
    /// Represents a rectangle using a floating point values
    /// </summary>
    public struct RectangleF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public static readonly RectangleF Empty = new RectangleF( 0.0f, 0.0f, 0.0f, 0.0f );

        public float Top
        {
            get
            {
                return Y;
            }
        }

        public float Left
        {
            get
            {
                return X;
            }
        }

        public float Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        public float Right
        {
            get
            {
                return X + Width;
            }
        }

        public Vector2 Location
        {
            get
            {
                return new Vector2( X, Y );
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2( Width, Height );
            }
        }

        /// <summary>
        ///  Tests if the rectangle has a zero width or height.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Width == 0.0f || Height == 0.0f;
            }
        }

        /// <summary>
        ///  Rectangle constructor
        /// </summary>
        /// <param name="x">Upper x coordinate</param>
        /// <param name="y">Upper y coordinate</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        public RectangleF( float x, float y, float width, float height )
            : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}

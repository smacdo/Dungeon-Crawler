/*
 * Copyright 2012-2014 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Scott.Forge
{
    /// <summary>
    ///  Describes how to scale a size object with respect to its current aspect ratio.
    /// </summary>
    [DataContract]
    public enum AspectRatioMode
    {
        [EnumMember] Ignore,
        [EnumMember] Keep,
        [EnumMember] KeepByExpanding
    }

    /// <summary>
    ///  Represents a width and a height as a size.
    /// </summary>
    [DataContract]
    public struct SizeF : IEquatable<SizeF>, IComparable<SizeF>, IComparable
    {
        private float mWidth;
        private float mHeight;

        /// <summary>
        ///  Initializes a new instance of the <see cref="SizeF"/> struct.
        /// </summary>
        /// <param name="width">Initial width.</param>
        /// <param name="height">Initial height.</param>
        public SizeF( float width, float height )
        {
            if ( width <= 0.0f )
            {
                throw new ArgumentException( "Width cannot be zero or negative" );
            }
            else if ( height <= 0.0f )
            {
                throw new ArgumentException( "Height cannot be zero or negative" );
            }

            mWidth = width;
            mHeight = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeF"/> struct.
        /// </summary>
        /// <param name="inputSize">Initial size.</param>
        public SizeF(Vector2 inputSize)
        {
            if (inputSize.X <= 0.0f)
            {
                throw new ArgumentException( "Width cannot be zero or negative" );
            }
            else if (inputSize.Y <= 0.0f)
            {
                throw new ArgumentException( "Height cannot be zero or negative" );
            }

            mWidth = inputSize.X;
            mHeight = inputSize.Y;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="SizeF"/> struct.
        /// </summary>
        /// <param name="inputSize">Initial size.</param>
        public SizeF(SizeF inputSize)
        {
            mWidth = inputSize.Width;
            mHeight = inputSize.Height;
        }

        /// <summary>
        ///  Get or set the width. Must always be zero or larger.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "width", Order = 0, IsRequired = true)]
        public float Width
        {
            get { return mWidth; }
            set
            {
                if ( value < 0.0f )
                {
                    throw new ArgumentException( "Width cannot be less than zero", "value" );
                }
                else
                {
                    mWidth = value;
                }
            }
        }

        /// <summary>
        ///  Get or set the height. Must always be zero or larger.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "height", Order = 1, IsRequired = true)]
        public float Height
        {
            get { return mHeight; }
            set
            {
                if ( value < 0.0f )
                {
                    throw new ArgumentException( "Height cannot be less than zero", "value" );
                }
                else
                {
                    mHeight = value;
                }
            }
        }

        /// <summary>
        ///  Get if the size is "empty", which means either the width or the height is zero.
        /// </summary>
        public bool IsEmpty
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return mWidth == 0.0f || mHeight == 0.0f; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Compares this object to another object.
        /// </summary>
        /// <remarks>
        ///  The SizeF uses size when comparing two objects. The SizeF object with the smaller area
        ///  will be ranked lower than an object with a higher size.
        /// </remarks>
        /// <returns>
        ///  Negative one if this object precedes the other object, zero if they are equal and
        ///  positive one if the other object precedes this object.
        /// </returns>
        public int CompareTo( SizeF other )
        {
            float myArea = Width * Height;
            float otherArea = other.Width * other.Height;

            if ( myArea < otherArea )
            {
                return -1;
            }
            else if ( myArea > otherArea )
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        ///  Compares this pair to another object.
        /// </summary>
        /// <returns>
        ///  Negative one if this object precedes the other object, zero if they are equal and
        ///  positive one if the other object precedes this object.
        /// </returns>
        int IComparable.CompareTo( object other )
        {
            if ( other is SizeF )
            {
                return CompareTo( (SizeF) other );
            }
            else
            {
                throw new ArgumentException("Cannot compare, passed object is not SizeF", "other");
            }
        }


        /// <summary>
        ///  Swap the width and height, and return the result.
        /// </summary>
        /// <remarks>
        ///  This object is not mutated by this method. 
        /// </remarks>
        /// <returns>The transposed size.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Tranposed" )]
        public SizeF Tranposed()
        {
            return new SizeF( mHeight, mWidth );
        }

        // Rename Scale to Resize

        /// <summary>
        ///  Scale this object using the given scaling parameters and return the result.
        /// </summary>
        /// <remarks>
        ///  This object is not mutated by this method.
        /// </remarks>
        /// <param name="newWidth">New size width.</param>
        /// <param name="newHeight">New size height.</param>
        /// <param name="mode">Aspect ratio preservation mode to use.</param>
        /// <returns>Scaled size.</returns>
        public SizeF Scale(SizeF newSize, AspectRatioMode mode = AspectRatioMode.Ignore)
        {
            return Scale(newSize.Width, newSize.Height, mode);
        }

        /// <summary>
        ///  Scale this object using the given scaling parameters and return the result.
        /// </summary>
        /// <remarks>
        ///  This object is not modified by this method.
        /// </remarks>
        /// <param name="newWidth">New size width.</param>
        /// <param name="newHeight">New size height.</param>
        /// <param name="mode">Aspect ratio preservation mode to use.</param>
        /// <returns>Scaled size.</returns>
        public SizeF Scale(float newWidth, float newHeight, AspectRatioMode mode = AspectRatioMode.Ignore)
        {
            // Make sure our new width and height values are legitimate.
            if (newWidth <= 0.0f)
            {
                throw new ArgumentException("Cannot scale if new width is zero or negative");
            }
            else if (newHeight <= 0.0f)
            {
                throw new ArgumentException("Cannot scale if new height is zero or negative");
            }

            // Scale the size according to the requested scaling mode.
            if (mode != AspectRatioMode.Ignore)
            {
                bool useHeight = false;
                double rw = (double) (newHeight * newWidth) / (double) (newHeight);

                // How do we want to scale the size?
                if (mode == AspectRatioMode.Keep)
                {
                    useHeight = (rw <= newWidth);
                }
                else // KeepAspectRatioByExpanding
                {
                    useHeight = (rw >= newHeight);
                }

                // Scale the size...
                if (useHeight)
                {
                    newWidth = (float) rw;
                }
                else
                {
                    newHeight = newWidth * Height / Width;
                }
            }

            // Return the scaled size.
            return new SizeF(newWidth, newHeight);
        }

        /// <summary>
        ///  Return a vector representing this size.
        /// </summary>
        /// <returns>A vector containing this size.</returns>
        public Vector2 ToVector2()
        {
            return new Vector2( Width, Height );
        }

        /// <summary>
        ///  Get the hash code for this instance.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked       // overflow is fine, just wrap
            {
                int hash = 17;

                hash *= 23 + mWidth.GetHashCode();
                hash *= 23 + mHeight.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///  Get a string representing this SizeF instance.
        /// </summary>
        public override string ToString()
        {
            return String.Format( "<size w: {0}, h: {1}>", mWidth, mHeight );
        }

        /// <summary>
        ///  Check if the other size is equal to this SizeF instance.
        /// </summary>
        public bool Equals( SizeF other )
        {
            return other.mWidth == mWidth && other.mHeight == mHeight;
        }

        /// <summary>
        ///  Check if the other object is equal to this SizeF instance.
        /// </summary>
        public override bool Equals( object obj )
        {
            if ( obj is SizeF )
            {
                return Equals( (SizeF) obj );
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==( SizeF left, SizeF right )
        {
            return left.Equals( right );
        }

        public static bool operator !=( SizeF left, SizeF right )
        {
            return !( left.Equals( right ) );
        }

        public static bool operator <( SizeF left, SizeF right )
        {
            return ( ( left.Width * left.Height ) < ( right.Width * right.Height ) );
        }

        public static bool operator <=( SizeF left, SizeF right )
        {
            return ( ( left.Width * left.Height ) <= ( right.Width * right.Height ) );
        }

        public static bool operator >( SizeF left, SizeF right )
        {
            return ( ( left.Width * left.Height ) > ( right.Width * right.Height ) );
        }

        public static bool operator >=( SizeF left, SizeF right )
        {
            return ( ( left.Width * left.Height ) >= ( right.Width * right.Height ) );
        }
    }
}
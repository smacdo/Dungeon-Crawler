/*
 * MathUtil.cs
 * Copyright 2012-2013 Scott MacDonald
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

namespace Scott.Common
{
    /// <summary>
    ///  Extra math utilities that are not normally found in System.Math.
    /// </summary>
    public static class MathX
    {
        /// <summary>
        ///  Normalize a value in the [min,max] range to [0.0,1.0]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [0.0,1.0] range.</returns>
        public static float NormalizeToZeroOneRange( float v, float min, float max )
        {
            return ( v - min ) / ( max - min );
        }

        /// <summary>
        ///  Normalize a value in the [min,max] range to [0.0,1.0]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [0.0,1.0] range.</returns>
        public static double NormalizeToZeroOneRange( double v, double min, double max )
        {
            return ( v - min ) / ( max - min );
        }

        /// <summary>
        ///  Clamps a value that is in the [min,max] range to [-1,1]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [-1.0,1.0] range.</returns>
        public static float NormalizeToNegativeOneOneRange( float v, float min, float max )
        {
            return 2.0f * ( v - min ) / ( max - min ) + -1.0f;
        }

        /// <summary>
        ///  Clamps a value that is in the [min,max] range to [-1,1]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [-1.0,1.0] range.</returns>
        public static double NormalizeToNegativeOneOneRange( double v, double min, double max )
        {
            return 2.0 * ( v - min ) / ( max - min ) + -1.0;
        }

        /// <summary>
        ///  Convert angle in degrees to angle in radians.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static float DegreeToRadian( float angle )
        {
            return (float) Math.PI * angle / 180.0f;
        }

        /// <summary>
        ///  Convert angle in radians to angle in degrees.
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static float RadianToDegree( float angle )
        {
            return angle * ( 180.0f / (float) Math.PI );
        }

        /// <summary>
        ///  Returns the smallest difference between two angle values in radians.
        /// </summary>
        /// <remarks>
        ///  This method requires the values a and b to be constrained to [0, 2PI].
        ///  Credit: http://gamedev.stackexchange.com/a/4472
        /// </remarks>
        /// <param name="a">Radian angle between 0 and 2 * PI.</param>
        /// <param name="b">Radian angle between 0 and 2 * PI.</param>
        /// <returns>Smallest difference between a and b.</returns>
        public static float SmallestAngleBetween( float a, float b )
        {
            return (float) Math.PI - Math.Abs( Math.Abs( a - b ) - (float) Math.PI );
        }

        /// <summary>
        ///  Returns the smallest difference between two angle values in degrees.
        /// </summary>
        /// <remarks>
        ///  This method requires the values a and b to be constrained to [0, 360].
        ///  Credit: http://gamedev.stackexchange.com/a/4472
        /// </remarks>
        /// <param name="a">Degree angle between 0 and 360.</param>
        /// <param name="b">Degree angle between 0 and 360.</param>
        /// <returns>Smallest difference between a and b.</returns>
        public static float SmallestDegreeAngleBetween( float a, float b )
        {
            return 180.0f - Math.Abs( Math.Abs( a - b ) - 180.0f );
        }

        /// <summary>
        ///  Check if two double values are nearly equal.
        /// </summary>
        /// <param name="a">First value to check.</param>
        /// <param name="b">Second value to check.</param>
        /// <param name="epsilon">Maximal difference before they are not equal.</param>
        /// <returns>If two double values are nearly equal.</returns>
        public static bool NearlyEqual( double a, double b, double epsilon )
        {
            double absA = Math.Abs( a );
            double absB = Math.Abs( b );
            double diff = Math.Abs( a - b );

            if ( a * b == 0.0 )
            {
                // A or b (or both) are zero. Relative error is therefore not meaningful here.
                return diff < ( epsilon * epsilon );
            }
            else
            {
                // Use relative error.
                return diff / ( absA + absB ) < epsilon;
            }
        }

        /// <summary>
        ///  Check if two floating point values are nearly equal.
        /// </summary>
        /// <param name="a">First value to check.</param>
        /// <param name="b">Second value to check.</param>
        /// <param name="epsilon">Maximal difference before they are not equal.</param>
        /// <returns>If two floating point values are nearly equal.</returns>
        public static bool NearlyEqual( float a, float b, float epsilon )
        {
            float absA = Math.Abs( a );
            float absB = Math.Abs( b );
            float diff = Math.Abs( a - b );

            if ( a * b == 0.0f )
            {
                // A or b (or both) are zero. Relative error is therefore not meaningful here.
                return diff < ( epsilon * epsilon );
            }
            else
            {
                // Use relative error.
                return diff / ( absA + absB ) < epsilon;
            }
        }

        /// <summary>
        ///  Calculate an approximated sin value using faster math than calling Math.Sin. X must
        ///  range between [0,PI].
        /// </summary>
        /// <remarks>
        ///  Due to C# having so-so math performance with JIT, it appears that using Math.Sin is
        ///  either just as fast, or ever so slightly slower than this method. It's probably not
        ///  worth using this method, but I'm keeping it around for historical purposes.
        ///  Credit: http://devmaster.net/forums/topic/4648-fast-and-accurate-sinecosine/
        /// </remarks>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float FastSin( float x )
        {
            const float B = 4.0f / (float) Math.PI;
            const float C = -4.0f / (float) ( Math.PI * Math.PI );
            const float P = 0.225f;

            float y0 = B * x + C * x * Math.Abs( x );
            float y1 = P * ( y0 * Math.Abs( y0 ) - y0 ) + y0;      // Q * y + P * y * abs(y)

            return y1;
        }

        /// <summary>
        ///  Return the next highest power of two that is larger than the given value.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        /// <returns>The next power of two larger than the given value.</returns>
        public static long NextPowerOf2( long value )
        {
            return (long) NextPowerOf2( (ulong) value );
        }

        /// <summary>
        ///  Return the next highest power of two that is larger than the given value.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        /// <returns>The next power of two larger than the given value.</returns>
        public static int NextPowerOf2( int value )
        {
            return (int) NextPowerOf2( (uint) value );
        }

        /// <summary>
        ///  Return the next highest power of two that is larger than the given value.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        /// <returns>The next power of two larger than the given value.</returns>
        public static ulong NextPowerOf2( ulong value )
        {
            if ( value == 0 )
            {
                return 1;
            }

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            value++;

            return value;
        }

        /// <summary>
        ///  Return the next highest power of two that is larger than the given value.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        /// <returns>The next power of two larger than the given value.</returns>
        public static uint NextPowerOf2( uint value )
        {
            if ( value == 0 )
            {
                return 1;
            }

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;

            return value;
        }

        /// <summary>
        ///  Linearly interpolate a value between [a,b] by an amount defined by t. T must vary
        ///  between zero and one.
        /// </summary>
        /// <remarks>
        ///  This function will perform a smooth linear interpolation of the range defined by
        ///  [a,b]. When t = 0, the value returned will be a. When t = 0.5, the value returned
        ///  will be halfway between a and b. And when t = 1, the value returned will be b.
        /// </remarks>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation amount, clamped to [0,1].</param>
        /// <returns>The interpolated value.</returns>
        public static float Lerp( float a, float b, float t )
        {
            return a + t * ( b - a );
        }

        /// <summary>
        ///  Linearly interpolate a value between [a,b] by an amount defined by t. T must vary
        ///  between zero and one.
        /// </summary>
        /// <remarks>
        ///  This function will perform a smooth linear interpolation of the range defined by
        ///  [a,b]. When t = 0, the value returned will be a. When t = 0.5, the value returned
        ///  will be halfway between a and b. And when t = 1, the value returned will be b.
        /// </remarks>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation amount, clamped to [0,1].</param>
        /// <returns>The interpolated value.</returns>
        public static double Lerp( double a, double b, double t )
        {
            return a + t * ( b - a );
        }

        /// <summary>
        ///  Returns the interpolation factor used to interpolate between two values. This is the
        ///  inverse of Lerp.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolate value from Lerp.</param>
        /// <returns>The interpolation amount, clamped to [0,1].</returns>
        public static float Unlerp( float a, float b, float t )
        {
            return ( t - a ) / ( b - a );
        }

        /// <summary>
        ///  Returns the interpolation factor used to interpolate between two values. This is the
        ///  inverse of Lerp.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolate value from Lerp.</param>
        /// <returns>The interpolation amount, clamped to [0,1].</returns>
        public static double Unlerp( double a, double b, double t )
        {
            return ( t - a ) / ( b - a );
        }
    }
}

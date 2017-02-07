/*
 * Copyright 2012-2015 Scott MacDonald
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

namespace Scott.Forge
{
    /// <summary>
    ///  Extra math utilities that are not normally found in System.Math.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        ///  Normalize a value in the [min,max] range to [0.0,1.0]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [0.0,1.0] range.</returns>
        public static float NormalizeToZeroOneRange(float v, float min, float max)
        {
            return (v - min) / (max - min);
        }

        /// <summary>
        ///  Normalize a value in the [min,max] range to [0.0,1.0]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [0.0,1.0] range.</returns>
        public static double NormalizeToZeroOneRange(double v, double min, double max)
        {
            return (v - min) / (max - min);
        }

        /// <summary>
        ///  Clamps a value that is in the [min,max] range to [-1,1]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [-1.0,1.0] range.</returns>
        public static float NormalizeToNegativeOneOneRange(float v, float min, float max)
        {
            return 2.0f * (v - min) / (max - min) + -1.0f;
        }

        /// <summary>
        ///  Clamps a value that is in the [min,max] range to [-1,1]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [-1.0,1.0] range.</returns>
        public static double NormalizeToNegativeOneOneRange(double v, double min, double max)
        {
            return 2.0 * (v - min) / (max - min) + -1.0;
        }

        /// <summary>
        ///  Convert angle in degrees to angle in radians.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static float DegreeToRadian(float angle)
        {
            return (float) Math.PI * angle / 180.0f;
        }

        /// <summary>
        ///  Convert angle in radians to angle in degrees.
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static float RadianToDegree(float angle)
        {
            return angle * (180.0f / (float) Math.PI);
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
        public static float SmallestAngleBetween(float a, float b)
        {
            return (float) Math.PI - Math.Abs(Math.Abs(a - b) - (float) Math.PI);
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
        public static float SmallestDegreeAngleBetween(float a, float b)
        {
            return 180.0f - Math.Abs(Math.Abs(a - b) - 180.0f);
        }

        /// <summary>
        ///  Check if two double values are nearly equal.
        /// </summary>
        /// <param name="a">First value to check.</param>
        /// <param name="b">Second value to check.</param>
        /// <param name="epsilon">Maximal difference before they are not equal.</param>
        /// <returns>If two double values are nearly equal.</returns>
        public static bool NearlyEqual(double a, double b, double epsilon)
        {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a == b)
            {
                return true;
            }
            else if (a == 0.0f || b == 0.0f || diff < double.Epsilon)
            {
                // A or b (or both) are zero. Relative error is therefore not meaningful here.
                return diff < (epsilon * double.Epsilon);
            }
            else
            {
                // Use relative error.
                return diff / (absA + absB) < epsilon;
            }
        }

        /// <summary>
        ///  Check if two floating point values are nearly equal.
        /// </summary>
        /// <param name="a">First value to check.</param>
        /// <param name="b">Second value to check.</param>
        /// <param name="epsilon">Maximal difference before they are not equal.</param>
        /// <returns>If two floating point values are nearly equal.</returns>
        public static bool NearlyEqual(float a, float b, float epsilon)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            {
                return true;
            }
            else if (a == 0.0f || b == 0.0f || diff < float.Epsilon)
            {
                // A or b (or both) are zero. Relative error is therefore not meaningful here.
                return diff < (epsilon * float.Epsilon);
            }
            else
            {
                // Use relative error.
                return diff / (absA + absB) < epsilon;
            }
        }

        /// <summary>
        ///  Check if two vectors are nearly equal.
        /// </summary>
        /// <param name="a">First value to check.</param>
        /// <param name="b">Second value to check.</param>
        /// <param name="epsilon">Maximal difference before they are not equal.</param>
        /// <returns>If two floating point values are nearly equal.</returns>
        public static bool NearlyEqual(Vector2 a, Vector2 b, float epsilon)
        {
            return NearlyEqual(a.X, b.X, epsilon) && NearlyEqual(a.Y, b.Y, epsilon);
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
        public static float FastSin(float x)
        {
            const float B = 4.0f / (float) Math.PI;
            const float C = -4.0f / (float) (Math.PI * Math.PI);
            const float P = 0.225f;

            float y0 = B * x + C * x * Math.Abs(x);
            float y1 = P * (y0 * Math.Abs(y0) - y0) + y0;      // Q * y + P * y * abs(y)

            return y1;
        }

        /// <summary>
        ///  Return the next highest power of two that is larger than the given value.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        /// <returns>The next power of two larger than the given value.</returns>
        public static long NextPowerOf2(long value)
        {
            return (long) NextPowerOf2((ulong) value);
        }

        /// <summary>
        ///  Return the next highest power of two that is larger than the given value.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        /// <returns>The next power of two larger than the given value.</returns>
        public static int NextPowerOf2(int value)
        {
            return (int) NextPowerOf2((uint) value);
        }

        /// <summary>
        ///  Return the next highest power of two that is larger than the given value.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        /// <returns>The next power of two larger than the given value.</returns>
        public static ulong NextPowerOf2(ulong value)
        {
            if (value == 0)
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
        public static uint NextPowerOf2(uint value)
        {
            if (value == 0)
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
        ///  Linearly remaps a value x in the range [xMin,xMax] to be in the range [oMin,oMax].
        /// </summary>
        /// <param name="x">Value to remap.</param>
        /// <param name="xMin">Input range minimum value.</param>
        /// <param name="xMax">Input range maximum value.</param>
        /// <param name="oMin">Output range minimum value.</param>
        /// <param name="oMax">Output range maximum value.</param>
        /// <returns>Linearly remapped value.</returns>
        public static double LinearRemap(
            double x,
            double xMin,
            double xMax,
            double oMin,
            double oMax)
        {
            // Get x interpolation amount from xMin to xMax.
            double xAmount = (x - xMin) / (xMax - xMin);

            // Now apply this interpolation factor to [oMin, oMax].
            return oMin + xAmount * (oMax - oMin);
        }

        /// <summary>
        ///  Linearly remaps a value x in the range [xMin,xMax] to be in the range [oMin,oMax].
        /// </summary>
        /// <param name="x">Value to remap.</param>
        /// <param name="xMin">Input range minimum value.</param>
        /// <param name="xMax">Input range maximum value.</param>
        /// <param name="oMin">Output range minimum value.</param>
        /// <param name="oMax">Output range maximum value.</param>
        /// <returns>Linearly remapped value.</returns>
        public static float LinearRemap(
            float x,
            float xMin,
            float xMax,
            float oMin,
            float oMax)
        {
            // Get x interpolation amount from xMin to xMax.
            float xAmount = (x - xMin) / (xMax - xMin);

            // Now apply this interpolation factor to [oMin, oMax].
            return oMin + xAmount * (oMax - oMin);
        }

        /// <summary>
        ///  Clamps a value to the range defined by [min,max].
        /// </summary>
        /// <typeparam name="T">Value type to clamp.</typeparam>
        /// <param name="v">Value to clamp.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Clamped value.</returns>
        public static T Clamp<T>(T v, T min, T max)
            where T : IComparable<T>
        {
            if (v.CompareTo(min) < 0)
            {
                return min;
            }
            else if (v.CompareTo(max) > 0)
            {
                return max;
            }
            else
            {
                return v;
            }
        }

        /// <summary>
        ///  Rotate Vector2 by the given angle. The angle must be specified in radians.
        /// </summary>
        /// <param name="v">Vector to rotate.</param>
        /// <param name="angle">Amount to rotate.</param>
        /// <returns>Rotated vector.</returns>
        public static Vector2 Rotate(Vector2 v, float angle)
        {
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);

            return new Vector2(
                (float) (v.X * c - v.Y * s),
                (float) (v.Y * c + v.X * s));
        }

        /// <summary>
        ///  Rotate Vector2 by the given angle. The angle must be specified in radians.
        /// </summary>
        /// <param name="v">Vector to rotate.</param>
        /// <param name="angle">Amount to rotate.</param>
        /// <returns>Rotated vector.</returns>
        public static Vector2 Rotate(float x, float y, float angle)
        {
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);

            return new Vector2(
                (float) (x * c - y * s),
                (float) (y * c + x * s));
        }

        /// <summary>
        ///  Gets a unit vector containing the direction going from the first vector to the second
        ///  vector.
        /// </summary>
        /// <returns>A unit direction vector.</returns>
        /// <param name="first">Vector to start at.</param>
        /// <param name="second">Vector to end at.</param>
        public static Vector2 GetDirectionTo(Vector2 first, Vector2 second)
        {
            if (first == second)
            {
                return Vector2.Zero;
            }

            return Vector2.Normalize(second - first);
        }

        /// <summary>
        ///  Normalizes a value in a wraparound numeric range.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <param name="lower">Lower wraparound boundary.</param>
        /// <param name="upper">Upper wraparound boundary.</param>
        /// <returns>Normalized input value.</returns>
        public static float Wrap(float value, float lower, float upper)
        {
            float distance = upper - lower;
            float times = (float) System.Math.Floor((value - lower) / distance);
            return value - (times * distance);
        }

        /// <summary>
        ///  Normalize an angle to [0, 360) range.
        /// </summary>
        /// <param name="value">Angle in degrees to normalize.</param>
        /// <returns>Normalized angle in degrees.</returns>
        public static float NormalizeAngle360(float value)
        {
            // Ref: http://stackoverflow.com/a/11498248
            var x = value % 360.0f;     // C# modulo operator supports float so no need to use fmod.
            
            if (x < 0.0f)
            {
                x += 360.0f;
            }

            return x;
        }

        /// <summary>
        ///  Normalize an angle to [0, 2*Pi) range.
        /// </summary>
        /// <param name="value">Angle in radians to normalize.</param>
        /// <returns>Normalized angle in radians.</returns>
        public static float NormalizeAngleTwoPi(float value)
        {
            // Ref: http://stackoverflow.com/a/11498248
            const float TwoPi = (float)(Math.PI * 2.0);
            var x = value % TwoPi;

            if (x < 0.0f)
            {
                x += TwoPi;
            }

            return x;
        }

        /// <summary>
        ///  Linearly interpolate an angle in radians.
        /// </summary>
        /// <remarks>
        ///  Implemented with help from: http://stackoverflow.com/q/2708476
        /// </remarks>
        /// <param name="start">Starting angle in radians.</param>
        /// <param name="end">Ending angle in radians.</param>
        /// <param name="t">Linear interpolation amount [0, 1).</param>
        /// <returns>Angle in radians.</returns>
        public static float LerpRadians(float start, float end, float t)
        {
            const float Pi = (float) Math.PI;
            const float TwoPi = 2.0f * Pi;

            var difference = Math.Abs(end - start);

            // If the angle difference is larger than half the unit circle (causing part of the answer to over/under
            // flow), adjust either the starting or ending angle by 2pi.
            if (difference > Pi)
            {
                if (end > start)
                {
                    start += TwoPi;
                }
                else
                {
                    end += TwoPi;
                }
            }

            // Calculate angle interpolation.
            var value = (start + ((end - start) * t));

            // Wrap value to unit circle and return.
            if (value >= 0 && value <= TwoPi)
            {
                return value;
            }
            else
            {
                return value % TwoPi;
            }
        }
    }
}

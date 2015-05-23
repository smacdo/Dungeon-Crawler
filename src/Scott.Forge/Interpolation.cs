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
    public static class Interpolation
    {
        /// <summary>
        ///  Linearly interpolate a value between [a,b] by an amount defined by t. T must vary between zero and one.
        /// </summary>
        /// <remarks>
        ///  This function will perform a smooth linear interpolation of the range defined by [a,b]. When t = 0, the
        ///  value returned will be a. When t = 0.5, the value returned will be halfway between a and b. And when
        ///  t = 1, the value returned will be b.
        /// </remarks>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation amount, clamped to [0,1].</param>
        /// <returns>The interpolated value.</returns>
        public static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        /// <summary>
        ///  Linearly interpolate a value between [a,b] by an amount defined by t. T must vary between zero and one.
        /// </summary>
        /// <remarks>
        ///  This function will perform a smooth linear interpolation of the range defined by [a,b]. When t = 0, the
        ///  value returned will be a. When t = 0.5, the value returned will be halfway between a and b. And when
        ///  t = 1, the value returned will be b.
        /// </remarks>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation amount, clamped to [0,1].</param>
        /// <returns>The interpolated value.</returns>
        public static double Lerp(double a, double b, double t)
        {
            return a + t * (b - a);
        }

        /// <summary>
        ///  Returns the interpolation factor used to interpolate between two values. This is the inverse of Lerp.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolate value from Lerp.</param>
        /// <returns>The interpolation amount, clamped to [0,1].</returns>
        public static float Unlerp(float a, float b, float t)
        {
            return (t - a) / (b - a);
        }

        /// <summary>
        ///  Returns the interpolation factor used to interpolate between two values. This is the inverse of Lerp.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolate value from Lerp.</param>
        /// <returns>The interpolation amount, clamped to [0,1].</returns>
        public static double Unlerp(double a, double b, double t)
        {
            return (t - a) / (b - a);
        }

        /// <summary>
        ///  Return the smooth step amount for a given factor between [0,1].
        /// </summary>
        /// <param name="t">Value ranging from 0 to 1.</param>
        /// <returns>Smooth step value.</returns>
        public static float SmoothStep(float t)
        {
            return (3.0f - 2.0f * t) * (t * t);
        }

        /// <summary>
        ///  Interpolate a value between [a,b] using smooth step interpolation by an amount defined by t.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation value, which be in the range [0, 1].</param>
        /// <returns>Interpolated value.</returns>
        public static float SmoothStep(float a, float b, float t)
        {
            return Lerp(a, b, SmoothStep(t));
        }


        /// <summary>
        ///  Return the smoother step amount for a given factor between [0,1].
        /// </summary>
        /// <param name="t">Value ranging from 0 to 1.</param>
        /// <returns>Smoother step value.</returns>
        public static float SmootherStep(float t)
        {
            return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
        }

        /// <summary>
        ///  Interpolate a value between [a,b] using smoother step interpolation by an amount defined by t.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation value, which be in the range [0, 1].</param>
        /// <returns>Interpolated value.</returns>
        public static float SmootherStep(float a, float b, float t)
        {
            return Lerp(a, b, SmootherStep(t));
        }
        
        /// <summary>
        ///  Return the Sinerp amount for a given factor between [0,1].
        /// </summary>
        /// <remarks>
        ///  This interpolation method will ease near the end of the range.
        /// </remarks>
        /// <param name="t">Value ranging from 0 to 1.</param>
        /// <returns>Sinerp value.</returns>
        public static float Sinerp(float t)
        {
            return (float) Math.Sin(t * Math.PI * 0.5f);
        }

        /// <summary>
        ///  Interpolate a value between [a,b] using sinusoidal interpolation by an amount defined by t.
        /// </summary>
        /// <remarks>
        ///  This interpolation method will ease near the end of the range.
        /// </remarks>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation value, which be in the range [0, 1].</param>
        /// <returns>Interpolated value.</returns>
        public static float Sinerp(float a, float b, float t)
        {
            return Lerp(a, b, Sinerp(t));
        }

        /// <summary>
        ///  Return the Coserp amount for a given factor between [0,1].
        /// </summary>
        /// <remarks>
        ///  This interpolation method will ease at the start of the range.
        /// </remarks>
        /// <param name="t">Value ranging from 0 to 1.</param>
        /// <returns>Coserp value.</returns>
        public static float Coserp(float t)
		{
            return 1.0f - (float) Math.Cos(t * Math.PI * 0.5f);
		}

        /// <summary>
        ///  Interpolate a value between [a,b] using cosinusoidal interpolation by an amount defined by t.
        /// </summary>
        /// <remarks>
        ///  This interpolation method will ease at the start of the range.
        /// </remarks>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation value, which be in the range [0, 1].</param>
        /// <returns>Interpolated value.</returns>
        public static float Coserp(float a, float b, float t)
        {
            return Lerp(a, b, Coserp(t));
        }

        /// <summary>
        ///  Return the Boing interpolation amount for a given factor between [0,1].
        /// </summary>
        /// <param name="t">Value ranging from 0 to 1.</param>
        /// <returns>Boing value.</returns>
        public static float Boing(float t)
        {
            t = MathHelper.Clamp(t, 0.0f, 1.0f);
            return (float)
                (Math.Sin(t * Math.PI * (0.2f + 2.5f * t * t * t)) *
                Math.Pow(1.0f - t, 2.2f) + t) *
                (1.0f + (1.2f * (1.0f - t)));
        }

        /// <summary>
        ///  Interpolate a value between [a,b] using boing interpolation by an amount defined by t.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Interpolation value, which be in the range [0, 1].</param>
        /// <returns>Interpolated value.</returns>
        public static float Boing( float a, float b, float t )
        {
            return Lerp(a, b, Boing(t));
        }

        /// <summary>
        ///  Return the bounce amount for a given factor between [0,1].
        /// </summary>
        /// <param name="t">Value ranging from 0 to 1.</param>
        /// <returns>Bounce value.</returns>
        public static float Bounce(float t)
        {
            return 
                (float) Math.Abs(Math.Sin(6.28f * (t + 1.0f) * (t + 1.0f)) * (1.0f - t));
        }

        public static float Bounce(float a, float b, float t)
        {
            return Lerp(a, b, Bounce(t));
        }
    }
}

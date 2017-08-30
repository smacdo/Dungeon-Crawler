/*
 * Copyright 2012-2017 Scott MacDonald
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

namespace Forge.Random
{
    /// <summary>
    ///  Provides random number generation similiar to System.Random, but with more features.
    /// </summary>
    [DataContract]
    public class ForgeRandom : System.Random
    {
        [DataMember(Name = "Generator", IsRequired = true)]
        private IRandomProvider mProvider;
        
        /// <summary>
        ///  Construct a new random number generator.
        /// </summary>
        public ForgeRandom()
            : this(new DefaultRandomProvider())
        {
        }

        /// <summary>
        ///  Construct a new random number generator.
        /// </summary>
        /// <param name="provider">A provider of random numbers.</param>
        public ForgeRandom(IRandomProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            mProvider = provider;
        }

        /// <summary>
        ///  Get the initial seed value for the random number generator.
        /// </summary>
        public int Seed
        {
            get { return mProvider.Seed; }
        }

        /// <summary>
        ///  Return a random number from [zero, maxInt).
        /// </summary>
        /// <returns>Random signed int.</returns>
        public override int Next()
        {
            return NextInt();
        }

        /// <summary>
        ///  Return a random number from [0, max).
        /// </summary>
        /// <returns>Random signed int.</returns>
        public int NextInt()
        {
            return mProvider.NextInt();
        }

        /// <summary>
        ///  Return a random number from [0, max).
        /// </summary>
        /// <remarks>
        ///  This method has a slight numerical bias towards smaller numbers.
        /// </remarks>
        /// <param name="max">The max value, exclsuive.</param>
        /// <returns>Random signed int from zero to max.</returns>
        public override int Next(int max)
        {
            return NextInt(0, max);
        }

        /// <summary>
        ///  Return a random number from [min, max).
        /// </summary>
        /// <remarks>
        ///  This method has a slight numerical bias towards smaller numbers.
        /// </remarks>
        /// <param name="min">Minimum value in the range.</param>
        /// <param name="max">Maximum value in the range.</param>
        /// <returns>Random signed int from min to max.</returns>
        public override int Next(int min, int max)
        {
            return NextInt(min, max);
        }

        /// <summary>
        ///  Return a random number from [min, max).
        /// </summary>
        /// <remarks>
        ///  This method has a slight numerical bias towards smaller numbers.
        ///  http://stackoverflow.com/a/5009006
        /// </remarks>
        /// <param name="min">Minimum value, inclusive.</param>
        /// <param name="max">Maximum value, exclusive..</param>
        /// <returns>Random signed int.</returns>
        public int NextInt(int min, int max)
        {
            var randomInt = mProvider.NextInt();
            return min + (randomInt % (max - min));
        }

        /// <summary>
        ///  Return a random floating point value from 0.0f to 1.0f exclusive.
        /// </summary>
        /// <returns>Random floating point value [0,1).</returns>
        public float NextFloat()
        {
            return mProvider.NextFloat();
        }

        /// <summary>
        ///  Return a random floating point value in the range [min, max).
        /// </summary>
        /// <param name="min">Minimum value, inclusive.</param>
        /// <param name="max">Maximum value, exclusive.</param>
        /// <returns>Random floating point value.</returns>
        public float NextFloat(float min, float max)
        {
            return (max - min) * NextFloat() + min;
        }

        /// <summary>
        ///  Return a random floating point value in the range [min, max).
        /// </summary>
        /// <param name="min">Minimum value, inclusive.</param>
        /// <param name="max">Maximum value, exclusive.</param>
        /// <returns>Random floatring point value.</returns>
        public double NextDouble(double min, double max)
        {
            return (max - min) * NextDouble() + min;
        }

        /// <summary>
        ///  Return a random double precision floating point value.
        /// </summary>
        /// <returns>
        ///  Random double precision floating point value.
        /// </returns>
        protected override double Sample()
        {
            return mProvider.NextDouble();
        }

        /// <summary>
        ///  Return a random boolean value.
        /// </summary>
        /// <returns>Random boolean value.</returns>
        public bool NextBool()
        {
            return NextBool(0.5f);
        }

        /// <summary>
        ///  Return a weighted random boolean value with a weight in the range [0, 1.0).
        /// </summary>
        /// <example>
        ///  NextBool(0.6) gives a 60% chance of returning true, and 40% of returning false.
        /// </example>
        /// <param name="trueWeight">Weight to give true.</param>
        /// <returns>Random boolean value.</returns>
        public bool NextBool(float trueWeight)
        {
            return NextFloat() < trueWeight;
        }

        /// <summary>
        ///  Fill the provided byte array with random byte values.
        /// </summary>
        /// <param name="bytes">Byte array to fill.</param>
        public override void NextBytes(byte[] bytes)
        {
            mProvider.NextBytes(bytes);
        }

        /// <summary>
        ///  Return a random 2d unit vector.
        /// </summary>
        /// <returns>A random unit vector.</returns>
        public Vector2 NextUnitVector2()
        {
            // Checked against answer: http://stackoverflow.com/a/25039730
            var phi = NextDouble(0, 2 * Math.PI);
            var x = Math.Cos(phi);
            var y = Math.Sin(phi);

            return new Vector2((float) x, (float) y);
        }

        /// <summary>
        ///  Return a random 3d unit vector.
        /// </summary>
        /// <returns></returns>
        public Vector3 NextUnitVector3()
        {
            // See: http://math.stackexchange.com/q/44689
            // See: http://stackoverflow.com/q/5408276
            var phi = NextDouble(0, 2 * Math.PI);
            var theta = Math.Acos(NextDouble(-1, 1));

            var x = Math.Sin(theta) * Math.Cos(phi);
            var y = Math.Sin(theta) * Math.Sin(phi);
            var z = Math.Cos(theta);

            return new Vector3((float) x, (float) y, (float) z);
        }
    }
}

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.Forge.Random
{
    /// <summary>
    ///  Provides System.Random for random number generation.
    /// </summary>
    public class DefaultRandomProvider : IRandomProvider
    {
        private System.Random mRandom;
        
        /// <summary>
        ///  Construct a new instsance of DefaultRandomProvider.
        /// </summary>
        public DefaultRandomProvider()
            : this(Environment.TickCount)
        {
        }

        /// <summary>
        ///  Construct a new instance of DefaultRandomProvider with the given seed.
        /// </summary>
        /// <param name="seed">Value to seed random number generator with.</param>
        public DefaultRandomProvider(int seed)
        {
            mRandom = new System.Random();
            Seed = seed;
        }

        /// <summary>
        ///  Get the value that was used to seed the random number generator.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        ///  Get next random bytes.
        /// </summary>
        /// <param name="bytes">Array of byte values to fill.</param>
        public void NextBytes(byte[] bytes)
        {
            mRandom.NextBytes(bytes);
        }

        /// <summary>
        ///  Get the next random double precision floating point value from [0, 1).
        /// </summary>
        /// <returns>Random double precision floating point value.</returns>
        public double NextDouble()
        {
            return mRandom.NextDouble();
        }

        /// <summary>
        ///  Get the next random single precision floating point value from [0, 1).
        /// </summary>
        /// <returns>Random single precision floating point value.</returns>
        public float NextFloat()
        {
            // See: http://stackoverflow.com/a/3365374
            // Perform arithmetic in double type to avoid overflowing
            var range = (double) float.MaxValue - (double) float.MinValue;
            var sample = mRandom.NextDouble();
            var scaled = (sample * range) + float.MinValue;
            return (float) scaled;
        }

        /// <summary>
        ///  Get the next random integer value from [0, intMax).
        /// </summary>
        /// <returns>Random integer value.</returns>
        public int NextInt()
        {
            return mRandom.Next();
        }
    }
}

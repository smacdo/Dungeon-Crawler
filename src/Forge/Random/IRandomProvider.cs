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

namespace Forge.Random
{
    /// <summary>
    ///  Random number generator.
    /// </summary>
    public interface IRandomProvider
    {
        /// <summary>
        ///  Get the value that was used to seed the random number generator.
        /// </summary>
        int Seed { get; }

        /// <summary>
        ///  Get the next random integer value from [0, intMax).
        /// </summary>
        /// <returns>Random integer value.</returns>
        int NextInt();

        /// <summary>
        ///  Get the next random single precision floating point value from [0, 1).
        /// </summary>
        /// <returns>Random single precision floating point value.</returns>
        float NextFloat();

        /// <summary>
        ///  Get the next random double precision floating point value from [0, 1).
        /// </summary>
        /// <returns>Random double precision floating point value.</returns>
        double NextDouble();

        /// <summary>
        ///  Get next random bytes.
        /// </summary>
        /// <param name="bytes">Array of byte values to fill.</param>
        void NextBytes(byte[] bytes);
    }
}

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
    ///  Wraps the System.Random class in a IRandomProvider interface for use in ForgeRandom.
    /// </summary>
    public class SystemRandomProvider : IRandomProvider
    {
        private System.Random mRandom;

        /// <summary>
        ///  Construct a new instance of the SystemRandomProvider.
        /// </summary>
        public SystemRandomProvider()
            : this(new System.Random())
        {
        }

        /// <summary>
        ///  Construct a new instance of the SystemRandomProvider.
        /// </summary>
        public SystemRandomProvider(System.Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            mRandom = random;
        }

        public int Seed => throw new NotImplementedException();

        public void NextBytes(byte[] bytes)
        {
            mRandom.NextBytes(bytes);
        }

        public double NextDouble()
        {
            return mRandom.NextDouble();
        }

        public float NextFloat()
        {
            throw new NotImplementedException();
        }

        public int NextInt()
        {
            return mRandom.Next();
        }

        public float NextUIntAsFloat()
        {
            throw new NotImplementedException();
        }
    }
}

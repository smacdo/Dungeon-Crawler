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

//
//  C++ implementation of the MT19937 random number generator. The original implementation was
//  coded by Takuji Nishmura and Makoto Matsumoto. Modified and ported to C++ by Scott MacDonald
//  on 2013/03/25. The generators returning floating point values are based on code written by
//  Isaku Wada, 2002/01/09. The original license is reproduced below:
//
// A C-program for MT19937, with initialization improved 2002/1/26.
// Coded by Takuji Nishimura and Makoto Matsumoto.
//
// Before using, initialize the state by using init_genrand(seed)
// or init_by_array(init_key, key_length).
//
// Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions
// are met:
//
//   1. Redistributions of source code must retain the above copyright
//      notice, this list of conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in the
//      documentation and/or other materials provided with the distribution.
//
//   3. The names of its contributors may not be used to endorse or promote
//      products derived from this software without specific prior written
//      permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// Any feedback is very welcome.
// http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
// email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
//
// ---
//
// Feedback about this C++ port should be sent to Scott MacDonald. See
// http://whitespaceconsideredharmful.com/ for contact information.
using System;
using System.Runtime.Serialization;
using Scott.Forge;

namespace Scott.Forge
{
    /// <summary>
    ///  Random number generator that uses Mersenne Twister.
    /// </summary>
    [DataContract]
    public class MersenneTwister : IRandom
    {
        private const int N = 624;
        private const int M = 397;
        private const ulong MATRIX_A = 0x9908B0DFUL;
        private const ulong UPPER_MASK = 0x80000000UL;
        private const ulong LOWER_MASK = 0x7fffffffUL;
        private const ulong INITIAL_SEED = 5489UL;
        private static ulong[] MAG01 = new ulong[] { 0x0, MATRIX_A };

        // Current state of the mersenne twister.
        [DataMember]
        private ulong[] mVals;

        [DataMember]
        private ulong mSeed;

        [DataMember]
        private int mIndex;

        // True if there is a cached Gaussian value to give out.
        [DataMember]
        private bool mHasNextGaussian = false;

        // A cached Gaussian value to give to a caller.
        [DataMember]
        private float mNextGaussian = 0.0f;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public MersenneTwister()
            : this( GetRandomSeed() )
        {
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="seed">Random seed to use when initializing.</param>
        public MersenneTwister( ulong seed )
        {
            Init( seed );
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="seed">Random seeds to use when initializing.</param>
        public MersenneTwister( ulong[] seed )
        {
            InitByArray( seed );
        }

        /// <summary>
        ///  Initialize a MersenneTwister to a starting state, given a seed value.
        /// </summary>
        /// <param name="seed">The seed value to use when initializing.</param>
        /// <returns>Starting state.</returns>
        private void Init( ulong seed )
        {
            ulong[] vals = new ulong[N];

            // Initialize the first element in the state array with the seed value.
            vals[0] = seed & 0xffffffff;

            // Initialize the rest of the state array.
            for ( int index = 1; index < N; ++index )
            {
                vals[index] = ( 1812433253 * ( vals[index-1] ^ ( vals[index-1] >> 30 ) ) + (uint)index );

                // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
                // In the previous versions, MSBs of the seed affect
                // only MSBs of the array mt[].
                // 2002/01/09 modified by Makoto Matsumoto
                vals[index] &= 0xffffffff; // for 32 bit machines
            }

            // Initialize and return the new mersenne twister state.
            mVals = vals;
            mSeed = seed;
            mIndex = 0;
        }

        /// <summary>
        ///  Initialize a MersenneTwister to a starting state, given a seed value.
        /// </summary>
        /// <param name="key">The seed values to use when initializing.</param>
        /// <returns>Starting state.</returns>
        private void InitByArray( ulong[] key )
        {
            if ( key == null )
            {
                throw new ArgumentNullException( "key" );
            }

            // Initial values.
            int i = 1, j = 0;
            int k = ( N > key.Length ? N : key.Length );

            // Initialize the mersenne twister state once with an initial seed value.
            Init( 19650218 ); // what's this magic number represent?
            ulong[] vals = mVals;

            // Now initialize the mersenne twister state again with the provided state array.
            for ( ; k > 0; --k )
            {
                vals[i] = ( vals[i] ^ ( ( vals[i - 1] ^ ( vals[i - 1] >> 30 ) ) * 1664525 ) )
                    + vals[j] + (ulong) j;   // non-linear... ?
                vals[i] &= 0xffffffff; // for WORDSIZE > 32 machines

                i++;
                j++;

                if ( i >= N )
                {
                    vals[0] = vals[N - 1];
                    i = 1;
                }

                if ( j >= key.Length )
                {
                    j = 0;
                }
            }

            for ( k = N - 1; k > 0; k-- )
            {
                vals[i] = ( vals[i] ^ ( ( vals[i - 1] ^ ( vals[i - 1] >> 30 ) ) ^ 1566083941 ) )
                    - (ulong) i; // non linear. again - ?
                vals[i] &= 0xffffffff; // for WORDSIZE > 32 machines

                i++;

                if ( i >= N )
                {
                    vals[0] = vals[N - 1];
                    i = 1;
                }
            }

            vals[0] = 0x80000000; // MSB is 1; assuring non-zero initial array
        }

        /// <summary>
        ///  Generate a random unsigned int and return it.
        /// </summary>
        /// <returns>Random unsigned int.</returns>
        public ulong NextULong()
        {
            ulong y;

            // If we are out of state, then regenerate the random state array.
            if ( mIndex >= N )          // Generate N words at a time
            {
                uint kk;

                for ( kk = 0; kk < N - M; ++kk )
                {
                    y = ( mVals[kk] & UPPER_MASK ) | ( mVals[kk + 1] & LOWER_MASK );
                    mVals[kk] = mVals[kk + M] ^ ( y >> 1 ) ^ MAG01[y & 0x1UL];
                }

                for ( ; kk < N - 1; ++kk )
                {
                    y = ( mVals[kk] & UPPER_MASK ) | ( mVals[kk + 1] & LOWER_MASK );
                    mVals[kk] = mVals[kk + ( M - N )] ^ ( y >> 1 ) ^ MAG01[y & 0x1UL];
                }

                y = ( mVals[N - 1] & UPPER_MASK ) | ( mVals[0] & LOWER_MASK );
                mVals[N - 1] = mVals[M - 1] ^ ( y >> 1 ) ^ MAG01[y & 0x1UL];

                mIndex = 0;
            }

            // Get next random value in sequence
            y = mVals[mIndex++];

            // Tempering
            y ^= ( y >> 11 );
            y ^= ( y << 7 )  & 0x9d2c5680U;
            y ^= ( y << 15 ) & 0xefc60000U;
            y ^= ( y >> 18 );

            return y;
        }

        /// <summary>
        ///  Return a random number from zero to the maximum integer value.
        /// </summary>
        /// <returns>Random signed int.</returns>
        public int NextInt()
        {
            return (int)( NextULong() >> 1 );
        }

        /// <summary>
        ///  Return a random number from min to max (inclusive).
        /// </summary>
        /// <remarks>
        ///  This has a numerical bias, we should correct that.
        /// </remarks>
        /// <param name="min">Minimum value in the range.</param>
        /// <param name="max">Maximum value in the range.</param>
        /// <returns>Random signed int from min to max.</returns>
        public int NextInt( int min, int max )
        {
            return min + NextInt() % ( max - min + 1 );
        }

        /// <summary>
        ///  Returns the next unsigned int as a floating point value.
        /// </summary>
        /// <returns></returns>
        public float NextUIntAsFloat()
        {
            return (float) NextULong();
        }

        /// <summary>
        ///  Return a random floating point value from 0.0f to 1.0f inclusive.
        /// </summary>
        /// <returns>Random floating point value [0,1].</returns>
        public float NextFloat()
        {
            float value = (float) NextULong();
            return value * ( 1.0f / 4294967295.0f );    // divide by 2^32-1
        }

        /// <summary>
        ///  Return a random floating point value from 0.0f to 1.0f exclusive.
        /// </summary>
        /// <returns>Random floating point value from [0,1).</returns>
        public float NextFloatExclusive()
        {
            float value = (float) NextULong();
            return value * ( 1.0f / 4294967296.0f );    // divide by 2^32
        }

        /// <summary>
        ///  Return a random floating point value larger than zero and smaller than one.
        /// </summary>
        /// <returns>Random floating point value from (0,1).</returns>
        public float NextFloatNonZeroExclusive()
        {
            float value = (float) NextULong();
            return ( value + 0.5f ) * ( 1.0f / 4294967296.0f );
        }

        /// <summary>
        ///  Return a random double precision floating point value with 53 bits of precision.
        /// </summary>
        /// <remarks>
        ///  I'm not sure of the range on this method... I should investigate!
        /// </remarks>
        /// <returns>
        ///  Random double precision floating point value with 53 bits of precision.
        /// </returns>
        public double NextDouble()
        {
            float a = (float)( NextULong() >> 5 );
            float b = (float)( NextULong() >> 6 );

            return ( a * 67108864.0 + b ) * ( 1.0 / 9007199254740992.0 );
        }

        /// <summary>
        ///  Return a random boolean value.
        /// </summary>
        /// <returns>Random boolean value.</returns>
        public bool NextBool()
        {
            return 1 == ( NextULong() % 2 );
        }

        /// <summary>
        ///  Fill the provided byte array with random byte values.
        /// </summary>
        /// <param name="bytes">Byte array to fill.</param>
        public void NextBytes( byte[] bytes )
        {
            int i = 0;

            if ( bytes == null )
            {
                throw new ArgumentException( "bytes" );
            }    

            // Fill the provided byte array with random values, filling it in increments of four.
            for ( i = 0; i < bytes.Length; i += 4 )
            {
                ulong v = NextULong();

                bytes[i + 0] = (byte)( v & 0xFF );
                bytes[i + 1] = (byte)( ( v >> 8  ) & 0xFF );
                bytes[i + 2] = (byte)( ( v >> 16 ) & 0xFF );
                bytes[i + 3] = (byte)( ( v >> 24 ) & 0xFF );
            }

            // Fill the remaining buckets before finishing.
            ulong last = NextULong();

            switch ( bytes.Length - i )
            {
                case 3:
                    bytes[i + 0] = (byte) ( last & 0xFF );
                    bytes[i + 1] = (byte) ( ( last >> 8 ) & 0xFF );
                    bytes[i + 2] = (byte) ( ( last >> 16 ) & 0xFF );
                    break;

                case 2:
                    bytes[i + 0] = (byte) ( last & 0xFF );
                    bytes[i + 1] = (byte) ( ( last >> 8 ) & 0xFF );
                    break;

                case 1:
                    bytes[i + 0] = (byte) ( last & 0xFF );
                    break;
            }
        }

        /// <summary>
        ///  Return a randomly generated seed value. It's not particularly secure or
        ///  non-reproducible.
        /// </summary>
        /// <returns>Random seed value.</returns>
        private static ulong GetRandomSeed()
        {
            System.Random r =new System.Random( new System.DateTime().Millisecond );
            return (ulong) r.Next();
        }

        /// <summary>
        ///  Return a floating point value generated with a Gaussian selection formula.
        /// </summary>
        /// <returns>Random gaussian value.</returns>
        public float NextGaussian()
        {
            float v1 = 0.0f, v2 = 0.0f, s = 0.0f;

            if ( mHasNextGaussian )
            {
                mHasNextGaussian = false;
                return mNextGaussian;
            }

            do
            {
                v1 = 2.0f * NextFloat() - 1.0f;
                v2 = 2.0f * NextFloat() - 1.0f;
                s = v1 * v1 + v2 * v2;
            }
            while ( s == 0.0f || s >= 1.0f );

            float multiplier = (float) Math.Sqrt( -2.0 * (Math.Log( s ) / s ) );

            mNextGaussian    = v2 * multiplier;
            mHasNextGaussian = true;

            return v1 * multiplier;
        }

        /// <summary>
        ///  Return a floating point value generated with a Gaussian selection formula.
        /// </summary>
        /// <param name="standardDeviation">Standard deviation for the value.</param>
        /// <param name="mean">Mean for the value.</param>
        /// <returns>Random Gaussian value.</returns>
        public float NextGaussian( float standardDeviation, float mean )
        {
            return NextGaussian() * standardDeviation + mean;
        }

        /// <summary>
        ///  Return a floating point value generated with a Gaussian selection formula.
        /// </summary>
        /// <param name="standardDeviation">Standard deviation for the value.</param>
        /// <param name="mean">Mean for the value.</param>
        /// <param name="min">Minimum acceptable value.</param>
        /// <param name="max">Maximum acceptable value.</param>
        /// <returns>Random Gaussian value.</returns>
        public float NextGaussian( float standardDeviation, float mean, float min, float max )
        {
            float v = NextGaussian() * standardDeviation + mean;
            return MathHelper.Clamp( v, min, max );
        }

    }
}

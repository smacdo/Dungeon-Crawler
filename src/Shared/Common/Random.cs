using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common
{
    /// <summary>
    ///  Type of random number generator to use.
    /// </summary>
    public enum RandomGeneratorType
    {
        /// <summary>
        ///  Mersenne Twister
        /// </summary>
        MersenneTwister
    }

    /// <summary>
    ///  Random number generator.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        ///  Return a random number from zero to the maximum integer value.
        /// </summary>
        int NextInt();

        /// <summary>
        ///  Return a random number from min to max (inclusive).
        /// </summary>
        int NextInt( int min, int max );

        /// <summary>
        ///  Return a random floating point value from 0.0f to 1.0f inclusive.
        /// </summary>
        float NextFloat();

        /// <summary>
        ///  Return a random double precision floating point value.
        /// </summary>
        double NextDouble();

        /// <summary>
        ///  Returns the next unsigned int as a floating point value.
        /// </summary>
        float NextUIntAsFloat();

        /// <summary>
        ///  Fill the provided byte array with random byte values.
        /// </summary>
        void NextBytes( byte[] bytes );
    }

    /// <summary>
    ///  A better implementation of System.Random that has serialization support and pluggable
    ///  random generators.
    /// </summary>
    [Serializable]
    public class Random : System.Random
    {
        IRandom mRandom;
        RandomGeneratorType mType;

        /// <summary>
        ///  Create a new random number generator of the specified type.
        /// </summary>
        /// <param name="type">Type of random number generator to create.</param>
        public Random( RandomGeneratorType type )
        {
            mType = type;
            mRandom = new MersenneTwister();
        }

        /// <summary>
        ///  Create a new random number generator of the specified type.
        /// </summary>
        /// <param name="type">Type of random number generator to create.</param>
        /// <param name="seed">Random number generator seed.</param>
        public Random( RandomGeneratorType type, int seed )
        {
            mType = type;
            mRandom = new MersenneTwister( (uint) seed );
        }

        /// <summary>
        ///  Return a random number from zero to the maximum integer value.
        /// </summary>
        /// <returns>Random signed int.</returns>
        public override int Next()
        {
            return mRandom.NextInt();
        }

        /// <summary>
        ///  Return a random number from zero to max (inclusive).
        /// </summary>
        /// <param name="max">The maximum value on the random range.</param>
        /// <returns>Random signed int from zero to max.</returns>
        public override int Next( int max )
        {
            return Next( 0, max );
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
        public override int Next( int min, int max )
        {
            return mRandom.NextInt( min, max );
        }

        /// <summary>
        ///  Return a random floating point value from 0.0f to 1.0f inclusive.
        /// </summary>
        /// <returns>Random floating point value [0,1].</returns>
        public float NextFloat()
        {
            return mRandom.NextFloat();
        }

        /// <summary>
        ///  Return a random floating point value from 0.0f to 1.0f exclusive.
        /// </summary>
        /// <returns>Random floating point value from [0,1).</returns>
        public float NextFloatExclusive()
        {
            float value = mRandom.NextUIntAsFloat();
            return value * ( 1.0f / 4294967296.0f );    // divide by 2^32
        }

        /// <summary>
        ///  Return a random floating point value larger than zero and smaller than one.
        /// </summary>
        /// <returns>Random floating point value from (0,1).</returns>
        public float NextFloatNonZeroExclusive()
        {
            float value = mRandom.NextUIntAsFloat();
            return ( value + 0.5f ) * ( 1.0f / 4294967296.0f );
        }

        /// <summary>
        ///  Return a random double precision floating point value.
        /// </summary>
        /// <returns>
        ///  Random double precision floating point value.
        /// </returns>
        protected override double Sample()
        {
            return mRandom.NextDouble();
        }

        /// <summary>
        ///  Return a random boolean value.
        /// </summary>
        /// <returns>Random boolean value.</returns>
        public bool NextBool()
        {
            return 1 == ( mRandom.NextInt() % 2 );
        }

        /// <summary>
        ///  Fill the provided byte array with random byte values.
        /// </summary>
        /// <param name="bytes">Byte array to fill.</param>
        public override void NextBytes( byte[] bytes )
        {
            mRandom.NextBytes( bytes );
        }
    }
}

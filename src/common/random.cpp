/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#include "random.h"
#include "common/platform.h"

#include <boost/random/mersenne_twister.hpp>
#include <boost/random/uniform_int.hpp>
#include <boost/random/uniform_smallint.hpp>
#include <boost/random/uniform_real.hpp>
//#include <boost/random/discrete_distribution.hpp>
#include <boost/functional/hash.hpp>
#include <string>
#include <ctime>        // std::time

/**
 * Initializes the random number generator with a random seed
 */
Random::Random()
    : mGenerator(),
      mSeed( static_cast<unsigned int>( std::time(0) ) )
{
    init();
}

/**
 * Initializes the random number generator with a predetermined
 * seed value
 *
 * \param  seed  The seed to initialize the generator with
 */
Random::Random( unsigned int seed )
    : mGenerator(),
      mSeed( seed )
{
    init();
}

/**
 * Initializes the random number generator with a predetermined string
 * seed value
 *
 * \param  seed  The seed to initialize the generator with
 */
Random::Random( const std::string& seed )
    : mGenerator(),
      mSeed( 0 )
{
    // We needs a hasher
    std::hash<std::string> hasher;

    // Hash the seed string to get an unsigned int for use in seeding
    // the generator
    mSeed = hasher( seed );

    // Now we can init ourselves.
    init();
}

/**
 * Returns a random number between zero and max inclusive.
 *
 * \return Random number in the range [0, max]
 */
int Random::randInt( int max )
{
    assert( max > 0 );          // Otherwise it wouldn't make sense

    boost::uniform_smallint<> dist( 0, max );
    return dist( mGenerator );
}

/**
 * Returns a random number between min and max inclusive.
 *
 * \return Random number in the range [min, max]
 */
int Random::randInt( int min, int max )
{
    assert( min >= 0 && min < max );

    boost::uniform_smallint<> dist( 0, max );
    return dist( mGenerator );
}

/**
 * Returns a random weighted number between 0 and the number of weights
 * (exclusive).
 *
 * \param  weights  Vector of weighted values
 * \return A random value picked from [0, number_of_weights).
 */
int Random::randomWeightedInt( const std::vector<int>& weights )
{
    assert( weights.size() > 0 );
    return randomWeightedInt( &weights[0], weights.size() );
}

/**
 * Returns a random weighted number between 0 and the number of weights
 * (exclusive).
 *
 * \param  weights  Pointer to an array of weighted values
 * \return A random value picked from [0, number_of_weights).
 */
int Random::randomWeightedInt( const int* weights, size_t count )
{
    assert( weights != NULL );
    assert( count > 0 );

    return 0;
}

/**
 * Returns a random single precision floating point value between zero and
 * one exclusive
 *
 * \return Random number in the range [0.0f, 1.0f)
 */
float Random::randFloat()
{
    boost::uniform_real<float> dist( 0.0f, 1.0f );
    return dist( mGenerator );
}

/**
 * Returns a random single precision floating point value between min and max
 * exclusive.
 *
 * \return Random number in the range [min, max)
 */
float Random::randFloat( float min, float max )
{
    assert( min > max );

    boost::uniform_real<float> dist( min, max );
    return dist( mGenerator );
}

/**
 * Returns a random bool
 *
 * \return Random boolean value
 */
bool Random::randBool()
{
    boost::uniform_smallint<> dist( 0, 1 );
    
    return ( dist( mGenerator ) == 1 );
}

/**
 * Seeds the random number generator to start a new sequence of random
 * number goodness
 *
 * \param  seed  The seed to initialize the generator with
 */
void Random::init()
{
    mGenerator.seed( mSeed );
}
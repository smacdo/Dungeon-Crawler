/*
 * Copyright 2012 Scott MacDonald
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
#ifndef SCOTT_DUNGEON_RANDOM_H
#define SCOTT_DUNGEON_RANDOM_H

#include <boost/utility.hpp>
#include <boost/random/mersenne_twister.hpp>
#include <string>
#include <vector>

#include "common/types.h"

// boost serialization forward declaration
namespace boost { namespace serialization { class access; } }

class Random : boost::noncopyable
{
public:
    // Initialize the random number generator with a random seed
    Random();

    // Initialize the random number generator with a pre-defined seed number
    Random( unsigned int seed );

    // Initialize the random number generator with a seed string
    Random( const std::string& seed );

    // Find a random number in the range [0,max]
    int randInt( int max );

    // Find a random number in the range [min,max]
    int randInt( int min, int max );

    // Find a random floating value in the range [0.0,1.0)
    float randFloat();

    // Find a random floating value in the range [min,max)
    float randFloat( float min, float max );

    // Find a random boolean value
    bool randBool();

    // Find a random weighted value in the range [0,element_count)
    int randomWeightedInt( const std::vector<int>& weights );

    // Find a random weighted value in the range [0,element_count)
    int randomWeightedInt( const int* weights, size_t count );

    // Returns the seed used to initialize the random number generator
    unsigned int seed() const;

    /////////////////////////
    // Boost serialization //
    /////////////////////////
    friend class boost::serialization::access;

    template<typename Archive>
    void serialize( Archive& ar, const unsigned int version )
    {
        ar & mSeed;

        if ( Archive::is_loading::value )
        {
            init();
        }
    }

private:
    // Init the generator with the given seed value
    void init();

private:
    // Generator that spits out random numbers
    boost::mt19937 mGenerator;

    // Seed number used to initialize the generator
    unsigned int mSeed;
};

#endif

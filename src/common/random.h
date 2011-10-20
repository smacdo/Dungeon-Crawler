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
#ifndef SCOTT_DUNGEON_RANDOM_H
#define SCOTT_DUNGEON_RANDOM_H

#include <boost/utility.hpp>
#include <boost/random/mersenne_twister.hpp>

#include <string>
#include <vector>

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

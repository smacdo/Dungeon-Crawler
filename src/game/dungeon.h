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
#ifndef SCOTT_DUNGEON_DUNGEON_H
#define SCOTT_DUNGEON_DUNGEON_H

#include <vector>
#include <string>
#include <memory>
#include <boost/utility.hpp>

class Level;

/**
 * Holds all relevant information about a dungeon, including all of the
 * levels in it and all of the actors and items currently inside.
 */
class Dungeon : public boost::noncopyable
{
public:
    // Create a new dungeon object
    Dungeon( const std::string& name,
             size_t maxWidth,
             size_t maxHeight,
             const std::vector< std::shared_ptr<Level> >& levels );

    // Dungeon destructor
    ~Dungeon();
 
    // Return the number of levels contained in this dungeon
    size_t levelCount() const;

    // Return the name of this dungeon
    std::string name() const;

    // Return the width of the game
    size_t maxWidth() const;

    // Return the height of the game
    size_t maxHeight() const;

    // Get a level
    std::shared_ptr<Level> getLevel( size_t index );

private:
    std::string mName;
    size_t mMaxWidth;
    size_t mMaxHeight;
    std::vector< std::shared_ptr<Level> > mLevels;
};

#endif

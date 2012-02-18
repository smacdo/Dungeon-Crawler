/*
 * Copyright (C) 2012 Scott MacDonald. All rights reserved.
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

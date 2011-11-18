#ifndef SCOTT_DUNGEON_LEVEL_H
#define SCOTT_DUNGEON_LEVEL_H

#include "tilegrid.h"

#include <string>
#include <boost/utility.hpp>

class Tile;

/**
 * This class represents a playable game level in dungeon crawler. It
 * contains the level's terrain tiles, actors, items and anything else
 * relating to its current state
 */
class Level : boost::noncopyable
{
public:
    // Level constructor
    Level( const std::string& levelName,
           const TileGrid& tileGrid );

    // Level destructor
    ~Level();

    // Return a reference to a requested tile
    Tile& tileAt( const Point& p );

    // Return a const reference to a requested tile
    const Tile& tileAt( const Point& p ) const;

    // Dump information about this level
    std::string dump() const;

    // Return the name of this level
    std::string name() const;

    // Return the width of this level
    size_t width() const;

    // Return the height of this level
    size_t height() const;

private:
    std::string mName;
    TileGrid mTileGrid;
};

#endif

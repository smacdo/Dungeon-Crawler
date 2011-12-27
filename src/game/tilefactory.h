#ifndef DUNGEON_GAME_TILEFACTORY_H
#define DUNGEON_GAME_TILEFACTORY_H

#include <vector>
#include <boost/utility.hpp>

#include "game/tile.h"

struct TileData;

/**
 * Tile factory is responsible for creating tiles
 */
class TileFactory : boost::noncopyable
{
public:
    TileFactory();
    ~TileFactory();

    Tile createGranite() const;
    Tile createVoid() const;
    Tile createWall() const;
    Tile createFloor() const;

private:
    std::vector<TileData*> mBlueprints;
};

#endif

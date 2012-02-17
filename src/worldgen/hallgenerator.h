#ifndef SCOTT_DUNGEON_HALLGENERATOR_H
#define SCOTT_DUNGEON_HALLGENERATOR_H

#include <boost/noncopyable.hpp>
#include "game/pathfinder.h"

struct RoomData;
class TileFactory;
class Random;
class TileGrid;

/**
 * Responsible for carving interesting hallways between rooms
 */
class HallGenerator : boost::noncopyable
{
public:
    HallGenerator( Random& random,
                   const TileFactory& factory,
                   TileGrid& tilegrid );
    ~HallGenerator();

    void connect( RoomData *pStartRoom, RoomData *pEndRoom );

private:
    void reset( RoomData *pStartRoom, RoomData *pEndRoom );
    int findMovementCost( const Point& current, const Point& prev );

private:
    Random& mRandom;
    const TileFactory& mTileFactory;
    TileGrid& mTileGrid;
    PathFinder mPathFinder;

    RoomData *mpStartRoom;
    RoomData *mpDestRoom;
};

#endif

#ifndef SCOTT_DUNGEON_ROOMDATA_H
#define SCOTT_DUNGEON_ROOMDATA_H

#include "common/point.h"
#include "common/rect.h"
#include "game/tilegrid.h"

#include <vector>

/**
 * Contains information on a generated room, including the TileGrid used and
 * useful coordinates.
 *
 * We should make it so that a RoomData class can be converted to a room class
 * and handed to the base game. (The base game doesn't need to worry about
 * things like the room's TileGrid)
 */
struct RoomData
{
    /**
     * Create a new room data structure. The provided rectangle contains
     * both the upper left most floor tile and the bottom most right floor
     * tile. 
     *
     * The rectangle does not need to be entirely floor tiles, so long as the
     * top left, bottom left and center are floor tiles.
     */
    RoomData( const Rect& floorRect, TileGrid& tileGrid )
        : floorArea( floorRect ),
          totalArea( floorRect.x() - 1,
                     floorRect.y() - 1,
                     floorRect.width() + 2,
                     floorRect.height() + 2 ),
          floorTopLeft( floorRect.topLeft() ),
          floorBottomRight( floorRect.bottomRight() ),
          floorCenter( floorRect.approximateCenter() ),
          tiles( tileGrid )
    {
    }

    /// A rectangle that tightly bounds the floor area of the room.
    Rect floorArea;

    /// Rectangle that fully bounds the room, including the wall
    ///  (should just be +1)
    Rect totalArea;

    /// The top left most floor tile in the room
    Point floorTopLeft;

    /// Bottom right most floor tile in the room
    Point floorBottomRight;

    /// The tile in the center of the room (must be a floor tile)
    Point floorCenter;

    /// Tile grid that describes the room
    TileGrid tiles;

    /// List of rooms that this room is connected to
    std::vector<RoomData*> connectedRooms;
};

#endif

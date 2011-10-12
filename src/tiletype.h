#ifndef SCOTT_DUNGEON_TILETYPE_H
#define SCOTT_DUNGEON_TILETYPE_H

enum ETileType { TILE_IMPASSABLE,    // nothing can be placed here, nor can it ever be used
                 TILE_UNALLOCATED,  // this tile has not been allocated to anyone
                 TILE_VOID,         // there is literally nothing here
                 TILE_WALL,         // its a wall
                 TILE_FLOOR,        // floor
                 TILE_DOOR,         // door
                 ETileType_Count
};

#endif


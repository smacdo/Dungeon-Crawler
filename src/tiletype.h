#ifndef SCOTT_DUNGEON_TILETYPE_H
#define SCOTT_DUNGEON_TILETYPE_H

enum ETileType { TILE_IMPASSABLE,  // nothing can be placed here, nor can it ever be used
                 TILE_EMPTY,       // nothing to see here, move alongs
                 TILE_WALL,        // its a wall
                 TILE_FLOOR,       // floor
                 ETileType_Count
};

#endif


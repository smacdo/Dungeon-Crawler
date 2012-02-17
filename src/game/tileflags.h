#ifndef DUNGEON_ENGINE_TILE_FLAGS
#define DUNGEON_ENGINE_TILE_FLAGS

enum ETileFlags
{
    ETILE_GRANITE,      // set if this tile is granite (unmodifable)
    ETILE_PLACED,       // set if the dungeon generator has modified this tile
    ETILE_IMPASSABLE,   // nothing can enter or be placed on this tile
    ETILE_WALK,         // tile can be walked on
    ETILE_FLY,          // tile can be flown across
    ETILE_SWIM,         // tile can be swum across
    ETILE_TUNNEL,       // tile can be tunneled through
    ETILE_WALL,         // considered wall tile for dungeon generation
    ETILE_FLOOR,        // considered floor tile for dungeon generation
    ETILE_DOORWAY,      // considered doorway for dungeon generation
    ETILE_IS_ROOM,      // tile belongs to a "room"
    ETILE_IS_HALL,      // tile belongs to a "hall"
    ETILE_BLOCK_LOS,    // blocks line of sight for an actor
    ETILE_FLAGS_COUNT
};

#endif

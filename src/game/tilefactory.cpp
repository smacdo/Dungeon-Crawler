#include <vector>
#include <string>

#include "game/tilefactory.h"
#include "game/tiledata.h"
#include "game/tileflags.h"
#include "game/tile.h"
#include "game/tiletypes.h"
#include "common/utils.h"

/**
 * The tile factory constructor
 */
TileFactory::TileFactory()
    : mBlueprints( ETILETYPE_COUNT, 0 )
{
    // Construct void tile
    TileData *pVoidData = new TileData;
    pVoidData->id    = ETILETYPE_VOID;
    pVoidData->name  = "void";
    pVoidData->title = "Void Tile";
    pVoidData->flags.set( ETILE_IMPASSABLE );

    mBlueprints[ pVoidData->id ] = pVoidData;

    // Construct granite tile
    TileData *pGraniteData = new TileData;
    pGraniteData->id       = ETILETYPE_GRANITE;
    pGraniteData->name     = "granite";
    pGraniteData->title    = "Granite Wall";

    pGraniteData->flags.set( ETILE_GRANITE );
    pGraniteData->flags.set( ETILE_IMPASSABLE );
    pGraniteData->flags.set( ETILE_WALL );
    pGraniteData->flags.set( ETILE_BLOCK_LOS );

    mBlueprints[ pGraniteData->id ] = pGraniteData;

    // Construct stone wall tile
    TileData *pStoneWallData = new TileData;
    pStoneWallData->id       = ETILETYPE_DUNGEON_WALL;
    pStoneWallData->name     = "stone_wall";
    pStoneWallData->title    = "Stone Wall";

    pStoneWallData->flags.set( ETILE_PLACED );
    pStoneWallData->flags.set( ETILE_IMPASSABLE );
    pStoneWallData->flags.set( ETILE_WALL );
    pStoneWallData->flags.set( ETILE_BLOCK_LOS );

    mBlueprints[ pStoneWallData->id ] = pStoneWallData;

    // Construct stone floor tile
    TileData *pStoneFloorData = new TileData;
    pStoneFloorData->id       = ETILETYPE_DUNGEON_FLOOR;
    pStoneFloorData->name     = "stone_floor";
    pStoneFloorData->title    = "Stone Floor";

    pStoneFloorData->flags.set( ETILE_PLACED );
    pStoneFloorData->flags.set( ETILE_WALK );
    pStoneFloorData->flags.set( ETILE_FLOOR );

    mBlueprints[ pStoneFloorData->id ] = pStoneFloorData;

    // Construct stone tile filler tile
    TileData *pStoneFillerData = new TileData;
    pStoneFillerData->id        = ETILETYPE_FILLER_STONE;
    pStoneFillerData->name      = "filler_stone";
    pStoneFillerData->title     = "Stone Rock";

    pStoneFillerData->flags.set( ETILE_IMPASSABLE );
    pStoneFillerData->flags.set( ETILE_WALL );
    pStoneFillerData->flags.set( ETILE_BLOCK_LOS );

    mBlueprints[ pStoneFillerData->id ] = pStoneFillerData;
}

/**
 * Tile factory destructor
 */
TileFactory::~TileFactory()
{
    DeleteVectorPointers( mBlueprints );
}

/**
 * Create and return a void tile
 */
Tile TileFactory::createVoid() const
{
    TileData *pTileData = mBlueprints[ ETILETYPE_VOID ];
    assert( pTileData != NULL );

    return Tile( pTileData );
}

/**
 * Create and return a granite tile
 */
Tile TileFactory::createGranite() const
{
    TileData *pTileData = mBlueprints[ ETILETYPE_GRANITE ];
    assert( pTileData != NULL );

    return Tile( pTileData );
}

/**
 * Create and return generic "filler" tiles (stuff that exists in the empty area
 * inbetween dungeon rooms/halls
 */
Tile TileFactory::createFiller() const
{
    TileData *pTileData = mBlueprints[ ETILETYPE_FILLER_STONE ];
    assert( pTileData != NULL );

    return Tile( pTileData );
}

/**
 * Create and return a wall
 */
Tile TileFactory::createWall() const
{
    TileData *pTileData = mBlueprints[ ETILETYPE_DUNGEON_WALL ];
    assert( pTileData != NULL );
    
    return Tile( pTileData );
}

/**
 * Create and return a floor
 */
Tile TileFactory::createFloor() const
{
    TileData *pTileData = mBlueprints[ ETILETYPE_DUNGEON_FLOOR ];
    assert( pTileData != NULL );

    return Tile( pTileData );
}

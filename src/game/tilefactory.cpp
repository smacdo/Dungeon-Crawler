#include <vector>
#include <string>

#include "game/tilefactory.h"
#include "game/tiledata.h"
#include "game/tileflags.h"
#include "game/tile.h"
#include "common/utils.h"

enum ETileType
{
    ETILETYPE_VOID,
    ETILETYPE_GRANITE,
    ETILETYPE_STONEWALL,
    ETILETYPE_STONEFLOOR,
    ETILETYPE_COUNT
};

/**
 * The tile factory constructor
 */
TileFactory::TileFactory()
    : mBlueprints( ETILETYPE_COUNT )
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

    pGraniteData->flags.set( ETILE_PLACED );
    pGraniteData->flags.set( ETILE_IMPASSABLE );
    pGraniteData->flags.set( ETILE_WALL );
    pGraniteData->flags.set( ETILE_BLOCK_LOS );

    mBlueprints[ pGraniteData->id ] = pGraniteData;

    // Construct stone wall tile
    TileData *pStoneWallData = new TileData;
    pStoneWallData->id       = ETILETYPE_STONEWALL;
    pStoneWallData->name     = "stone_wall";
    pStoneWallData->title    = "Stone Wall";

    pStoneWallData->flags.set( ETILE_PLACED );
    pStoneWallData->flags.set( ETILE_IMPASSABLE );
    pStoneWallData->flags.set( ETILE_WALL );
    pStoneWallData->flags.set( ETILE_BLOCK_LOS );

    mBlueprints[ pStoneWallData->id ] = pStoneWallData;

    // Construct stone floor tile
    TileData *pStoneFloorData = new TileData;
    pStoneFloorData->id       = ETILETYPE_STONEFLOOR;
    pStoneFloorData->name     = "stone_floor";
    pStoneFloorData->title    = "Stone Floor";

    pStoneFloorData->flags.set( ETILE_PLACED );
    pStoneFloorData->flags.set( ETILE_WALK );
    pStoneFloorData->flags.set( ETILE_FLOOR );

    mBlueprints[ pStoneFloorData->id ] = pStoneFloorData;
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
 * Create and return a wall
 */
Tile TileFactory::createWall() const
{
    TileData *pTileData = mBlueprints[ ETILETYPE_STONEWALL ];
    assert( pTileData != NULL );
    
    return Tile( pTileData );
}

/**
 * Create and return a floor
 */
Tile TileFactory::createFloor() const
{
    TileData *pTileData = mBlueprints[ ETILETYPE_STONEFLOOR ];
    assert( pTileData != NULL );

    return Tile( pTileData );
}

/*
 * Copyright 2012 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#include <vector>
#include <string>

#include "game/tilefactory.h"
#include "game/tileflags.h"
#include "game/tile.h"
#include "game/tiletype.h"
#include "common/utils.h"

/**
 * The tile factory constructor
 */
TileFactory::TileFactory()
    : mBlueprints( ETILETYPE_COUNT, 0 )
{
    // Construct void tile
    TileType *pVoidData = new TileType;
    pVoidData->id    = ETILETYPE_VOID;
    pVoidData->name  = "void";
    pVoidData->title = "Void Tile";

    mBlueprints[ pVoidData->id ] = pVoidData;

    // Construct granite tile
    TileType *pGraniteData = new TileType;
    pGraniteData->id       = ETILETYPE_GRANITE;
    pGraniteData->name     = "granite";
    pGraniteData->title    = "Granite Wall";

    pGraniteData->flags.set( ETILE_GRANITE );
    pGraniteData->flags.set( ETILE_IMPASSABLE );
    pGraniteData->flags.set( ETILE_WALL );
    pGraniteData->flags.set( ETILE_BLOCK_LOS );

    mBlueprints[ pGraniteData->id ] = pGraniteData;

    // Construct stone wall tile
    TileType *pStoneWallData = new TileType;
    pStoneWallData->id       = ETILETYPE_DUNGEON_WALL;
    pStoneWallData->name     = "stone_wall";
    pStoneWallData->title    = "Stone Wall";

    pStoneWallData->flags.set( ETILE_IMPASSABLE );
    pStoneWallData->flags.set( ETILE_WALL );
    pStoneWallData->flags.set( ETILE_BLOCK_LOS );

    mBlueprints[ pStoneWallData->id ] = pStoneWallData;

    // Construct stone floor tile
    TileType *pStoneFloorData = new TileType;
    pStoneFloorData->id       = ETILETYPE_DUNGEON_FLOOR;
    pStoneFloorData->name     = "stone_floor";
    pStoneFloorData->title    = "Stone Floor";

    pStoneFloorData->flags.set( ETILE_WALK );
    pStoneFloorData->flags.set( ETILE_FLOOR );

    mBlueprints[ pStoneFloorData->id ] = pStoneFloorData;

    // Construct stone tile filler tile
    TileType *pStoneFillerData = new TileType;
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
    TileType *pTileType = mBlueprints[ ETILETYPE_VOID ];
    assert( pTileType != NULL );

    return Tile( pTileType );
}

/**
 * Create and return a granite tile
 */
Tile TileFactory::createGranite() const
{
    TileType *pTileType = mBlueprints[ ETILETYPE_GRANITE ];
    assert( pTileType != NULL );

    return Tile( pTileType );
}

/**
 * Create and return generic "filler" tiles (stuff that exists in the empty area
 * inbetween dungeon rooms/halls
 */
Tile TileFactory::createFiller() const
{
    TileType *pTileType = mBlueprints[ ETILETYPE_FILLER_STONE ];
    assert( pTileType != NULL );

    return Tile( pTileType );
}

/**
 * Create and return a wall
 */
Tile TileFactory::createWall() const
{
    TileType *pTileType = mBlueprints[ ETILETYPE_DUNGEON_WALL ];
    assert( pTileType != NULL );
    
    return Tile( pTileType );
}

/**
 * Create and return a floor
 */
Tile TileFactory::createFloor() const
{
    TileType *pTileType = mBlueprints[ ETILETYPE_DUNGEON_FLOOR ];
    assert( pTileType != NULL );

    return Tile( pTileType );
}

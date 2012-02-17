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
#include "game/tiletype.h"
#include "game/tileflags.h"

/**
 * Default TileType constructor. This creates an "invalid" tile which is
 * also known as the void
 *
 * (Assumes that tile id = void!!)
 */
TileType::TileType()
    : mId( 0 ),
      mName( "void" ),
      mTitle( "Void Tile" ),
      mFlags()
{
    mFlags.set( ETILE_IMPASSABLE );
}

/**
 * Tile type constructor
 */
TileType::TileType( unsigned int id,
                    const std::string& name,
                    TileTypeFlagSet flags )
    : mId( id ),
      mName( name ),
      mTitle( name ),
      mFlags( flags )
{
}

/**
 * Tile data constructor
 *
 * \param  id     The tile id that uniquely identifies this tile type
 * \param  name   Name of this tile
 * \param  flags  Bit flags for this tile
 */
TileType::TileType( unsigned int id,
                    const std::string& name,
                    const std::string& title,
                    TileTypeFlagSet flags )
    : mId( id ),
      mName( name ),
      mTitle( title ),
      mFlags( flags )
{
}

/**
 * Return the tile's id
 */
unsigned int TileType::id() const
{
    return mId;
}

/**
 * Return the tile's name
 */
std::string TileType::name() const
{
    return mName;
}

/**
 * Return a reference to the tile's flags
 */
const TileTypeFlagSet& TileType::flags() const
{
    return mFlags;
}

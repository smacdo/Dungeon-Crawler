#include "world.h"
#include "dungeon.h"
#include "level.h"

#include "common/utils.h"

#include <memory>
#include <cassert>

/**
 * World constructor. Create a new world
 */
World::World( Dungeon* mainDungeon )
    : mMainDungeon( mainDungeon )
{
    assert( mainDungeon != NULL );
}

/**
 * Destructor. Blows up the world (oh noes)
 */
World::~World()
{
}

/**
 * Simulates zero or more time slices
 */
void World::simulate( size_t sliceCount )
{

}

std::shared_ptr<Dungeon> World::mainDungeon()
{
    return mMainDungeon;
}

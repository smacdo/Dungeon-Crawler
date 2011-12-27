#include "dungeon.h"
#include <vector>
#include <string>
#include <memory>
#include <cassert>

/**
 * Dungeon constructor
 *
 * \param  name       Name of the dungeon
 * \param  maxWidth   Maximum width for a level in the dungeon
 * \param  maxHeight  Maximum height for a level in the dungeon
 * \param  levels     Collection of levels in the dungeon
 */
Dungeon::Dungeon( const std::string& name,
                  size_t maxWidth,
                  size_t maxHeight,
                  const std::vector< std::shared_ptr<Level> >& levels )
    : mName( name ),
      mMaxWidth( maxWidth ),
      mMaxHeight( maxHeight ),
      mLevels( levels )
{
    
}

/**
 * Destructor. Note that when this is called, it will forcibly check that
 * it is the only owner left for the dungeon levels.
 */
Dungeon::~Dungeon()
{
    // Go through each level, make sure we are the sole owner before
    // deleting it
    for ( auto itr = mLevels.begin(); itr != mLevels.end(); ++itr )
    {
        assert( itr->use_count() == 1 );
        itr->reset();
    }
}

/**
 * Return the number of dungeon levels inside of this dungeon
 */
size_t Dungeon::levelCount() const
{
    return mLevels.size();
}

/**
 * Returns the name of the dungeon
 */
std::string Dungeon::name() const
{
    return mName;
}

/**
 * Returns the maximum width of a level in the dungeon
 */
size_t Dungeon::maxWidth() const
{
    return mMaxWidth;
}

/**
 * Returns the maximum height of a level in the dungeon
 */
size_t Dungeon::maxHeight() const
{
    return mMaxHeight;
}

/**
 * Returns a shared pointer to a level in the dungeon
 */
std::shared_ptr<Level> Dungeon::getLevel( size_t index )
{
    assert( index < mLevels.size() );
    return mLevels[index];
}

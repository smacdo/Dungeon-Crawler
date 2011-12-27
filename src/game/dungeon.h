#ifndef SCOTT_DUNGEON_DUNGEON_H
#define SCOTT_DUNGEON_DUNGEON_H

#include <vector>
#include <string>
#include <memory>
#include <boost/utility.hpp>

class Level;

/**
 * Holds all relevant information about a dungeon, including all of the
 * levels in it and all of the actors and items currently inside.
 */
class Dungeon : public boost::noncopyable
{
public:
    // Create a new dungeon object
    Dungeon( const std::string& name,
             size_t maxWidth,
             size_t maxHeight,
             const std::vector< std::shared_ptr<Level> >& levels );

    // Dungeon destructor
    ~Dungeon();
 
    // Return the number of levels contained in this dungeon
    size_t levelCount() const;

    // Return the name of this dungeon
    std::string name() const;

    // Return the width of the game
    size_t maxWidth() const;

    // Return the height of the game
    size_t maxHeight() const;

    // Get a level
    std::shared_ptr<Level> getLevel( size_t index );

private:
    std::string mName;
    size_t mMaxWidth;
    size_t mMaxHeight;
    std::vector< std::shared_ptr<Level> > mLevels;
};

#endif

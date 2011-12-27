#ifndef SCOTT_DUNGEON_WORLD_H
#define SCOTT_DUNGEON_WORLD_H

#include <memory>
#include <boost/utility.hpp>

class Dungeon;

/**
 * World is the class that is responsible for maintaing the state of
 * the entire game world, as well as high level simulation functionality.
 *
 * We might want to split this up in the future into a class for storing
 * the state of the game world, and a class for simulating it.
 */
class World : boost::noncopyable
{
public:
    World( Dungeon* mainDungeon );
    ~World();

    void simulate( size_t sliceCount );

    std::shared_ptr<Dungeon> mainDungeon();

protected:

private:
    // Right now we're only keeping track of one dungeon
    std::shared_ptr<Dungeon> mMainDungeon;
};

#endif


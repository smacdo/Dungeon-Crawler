#ifndef SCOTT_DUNGEON_LEVEL_GENERATOR
#define SCOTT_DUNGEON_LEVEL_GENERATOR

#include <cstddef>
#include <boost/noncopyable.hpp>

class RoomGenerator;
class Level;

class LevelGenerator : boost::noncopyable
{
public:
    LevelGenerator( RoomGenerator *pRoomGen,
                    size_t width,
                    size_t height );
    ~LevelGenerator();

    Level * generate();

protected:
    void emplaceLevelBorders( Level& level ) const;

private:
    RoomGenerator *mRoomGenerator;
    Level *mLevel;
    size_t mLevelWidth;
    size_t mLevelHeight;
};

#endif
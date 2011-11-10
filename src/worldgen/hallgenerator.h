#ifndef SCOTT_DUNGEON_HALLGENERATOR_H
#define SCOTT_DUNGEON_HALLGENERATOR_H

#include <boost/noncopyable.hpp>

struct RoomData;
class Random;

/**
 * Responsible for carving interesting hallways between rooms
 */
class HallGenerator : boost::noncopyable
{
public:
    HallGenerator( Random& random );
    ~HallGenerator();

    void connect( RoomData *pStartRoom, RoomData *pEndRoom );

private:
    void reset( RoomData *pStartRoom, RoomData *pEndRoom );

private:
    Random& mRandom;
    RoomData *mpStartRoom;
    RoomData *mpDestRoom;
};

#endif

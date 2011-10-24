#ifndef SCOTT_DUNGEON_CONNECTIONGENERATOR_H
#define SCOTT_DUNGEON_CONNECTIONGENERATOR_H

#include <boost/noncopyable.hpp>

class Random;

/**
 * Responsible for taking a set of rooms in a dungeon level, and connecting them
 * together. Will connect rooms with respect to settable parameters, and can be queried
 * for extra information
 */
class ConnectionGenerator : public boost::noncopyable
{
public:
    ConnectionGenerator( Random& random );
    ~ConnectionGenertor();

    void setMinMaxConnections( int min, int max );
    void connect( const std::vector<RoomData*>& rooms );

protected:
    void connectRoom( RoomData& room );

private:
    Random& mRandom;
    int mMinConnections;
    int mMaxConnections;
    int mRoomSizeMultiple;
};

#endif

#ifndef SCOTT_COMMON_TYPES_H
#define SCOTT_COMMON_TYPES_H

// Forward declaration of class boost::serialization::access
namespace boost
{
    namespace serialization
    {
        class access;
    }
}

/////////////////////////////////////////////////////////////////////////////
// Standard enumerations
/////////////////////////////////////////////////////////////////////////////

/**
 * Sizes of the room
 */
enum ERoomSize
{
    ROOM_SIZE_TINY,
    ROOM_SIZE_SMALL,
    ROOM_SIZE_MEDIUM,
    ROOM_SIZE_LARGE,
    ROOM_SIZE_HUGE,
    ROOM_SIZE_GIGANTIC,
    ERoomSize_COUNT
};

#endif

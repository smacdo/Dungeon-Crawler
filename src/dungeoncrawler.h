#ifndef SCOTT_DUNGEON_CRAWLER_H
#define SCOTT_DUNGEON_CRAWLER_H

#include "common/platform.h"

/////////////////////////////////////////////////////////////////////////////
// Standard enumerations
/////////////////////////////////////////////////////////////////////////////

enum EProgramStatus
{
    EPROGRAM_OK = 0,
    EPROGRAM_ASSERT_FAILED = 1,
    EPROGRAM_FATAL_ERROR   = 2,
    EPROGRAM_USER_ERROR    = 3
};

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

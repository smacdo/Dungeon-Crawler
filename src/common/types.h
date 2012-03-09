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

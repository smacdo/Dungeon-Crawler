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
#ifndef SCOTT_DUNGEON_INPUT_MANAGER_H
#define SCOTT_DUNGEON_INPUT_MANAGER_H

#include "common/point.h"

class InputManager
{
public:
    InputManager();
    ~InputManager();

    void process();

    bool didUserPressQuit() const;
    bool didUserMove() const;

    int userMoveXAxis() const;
    int userMoveYAxis() const;

    Point userMovement() const;

private:
    void processKeypress();

    bool mUserPressedQuit;
    bool mDidUserMove;

    Point mUserMovement;

    int mUserMoveX;
    int mUserMoveY;
};

#endif


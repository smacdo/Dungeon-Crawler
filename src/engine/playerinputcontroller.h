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
#ifndef SCOTT_DUNGEON_PLAYER_INPUT_CONTROLLER_H
#define SCOTT_DUNGEON_PLAYER_INPUT_CONTROLLER_H

#include <memory>
#include <boost/utility.hpp>

class Actor;
class InputManager;

/**
 * A plyaer input controller is the interface between the game's input state,
 * the actions fed into an actor and the resulting UI reactions
 */
class PlayerInputController : boost::noncopyable
{
public:
    PlayerInputController();
    ~PlayerInputController();

    void attachTo( std::shared_ptr<Actor> pActor );
    void update( const InputManager& input );

private:
    std::shared_ptr<Actor> mpActor;
};

#endif

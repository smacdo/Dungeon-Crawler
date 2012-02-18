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
#ifndef SCOTT_DUNGEON_SIMULATION_H
#define SCOTT_DUNGEON_SIMULATION_H

#include <memory>
#include "game/gameplayengine.h"

class TileFactory;
class World;
class Actor;

/**
 * This class is responsible for storing and simulating the world state.
 * [MORE DESCRIPTION PLZ]
 *
 * This class is not a full "engine", in that it only simulates the game. All
 * other traditional game engine roles such as sound and graphics are delegated
 * to other managers that the caller must construct.
 */
class GamePlayEngine
{
public:
    GamePlayEngine();
    ~GamePlayEngine();

    // Simulate the game world for one or more iterations
    void simulate( unsigned int iterations );

    // This creates a new world and spawns a player into it
    void createNewWorld();

    // Returns a reference to the engine's tile factory
    TileFactory& tileFactory();

    // Returns a constant reference to the game world
    const World& activeWorld() const;

private:
    World * mpWorld;
    TileFactory * mpTileFactory;
    std::shared_ptr<Actor> mpPlayerActor;
};

#endif

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
#include "engine/playerinputcontroller.h"
#include "engine/actor.h"
#include "inputmanager.h"

/**
 * Player input controller constructor
 */
PlayerInputController::PlayerInputController()
    : mpActor()
{
    // empty
}

/**
 * Player input controller destructor
 */
PlayerInputController::~PlayerInputController()
{
    // empty
}

/**
 * Attach this input controller to an actor. This will cause all input events
 * process by this controller to be routed to this actor
 */
void PlayerInputController::attachTo( std::shared_ptr<Actor> pActor )
{
    mpActor = pActor;
}

/**
 * Updates the attached actor with any input from the given input manager
 */
#include <iostream>
void PlayerInputController::update( const InputManager& input )
{
    // empty.. stupid simple for the moment. Switch to messaging and let
    // the actual controller figure it out
    if ( mpActor && input.didUserMove() )
    {
        Point direction = input.userMovement();
        mpActor->setPosition( mpActor->position() + direction );
    }
}

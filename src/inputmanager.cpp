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
#include "inputmanager.h"
#include <SDL2/SDL.h>
#include <cassert>

InputManager::InputManager()
    : mUserPressedQuit( false ),
      mDidUserMove( false ),
      mUserMovement(),
      mUserMoveX( 0 ),
      mUserMoveY( 0 )
{
    // SDL_Init stuff here?
}


InputManager::~InputManager()
{
}

/**
 * Processes the current user input
 */
void InputManager::process()
{
    SDL_Event event;

    // Clear out last time's input values
    mUserPressedQuit = false;
    mDidUserMove     = false;
    mUserMovement    = Point( 0, 0 );

    mUserMoveX = 0;
    mUserMoveY = 0;

    // Process input events until we run out of ones to process
    while ( SDL_PollEvent( &event ) )
    {
        switch ( event.type )
        {
            case SDL_QUIT:
                mUserPressedQuit = true;
                break;

            case SDL_KEYDOWN:
            case SDL_KEYUP:
                processKeypress( event );
                break;

            default:
                break;
        }
    }
}

/**
 * Processes keyboard inputs
 */
void InputManager::processKeypress( const SDL_Event& event )
{
    assert( event.type == SDL_KEYDOWN || event.type == SDL_KEYUP );

    // right now we don't care if keys are up or down... just register
    // based on down position
    if ( event.type == SDL_KEYUP )
    {
        return;
    }

    // what happened?
    switch ( event.key.keysym.sym )
    {
        case SDLK_ESCAPE:
            mUserPressedQuit = true;
            break;

        case SDLK_UP:
            mUserMoveY   = -1;
            break;

        case SDLK_DOWN:
            mUserMoveY   = 1;
            break;

        case SDLK_LEFT:
            mUserMoveX   = -1;
            break;

        case SDLK_RIGHT:
            mUserMoveX   = 1;
            break;

        case SDLK_KP_7:
            mUserMovement = Point( -1, -1 );
            break;

        case SDLK_KP_8:
            mUserMovement = Point( 0, -1 );
            break;

        case SDLK_KP_9:
            mUserMovement = Point( 1, -1 );
            break;

        case SDLK_KP_4:
            mUserMovement = Point( -1, 0 );
            break;

        case SDLK_KP_6:
            mUserMovement = Point( 1, 0 );
            break;

        case SDLK_KP_1:
            mUserMovement = Point( -1, 1 );
            break;

        case SDLK_KP_2:
            mUserMovement = Point( 0, 1 );
            break;

        case SDLK_KP_3:
            mUserMovement = Point( 1, 1 );
            break;

        default:
            break;
    }

    if ( mUserMovement.x() != 0 || mUserMovement.y() != 0 )
    {
        mDidUserMove = true;
    }
}

/**
 * Checks if the user wants out
 */
bool InputManager::didUserPressQuit() const
{
    return mUserPressedQuit;
}

bool InputManager::didUserMove() const
{
    return mDidUserMove;
}

int InputManager::userMoveXAxis() const
{
    return mUserMoveX;
}

int InputManager::userMoveYAxis() const
{
    return mUserMoveY;
}

Point InputManager::userMovement() const
{
    return mUserMovement;
}

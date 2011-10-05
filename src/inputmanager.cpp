#include "inputmanager.h"
#include <SDL.h>

InputManager::InputManager()
    : mUserPressedQuit( false )
{
}


InputManager::~InputManager()
{
}

/**
 * Processes the current user input
 */
void InputManager::processInput()
{
    SDL_Event event;

    while ( SDL_PollEvent( &event ) )
    {
        switch ( event.type )
        {
            case SDL_QUIT:
                mUserPressedQuit = true;
                break;

            case SDL_KEYDOWN:
            {
                if  ( event.key.keysym.sym = SDLK_ESCAPE )
                {
                    mUserPressedQuit = true;
                }
            }
        }
    }
}

/**
 * Checks if the user wants out
 */
bool InputManager::didUserPressQuit() const
{
    return mUserPressedQuit;
}

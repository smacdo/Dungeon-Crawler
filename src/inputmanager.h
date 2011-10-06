#ifndef SCOTT_DUNGEON_INPUT_MANAGER_H
#define SCOTT_DUNGEON_INPUT_MANAGER_H

#include <SDL.h>

class InputManager
{
public:
    InputManager();
    ~InputManager();

    void processInput();

    bool didUserPressQuit() const;
    bool didUserMove() const;

    int userMoveXAxis() const;
    int userMoveYAxis() const;

private:
    void processKeypress( const SDL_Event& event );

    bool mUserPressedQuit;
    bool mDidUserMove;

    int mUserMoveX;
    int mUserMoveY;
};

#endif


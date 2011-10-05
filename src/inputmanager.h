#ifndef SCOTT_DUNGEON_INPUT_MANAGER_H
#define SCOTT_DUNGEON_INPUT_MANAGER_H

class InputManager
{
public:
    InputManager();
    ~InputManager();

    void processInput();

    bool didUserPressQuit() const;

private:
    bool mUserPressedQuit;
};

#endif


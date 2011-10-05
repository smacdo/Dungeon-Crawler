#ifndef SCOTT_DUNGEON_DOOR_H
#defune SCOTT_DUNGEON_DOOR_H

class Room;

class Door
{
private:
    // Position of the door
    size_t mRow, mCol;

    // Door status
    bool mIsLocked;

    // The room that the door is connected to
    Room * mRoom;
};

#endif

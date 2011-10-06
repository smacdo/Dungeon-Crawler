#ifndef SCOTT_DUNGEON_ROOM_GENERATOR_H
#define SCOTT_DUNGEON_ROOM_GENERATOR_H

class Rect;
class Room;
class Level;

/**
 * Makes and builds rooms. Can be subclassed to generate thematic levels
 * with similiar (or different!) room types and whatnot.
 *
 * For example, you could subclass the room generator to make crypt levels,
 * a castle room generator, etc.
 */
class RoomGenerator
{
public:
    RoomGenerator();
    ~RoomGenerator();
    
    void setLevel( Level* level );

    /**
     * Generates a room to place in the requested area. This method
     * can potentially refuse to place a level, in which case the returned
     * pointer will be null
     */
    Room* generate( const Rect& area );

protected:
    void addRandomDoor( Room& pRoom ) const;

private:
    Level * mLevel;
};

#endif

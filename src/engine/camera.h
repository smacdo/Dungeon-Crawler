#ifndef SCOTT_DUNGEON_CAMERA_H
#define SCOTT_DUNGEON_CAMERA_H

/**
 * Camera defines a visible region of the dungeon.
 *
 * The camera uses a tile to denote a single unit, and a fractional
 * value (0.0f-1.0f) to denote a percentage of tile. Whenever
 * referring to a tile, we use its upper left hand corner.
 *
 * So if the camera's x/y is set at (3,2) then the entire tile at
 * row 2, column 3 is visible. If (3+0.25,2) then it is the same, except
 * that 0.25 in the x direction is obscurred
 */
class Camera
{
public:
    Camera( int upperX, int upperY,
            int visibleWidth, int visibleHeight,
            int levelWidth,   int levelHeight );
    ~Camera();

    void move( int x, int y );

    bool isVisible( int tileX, int tileY ) const;

private:
    int mUpperX, mUpperY;
    int mVisibleWidth, mVisibleHeight;
    int mLevelWidth, mLevelHeight;
    float mDX, mDY;
};

#endif


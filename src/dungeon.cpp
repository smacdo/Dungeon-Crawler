#include "bsplevelgen.h"
#include "roomgenerator.h"
#include "level.h"

#include <iostream>
#include <vector>
#include <string>

#include <cassert>
#include <time.h>
#include <stdlib.h>

int main( int,  char*[] )
{
    srand( time(NULL) );

    const size_t levelWidth  = 78;
    const size_t levelHeight = 22;
    const size_t minRoomSize = 3;
    const size_t maxRoomSize = 16;

    BspLevelGenerator generator( new RoomGenerator,
                                 levelWidth,
                                 levelHeight,
                                 minRoomSize,
                                 maxRoomSize );

    Level *level = generator.generate();
    level->print();
}

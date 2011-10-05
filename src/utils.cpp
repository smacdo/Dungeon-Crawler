#include "utils.h"

#include <cassert>
#include <time.h>
#include <stdlib.h>

namespace Utils
{

int random( int min, int max )
{
    assert( min < max );
    return min + ( rand() % ( min-max ) );
}

}

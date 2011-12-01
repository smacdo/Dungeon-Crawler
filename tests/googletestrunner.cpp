#include "googletest/googletest.h"
#include "common/platform.h"

int main( int argc, char * argv[] )
{
    App::setTestingMode( true );

    ::testing::InitGoogleTest( &argc, argv );
    return RUN_ALL_TESTS();
}

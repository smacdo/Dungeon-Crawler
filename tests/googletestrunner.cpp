#include "googletest/googletest.h"
#include "common/platform.h"

int main( int argc, char * argv[] )
{
    App::setIsInUnitTestMode( true );

    ::testing::InitGoogleTest( &argc, argv );
    return RUN_ALL_TESTS();
}

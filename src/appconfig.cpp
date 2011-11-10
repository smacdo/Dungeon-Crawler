#include "appconfig.h"
#include "config.h"

#include <string>

AppConfig::AppConfig()
    : rwWidth( DEFAULT_RENDER_WIDTH ),
      rwHeight( DEFAULT_RENDER_HEIGHT ),
      rwFullscreen( DEFAULT_FULLSCREEN ),
      rwDriver( "default" ),
      randomSeed( 0 ),
      shouldLaunchGame( true )
{
}

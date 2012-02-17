#include "appconfig.h"

#include <string>

const int DEFAULT_RENDER_WIDTH  = 800;
const int DEFAULT_RENDER_HEIGHT = 600;
const bool DEFAULT_FULLSCREEN   = false;

AppConfig::AppConfig()
    : rwWidth( DEFAULT_RENDER_WIDTH ),
      rwHeight( DEFAULT_RENDER_HEIGHT ),
      rwFullscreen( DEFAULT_FULLSCREEN ),
      randomSeed( 0 ),
      shouldLaunchGame( true )
{
}

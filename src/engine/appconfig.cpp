/*
 * Copyright 2012 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#include "engine/appconfig.h"

#include <string>

/*
 * Hardcoded default values. Feel free to modify these if you want, but the
 * game should prefer to load values from a settings file
 */
const int DEFAULT_RENDER_WIDTH  = 800;
const int DEFAULT_RENDER_HEIGHT = 600;
const bool DEFAULT_FULLSCREEN   = false;

/**
 * AppConfig default constructor. Sets logical default values for the
 * applications settings
 */
AppConfig::AppConfig()
    : debug( false ),
      quiet( false ),
      rwWidth( DEFAULT_RENDER_WIDTH ),
      rwHeight( DEFAULT_RENDER_HEIGHT ),
      rwFullscreen( DEFAULT_FULLSCREEN ),
      randomSeed( 0 ),
      shouldLaunchGame( true )
{
}

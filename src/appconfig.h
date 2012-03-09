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
#ifndef SCOTT_DUNGEON_CONFIGURATION_H
#define SCOTT_DUNGEON_CONFIGURATION_H

#include <string>

/**
 * This class loads and stores application specific values.
 *
 * \TODO Turn this into a class
 * \TODO Convert dimensions into unsigned ints
 * \TODO Use boost program_options to better specify command line settings
 * \TODO Add ability to load settings from disk
 * \TODO Add ability to save settings back to disk
 */
struct AppConfig
{
    AppConfig();

    void readCommandLine( int argc, char** argv );
    void readConfigFile( const std::string& path );

    int rwWidth;
    int rwHeight;
    bool rwFullscreen;
    std::string rwDriver;
    int randomSeed;
    bool shouldLaunchGame;
};


#endif

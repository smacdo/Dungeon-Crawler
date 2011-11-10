#ifndef SCOTT_DUNGEON_CONFIGURATION_H
#define SCOTT_DUNGEON_CONFIGURATION_H

#include <string>

struct AppConfig
{
    AppConfig();

    int rwWidth;
    int rwHeight;
    bool rwFullscreen;
    std::string rwDriver;
    int randomSeed;
    bool shouldLaunchGame;
};


#endif

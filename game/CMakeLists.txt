###########################################################################
### CMake script for the game data folder. Copies and prepares all of   ###
### required data for the game when building                            ###
###########################################################################
file( COPY spritesheets/tiles.png spritesheets/tiles.xml
      DESTINATION ${GAME_PATH}/data/sprites )

###########################################################################
### CMake script for the game data folder. Copies and prepares all of   ###
### required data for the game when building                            ###
###########################################################################
set(OUTDIR "${GAME_OUTPUT_PATH}/data")
file( COPY spritesheets/tiles.png spritesheets/tiles.xml
           spritesheets/players.png spritesheets/players.xml
      DESTINATION ${OUTDIR}/sprites )

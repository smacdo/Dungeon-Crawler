#!/bin/bash
#######################################################################
# Regenerates game assets
./scripts/generate-spritesheet.rb assets/sprites/tiles game/spritesheets tiles 
./scripts/generate-spritesheet.rb assets/sprites/players/ game/spritesheets players
./scripts/generate-spritesheet.rb assets/sprites/monsters/ game/spritesheets monsters

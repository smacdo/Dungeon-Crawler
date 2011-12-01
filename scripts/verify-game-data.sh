#!/bin/bash
gamedir="game"
datadir="game/data"
xlint="xmllint --noout"

###
### Validate sprite data files
###
`$xlint --schema $datadir/sprites.xsd $datadir/sprites.xml`

if [ $? -ne 0 ]; then
    echo "sprites.xml failed xml validation"
    exit 1
fi

###
### All done
###
echo "Verification finished, looks like everything is OK!"

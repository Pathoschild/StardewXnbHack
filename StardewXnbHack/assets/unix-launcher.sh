#!/bin/bash
# MonoKickstart Shell Script
# Written by Ethan "flibitijibibo" Lee
# Modified for StardewXnbHack to check that it's run from the game folder, and copy the bin files from the game's.

# Move to script's directory
cd "`dirname "$0"`"

# validate script is being run from the game folder
if [ ! -f "StardewValley.exe" ] && [ ! -f "Stardew Valley.exe" ] || [ ! -d "mono" ]; then
    echo "Oops! You must run StardewXnbHack in your Stardew Valley game folder. See usage instructions: https://github.com/Pathoschild/StardewXnbHack#readme.";
    read
else
    # Get the system architecture
    UNAME=`uname`
    ARCH=`uname -m`

    # MonoKickstart picks the right libfolder, so just execute the right binary.
    if [ "$UNAME" == "Darwin" ]; then
        # ... Except on OSX.
        export DYLD_LIBRARY_PATH=$DYLD_LIBRARY_PATH:./osx/

        # El Capitan is a total idiot and wipes this variable out, making the
        # Steam overlay disappear. This sidesteps "System Integrity Protection"
        # and resets the variable with Valve's own variable (they provided this
        # fix by the way, thanks Valve!). Note that you will need to update your
        # launch configuration to the script location, NOT just the app location
        # (i.e. Kick.app/Contents/MacOS/Kick, not just Kick.app).
        # -flibit
        if [ "$STEAM_DYLD_INSERT_LIBRARIES" != "" ] && [ "$DYLD_INSERT_LIBRARIES" == "" ]; then
            export DYLD_INSERT_LIBRARIES="$STEAM_DYLD_INSERT_LIBRARIES"
        fi

        ln -sf mcs.bin.osx mcs
        cp StardewValley.bin.osx StardewXnbHack.bin.osx
        ./StardewXnbHack.bin.osx $@
    else
        if [ "$ARCH" == "x86_64" ]; then
            ln -sf mcs.bin.x86_64 mcs
            cp StardewValley.bin.x86_64 StardewXnbHack.bin.x86_64
            ./StardewXnbHack.bin.x86_64 $@
        else
            ln -sf mcs.bin.x86 mcs
            cp StardewValley.bin.x86 StardewXnbHack.bin.x86
            ./StardewXnbHack.bin.x86 $@
        fi
    fi
fi

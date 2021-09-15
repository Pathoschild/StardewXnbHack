# Move to script's directory
cd "`dirname "$0"`"

# validate script is being run from the game folder
if [ ! -f "Stardew Valley.dll" ]; then
    echo "Oops! StardewXnbHack must be placed in the Stardew Valley game folder.\nSee instructions: https://github.com/Pathoschild/StardewXnbHack#readme.";
    read
elif [ ! -f "StardewModdingAPI.dll" ]; then
    echo "Oops! SMAPI must be installed in the game folder to use StardewXnbHack.\nSee instructions: https://github.com/Pathoschild/StardewXnbHack#readme.";
    read
else
    # copy game's platform configs
    cp "Stardew Valley.runtimeconfig.json" StardewXnbHack.runtimeconfig.json
    cp "Stardew Valley.deps.json" StardewXnbHack.deps.json

    # launch app
    ./StardewXnbHack

    read
fi

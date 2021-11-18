# Move to script's directory
cd "`dirname "$0"`"

# script must be in the game folder
if [ ! -f "Stardew Valley.dll" ]; then
    echo "Oops! StardewXnbHack must be placed in the Stardew Valley game folder.\nSee instructions: https://github.com/Pathoschild/StardewXnbHack#readme.";
    read

# SMAPI must be installed
elif [ ! -f "StardewModdingAPI.dll" ]; then
    echo "Oops! SMAPI must be installed in the game folder to use StardewXnbHack.\nSee instructions: https://github.com/Pathoschild/StardewXnbHack#readme.";
    read

# .NET 5 must be installed
elif ! command -v dotnet >/dev/null 2>&1; then
    echo "Oops! You must have .NET 5 installed to use SMAPI: https://dotnet.microsoft.com/download";
    read
elif ! (dotnet --info | grep "Microsoft.NETCore.App 5." 1>nul); then
    echo "Oops! You must have .NET 5 installed to use SMAPI: https://dotnet.microsoft.com/download";
    read

# run script
else
    # copy game's platform configs
    cp "Stardew Valley.runtimeconfig.json" StardewXnbHack.runtimeconfig.json
    cp "Stardew Valley.deps.json" StardewXnbHack.deps.json

    # launch app
    dotnet StardewXnbHack.dll
    read
fi

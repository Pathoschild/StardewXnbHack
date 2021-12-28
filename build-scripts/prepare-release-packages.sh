#!/bin/bash


##########
## Constants
##########
gamePath="/home/pathoschild/Stardew Valley"
buildConfig="Release"
platforms=("Linux" "macOS" "Windows")
declare -A runtimes=(["Linux"]="linux-x64" ["macOS"]="osx-x64" ["Windows"]="win-x64")
declare -A msBuildPlatformNames=(["Linux"]="Unix" ["macOS"]="OSX" ["Windows"]="Windows_NT")


##########
## Move to solution root
##########
cd "`dirname "$0"`/.."


##########
## Clear old build files
##########
echo "Clearing old builds..."
echo "-----------------------"
for path in **/bin **/obj; do
    echo "$path"
    rm -rf "$path"
done
rm -rf "bin"
echo ""


##########
## Compile files
##########
version="$1"
if [ $# -eq 0 ]; then
    echo "StardewXnbHack release version (like '2.0.0'):"
    read version
fi


##########
## Compile files
##########
for platform in ${platforms[@]}; do
    # constants
    runtime=${runtimes[$platform]}
    msbuildPlatformName=${msBuildPlatformNames[$platform]}
    binPath="StardewXnbHack/bin/Release/net6.0/$runtime/publish"
    folderName="StardewXnbHack $version for $platform"
    bundlePath="bin/$folderName"

    # compile
    echo "Compiling for $platform..."
    echo "--------------------------"
    dotnet publish StardewXnbHack --configuration $buildConfig -v minimal --runtime "$runtime" -p:OS="$msbuildPlatformName" -p:GamePath="$gamePath" -p:PublishSingleFile=True --self-contained true
    echo ""
    echo ""

    # build package folder
    echo "Preparing package for $platform..."
    echo "----------------------------------"
    mkdir "$bundlePath" --parents
    cp "$binPath/StardewXnbHack"* "$bundlePath"

    if [ -t "$bundlePath/StardewXnbHack" ]; then
        chmod 755 "$bundlePath/StardewXnbHack"
    fi

    # zip package
    pushd bin > /dev/null
    zip -9 "$folderName.zip" "$folderName" --recurse-paths --quiet
    popd > /dev/null
    echo "Package created at $(pwd)/bin/$folderName.zip"
    echo ""
    echo ""
done
exit

echo "Done!"

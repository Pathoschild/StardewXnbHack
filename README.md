**StardewXnbHack** is a one-way XNB unpacker for Stardew Valley. It supports every Stardew Valley
asset type, is very easy to update for game changes, and is quick at unpacking many files at once.

![](StardewXnbHack/assets/icon.png)

## Usage
**Note: these instructions are for the upcoming 1.0 release. See the FAQs to compile it yourself
if you want to use it before release.**

To install it:

1. Install [Stardew Valley](https://www.stardewvalley.net/) and [SMAPI](https://smapi.io/).
2. From the [releases page](https://github.com/Pathoschild/StardewXnbHack/releases), download the
   `StardewXnbHack *.zip` file for your operating system under 'assets'.
3. Unzip it into your Stardew Valley folder, so `StardewXnbHack.exe` is in the same folder as
   `Stardew Valley.exe`.

To unpack the entire `Content` folder into `Content (unpacked)` on...

OS      | instruction
------- | -----------
Linux   | execute `StardewXnbHack.sh`.
MacOS   | double-click `StardewXnbHack.command`.
Windows | double-click `StardewXnbHack.exe`.

## FAQs
### How does this compare to other XNB unpackers?
StardewXnbHack reads files through a temporary game instance, unlike other unpackers which read
them directly. That lets it support custom Stardew Valley formats, but it can't repack files (which
is [rarely needed anyway](https://stardewvalleywiki.com/Modding:Content_Patcher)) or support other
games.

The main differences at a glance:

&nbsp;                | Stardew XNB Hack | [xnbcli](https://github.com/LeonBlade/xnbcli/) | [XNBExtract](https://community.playstarbound.com/threads/110976)
--------------------- | ---------------- | ------ | -----------
Supported asset types | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font texture<br />✓ font XML data<br />✓ structured data | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font textures<br />✓ font XML data<br />❑ structured data | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font textures<br />❑ font XML data<br />❑ structured data
Export formats | ✓ `.png` for images<br />✓ `.tmx` for maps<br />✓ `.json` for data ([CP](https://stardewvalleywiki.com/Modding:Content_Patcher)-compatible) | ✓ `.png` for images<br />✓ `.tbin` for maps¹<br />❑ `.json` for data (custom format) | ✓ `.png` for images<br />✓ `.tbin` for maps¹<br />❑ `.yaml` for data
Supported platforms | ✓ Windows<br />✓ Linux²<br />✓ Mac² | ✓ Windows<br />✓ Linux<br />✓ Mac | ✓ Windows<br />❑ Linux<br />❑ Mac
Supported operations | ✓ unpack<br />❑ pack | ✓ unpack<br />✓ pack  (uncompressed) | ✓ unpack<br />✓ pack
Maintainable | ✓ simple hack, easy to update | ❑ complex | ❑ complex, closed-source
Sample unpack time<br />(full `Content` folder) | ≈0m 43s | ≈6m 5s | ≈2m 20s
License | MIT | GPL | n/a

<sup>¹ `.tmx` is the [preferred map format](https://stardewvalleywiki.com/Modding:Maps#Map_formats), but you can open the `.tbin` file in Tiled and export it as `.tmx`.</sup>  
<sup>² Some sprite font textures can't be unpacked on Linux/Mac, but all other assets unpack fine.</sup>

## For StardewXnbHack developers
This section explains how to edit or compile StardewXnbHack from the source code. Most users should
[use an official release](#usage) instead.

### Compile from source
1. Install [Stardew Valley](https://www.stardewvalley.net/) and [SMAPI](https://smapi.io/).
2. Open the `.sln` solution file in [Visual Studio](https://visualstudio.microsoft.com/vs/).
3. Click _Build > Build Solution_. (If it doesn't find the Stardew Valley folder automatically, see
   [_custom game path_ in the mod build package readme](https://smapi.io/package/custom-game-path).)

### Debug a local build
Just launch the project via _Debug > Start Debugging_. It will run from your `bin` folder, but
it'll auto-detect your game folder and unpack its `Content` folder.

### Prepare a compiled release
To prepare a crossplatform SMAPI release, you'll need to compile it on two platforms. See
[crossplatforming info](https://stardewvalleywiki.com/Modding:Modder_Guide/Test_and_Troubleshoot#Testing_on_all_platforms)
on the wiki for the first-time setup.

1. Update the version numbers in `StardewXnbHack.csproj` using a [semantic version](https://semver.org).

2. In Windows:
   1. Rebuild the solution with the _release_ solution configuration.
   2. Zip `StardewXnbHack.exe` and `StardewXnbHack.exe.config` into a file with the version number
      and OS, like `StardewXnbHack 1.0.0 (Windows).zip`.

3. In Linux/Mac:
   1. Rebuild the solution with the _release_ solution configuration.
   2. Create two zips containing these files:
      ```
      StardewXnbHack 1.0.0 (Linux).zip/
         StardewXnbHack.exe
         StardewXnbHack.exe.config
         StardewXnbHack.sh

      StardewXnbHack 1.0.0 (MacOS).zip/
         StardewXnbHack.command
         StardewXnbHack.exe
         StardewXnbHack.exe.config
      ```

4. Post a release with all three zip files.

## See also
* [Release notes](release-notes.md)

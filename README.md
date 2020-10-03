**StardewXnbHack** is a one-way XNB unpacker for Stardew Valley.

It supports every Stardew Valley asset type, is very easy to update for game changes, and is quick
at unpacking many files at once.

## Usage
**Note: these instructions are for the upcoming 1.0 release. See the FAQs to compile it yourself
if you want to use it before release.**

To install it:

1. Install [Stardew Valley](https://www.stardewvalley.net/) and [SMAPI](https://smapi.io/).
2. From the [releases page](https://github.com/Pathoschild/SMAPI/releases), download the
   `StardewXnbHack *.zip` file under 'assets'.
3. Unzip it into your Stardew Valley folder, so `StardewXnbHack.exe` is in the same folder as
   `Stardew Valley.exe`.

To unpack files, just double-click `StardewXnbHack.exe`. It'll unpack the `Content` folder into
`Content (unpacked)` automatically.

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
Export formats | ✓ `.png` for images<br />✓ `.tmx` for maps<br />✓ `.json` for data ([CP](https://www.nexusmods.com/stardewvalley/mods/1915)-compatible) | ✓ `.png` for images<br />✓ `.tbin` for maps¹<br />❑ `.json` for data (custom format) | ✓ `.png` for images<br />✓ `.tbin` for maps¹<br />❑ `.yaml` for data
Supported platforms | ✓ Windows<br />✓ Linux²<br />✓ Mac² | ✓ Windows<br />✓ Linux<br />✓ Mac | ✓ Windows<br />❑ Linux<br />❑ Mac
Supported operations | ✓ unpack<br />❑ pack | ✓ unpack<br />✓ pack  (uncompressed) | ✓ unpack<br />✓ pack
Maintainable | ✓ simple hack, easy to update | ❑ complex | ❑ complex, closed-source
Sample unpack time<br />(full `Content` folder) | ≈0m 40s | ≈6m 5s | ≈2m 20s
License | MIT | GPL | n/a

<sup>¹ `.tmx` is the preferred map format for mods since it enables more features, but you can open the `.tbin` file in Tiled and export it as `.tmx`.</sup>  
<sup>² Some sprite font textures can't be unpacked on Linux/Mac, but all other assets unpack fine.</sup>

### Can I compile the code myself?
In most cases you should [use the release version](#usage) instead. If you really want to compile
it yourself:

1. Install [Stardew Valley](https://www.stardewvalley.net/) and [SMAPI](https://smapi.io/).
2. Open the `.sln` solution file in [Visual Studio](https://visualstudio.microsoft.com/vs/).
3. Click _Build > Build Solution_. (If it doesn't find the Stardew Valley folder automatically, see
   [_custom game path_ in the mod build package readme](https://smapi.io/package/custom-game-path).)

That's it! Just click _Debug > Start without debugging_ in Visual Studio (or run `StardewXnbHack.exe`
in the `bin` folder) to unpack with the default options. This will export the files to a
`Content (unpacked)` folder in the game folder.

## See also
* [Release notes](release-notes.md)

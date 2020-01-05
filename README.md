**Stardew XNB Hack** is a simple one-way XNB unpacker for Stardew Valley.

It supports every Stardew Valley asset type, is very easy to update for game changes, and is quick
at unpacking many files at once, but it sacrifices almost everything else to achieve that. For most
users, [xnbcli is recommended](https://stardewvalleywiki.com/Modding:Editing_XNB_files#Unpack_game_files)
instead.

## Compared to other XNB unpackers
&nbsp;                | Stardew XNB Hack | [xnbcli](https://github.com/LeonBlade/xnbcli/) | [XNBExtract](https://community.playstarbound.com/threads/110976)
--------------------- | ---------------- | ------ | -----------
Supported asset types | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font texture<br />✓ font XML data<br />✓ structured data | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font textures<br />✓ font XML data<br />❑ structured data | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font textures<br />❑ font XML data<br />❑ structured data
Export formats | ✓ `.png` for images<br />✓ `.tbin` for maps<br />✓ `.json` for data ([CP](https://www.nexusmods.com/stardewvalley/mods/1915)-compatible) | ✓ `.png` for images<br />✓ `.tbin` for maps<br />❑ `.json` for data (wrapped format) | ✓ `.png` for images<br />✓ `.tbin` for maps<br />❑ `.yaml` for data
Supported platforms | ✓ Windows<br />✓ Linux¹<br />✓ Mac¹ | ✓ Windows<br />✓ Linux<br />✓ Mac | ✓ Windows<br />❑ Linux<br />❑ Mac
Supported operations | ✓ unpack<br />❑ pack | ✓ unpack<br />✓ pack  (uncompressed) | ✓ unpack<br />✓ pack
User-friendly | ❑ run in Visual Studio<br />❑ unpack `Content` folder<br />❑ no command line | ✓ run script<br />✓ unpack specific files<br />✓ command line | ✓ run script<br />✓ unpack specific files<br />✓ command line
Maintainable | ✓ simple hack, easy to update | ❑ complex | ❑ complex, closed-source
Sample unpack time<br />(full `Content` folder) | ≈0m 40s | ≈6m 5s | ≈2m 20s
License | MIT | GPL | n/a

¹ Some sprite font textures can't be unpacked on Linux/Mac, but all other assets unpack fine.

## Usage
### Compile the code
1. Install the latest version of [SMAPI](https://smapi.io/). (The unpacker uses the SMAPI toolkit.)
2. Open the `.csproj` project file in [Visual Studio](https://visualstudio.microsoft.com/vs/).
3. Click _Build > Build Solution_. (If it doesn't find the Stardew Valley folder automatically, see
   [_custom game path_ in the mod build package readme](https://smapi.io/package/custom-game-path).)
4. See the compiled files in the project's `bin` folder.

### Run the tool
Just run `StardewXnbHack.exe` (or click _Debug > Start without debugging_ in Visual Studio) to
unpack with the default options. This will export the files to a `Content (unpacked)` folder in the
game folder.

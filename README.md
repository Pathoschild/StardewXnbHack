**Stardew XNB Hack** is a simple one-way XNB unpacker for Stardew Valley.

It supports every Stardew Valley asset type, is very easy to update for game changes, and is quick
at unpacking many files at once, but it sacrifices almost everything else to achieve that. For most
users, [xnbcli is recommended](https://stardewvalleywiki.com/Modding:Editing_XNB_files#Unpack_game_files)
instead.

## Compared to other XNB unpackers
&nbsp;                | Stardew XNB Hack | [xnbcli](https://github.com/LeonBlade/xnbcli/) | [XNBExtract](https://community.playstarbound.com/threads/110976)
--------------------- | ---------------- | ------ | -----------
Supported asset types | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font texture<br />✓ font XML data<br />✓ structured data | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font textures<br />✓ font XML data<br />❑ structured data | ✓ images<br />✓ maps<br />✓ dictionary data<br />✓ font textures<br />❑ font XML data<br />❑ structured data
Export formats | ✓ `.png` for images<br />✓ `.tbin` for maps<br />✓ `.json` for data ([CP](https://www.nexusmods.com/stardewvalley/mods/1915)-compatible) | ✓ `.png` for images<br />✓ `.tbin` for maps<br />❑ `.json` for data (with added metadata) | ✓ `.png` for images<br />✓ `.tbin` for maps<br />❑ `.yaml` for data
Supported platforms | ✓ Windows<br />≈ Linux¹<br />≈ Mac¹ | ✓ Windows<br />✓ Linux<br />✓ Mac | ✓ Windows<br />❑ Linux<br />❑ Mac
Supported operations | ✓ unpack<br />❑ pack | ✓ unpack<br />✓ pack  (uncompressed) | ✓ unpack<br />✓ pack
User-friendly | ❑ run in Visual Studio<br />❑ unpack `Content` folder<br />❑ no command line | ✓ run script<br />✓ unpack specific files<br />✓ command line | ✓ run script<br />✓ unpack specific files<br />✓ command line
Maintainable | ✓ simple hack, easy to update | ❑ complex | ❑ complex, closed-source
Sample unpack time<br />(full `Content` folder) | ≈0m 40s | ≈6m 5s | ≈2m 20s
License | MIT | GPL | n/a

¹ Some sprite font textures can't be unpacked on Linux/Mac; everything else should work.

## Usage
1. Install [SMAPI 3.0 or later](https://smapi.io/). (The unpacker uses the SMAPI toolkit.)
2. Open the project in Visual Studio.
3. Click _Build > Build Solution_. (If it doesn't find the Stardew Valley folder automatically, see
   [_game path_ in the mod build package readme](https://github.com/Pathoschild/SMAPI/blob/develop/docs/mod-build-config.md#game-path).)
4. Click _Debug > Start without debugging_ to run the unpacker.
5. The entire `Content` folder for the detected game will be unpacked into `Content (unpacked)`.

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
Supported operations | ✓ unpack<br />❑ pack | ✓ unpack<br />✓ pack  (uncompressed) | ✓ unpack<br />✓ pack
Supported platforms | ✓ Windows<br />❑ Linux<br />❑ Mac | ✓ Windows<br />✓ Linux<br />✓ Mac | ✓ Windows<br />❑ Linux<br />❑ Mac
User-friendly | ❑ run in Visual Studio<br />❑ unpack whole `Content` folder<br />❑ no command-line interface | ✓ run script<br />✓ unpack selected files<br />✓ command-line interface | ✓ run script<br />✓ unpack selected files<br />✓ command-line interface
Maintainable | ✓ simple hack, easy to update | ❑ complex | ❑ complex, closed-source
Sample unpack time<br />(full `Content` folder) | ≈0m 40s | ≈6m 5s | ≈2m 20s
License | MIT | GPL | n/a

## Usage
1. Open the project in Visual Studio.
2. Click _Build > Build Solution_. It should find the Stardew Valley folder automatically.
3. Click _Debug > Start without debugging_ to run the unpacker.
4. The entire `Content` folder for the detected game will be unpacked into `Content (unpacked)`.

[← back to readme](README.md)

# Release notes
## Upcoming release
* Added support for running unpacker from...
  * within the game folder (thanks to Platonymous!);
  * another app or an in-game mod.
* Changed BMFont extension from `.xml` to `.fnt` (thanks to Platonymous!).
* Fixed cases where an XNB file which couldn't be unpacked isn't copied into the export folder as-is.

## 1.0
Includes changes between 16 June 2019 and 25 April 2020, which didn't have packaged releases.

* Initial implementation with support for...
  * unpacking data (`.json`);
  * unpacking maps (`.tbin`);
  * unpacking textures (`.png`);
  * unpacking SpriteFont (`.png` and `.json`), and BMFont (`.png` and `.xml`) font files.
  * Linux/MacOS/Windows.
* Fixed MacOS build error (thanks to strobel1ght!).

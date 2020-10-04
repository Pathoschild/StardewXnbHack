[← back to readme](README.md)

# Release notes
## Upcoming release
* Added compiled release.
* Added icon/mascot (thanks to ParadigmNomad!).
* Added support for running it from the game folder or another app.
* Added file count and unpack time to log.
* Improved compatibility on Linux/MacOS.
* Changed map format from `.tbin` to `.tmx` (thanks to Platonymous!).
* Fixed error unpacking `SpriteFont` textures on Linux/MacOS.
* Fixed unsupported XNB files not always copied into the export folder.
* Fixed BMFont file extension set to `.xml` instead of `.fnt` (thanks to Platonymous!).

## Prerelease
Includes changes between 16 June 2019 and 25 April 2020, which didn't have packaged releases.

* Initial implementation with support for...
  * unpacking data (`.json`);
  * unpacking maps (`.tbin`);
  * unpacking textures (`.png`);
  * unpacking SpriteFont (`.png` and `.json`), and BMFont (`.png` and `.xml`) font files.
  * Linux/MacOS/Windows.
* Fixed MacOS build error (thanks to strobel1ght!).

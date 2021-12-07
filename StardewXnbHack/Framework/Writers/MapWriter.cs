using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using StardewModdingAPI.Toolkit.Utilities;
using StardewValley;
using TMXTile;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes <see cref="Map"/> assets to disk.</summary>
    internal class MapWriter : BaseAssetWriter
    {
        /*********
        ** Fields
        *********/
        /// <summary>The actual size of a tile in the tilesheet.</summary>
        const int TileSize = Game1.tileSize / Game1.pixelZoom;

        /// <summary>The underlying map format handler.</summary>
        private readonly TMXFormat Format;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public MapWriter()
        {
            // init TMX support
            this.Format = new TMXFormat(Game1.tileSize / Game1.pixelZoom, Game1.tileSize / Game1.pixelZoom, Game1.pixelZoom, Game1.pixelZoom);
        }

        /// <summary>Whether the writer can handle a given asset.</summary>
        /// <param name="asset">The asset value.</param>
        public override bool CanWrite(object asset)
        {
            return asset is Map;
        }

        /// <summary>Write an asset instance to disk.</summary>
        /// <param name="asset">The asset value.</param>
        /// <param name="toPathWithoutExtension">The absolute path to the export file, without the file extension.</param>
        /// <param name="relativePath">The relative path within the content folder.</param>
        /// <param name="platform">The operating system running the unpacker.</param>
        /// <param name="error">An error phrase indicating why writing to disk failed (if applicable).</param>
        /// <returns>Returns whether writing to disk completed successfully.</returns>
        public override bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, out string error)
        {
            Map map = (Map)asset;

            // fix tile sizes (game overrides them in-memory)
            IDictionary<Layer, Size> tileSizes = new Dictionary<Layer, Size>();
            foreach (var layer in map.Layers)
            {
                tileSizes[layer] = layer.TileSize;
                layer.TileSize = new Size(MapWriter.TileSize, MapWriter.TileSize);
            }

            // fix image sources (game overrides them in-memory)
            IDictionary<TileSheet, string> imageSources = new Dictionary<TileSheet, string>();
            foreach (var sheet in map.TileSheets)
            {
                imageSources[sheet] = sheet.ImageSource;
                sheet.ImageSource = this.GetOriginalImageSource(relativePath, sheet.ImageSource);
            }

            // save file
            using (Stream stream = new MemoryStream())
            {
                // serialize to stream
                this.Format.Store(map, stream, DataEncodingType.CSV);

                // workaround: TMXTile doesn't indent the XML in newer .NET versions
                stream.Position = 0;
                var doc = XDocument.Load(stream);
                File.WriteAllText($"{toPathWithoutExtension}.tmx", "<?xml version=\"1.0\"?>\n" + doc.ToString());
            }

            // undo changes
            foreach (var layer in map.Layers)
                layer.TileSize = tileSizes[layer];
            foreach (var sheet in map.TileSheets)
                sheet.ImageSource = imageSources[sheet];

            error = null;
            return true;
        }


        /*********
        ** Public methods
        *********/
        /// <summary>Get the image source for a map tilesheet without the game's automatic path changes.</summary>
        /// <param name="relativeMapPath">The relative path to the map file within the content folder.</param>
        /// <param name="imageSource">The tilesheet image source.</param>
        private string GetOriginalImageSource(string relativeMapPath, string imageSource)
        {
            string mapDirPath = PathUtilities.NormalizePath(Path.GetDirectoryName(relativeMapPath));
            string normalizedImageSource = PathUtilities.NormalizePath(imageSource);

            return normalizedImageSource.StartsWith($"{mapDirPath}{PathUtilities.PreferredPathSeparator}", StringComparison.OrdinalIgnoreCase)
                ? imageSource.Substring(mapDirPath.Length + 1)
                : imageSource;
        }
    }
}

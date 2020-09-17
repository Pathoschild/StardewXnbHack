using System.Collections.Generic;
using System.IO;
using StardewModdingAPI.Toolkit.Utilities;
using StardewValley;
using xTile;
using xTile.Dimensions;
using xTile.Layers;

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
        public static string[] MapFormatExtensions { get; set; } = new string[] { "tbin" };

        /*********
        ** Public methods
        *********/
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

            // save file
            foreach (string extension in MapFormatExtensions)
            {
                var format = xTile.Format.FormatManager.Instance.GetMapFormatByExtension(extension);

                using (Stream stream = File.Create($"{toPathWithoutExtension}.{extension}"))
                    format.Store(map, stream);

                // undo tile size changes
                foreach (var layer in map.Layers)
                    layer.TileSize = tileSizes[layer];
            }
            error = null;
            return true;
        }
    }
}

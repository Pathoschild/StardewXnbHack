using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Toolkit.Utilities;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes <see cref="Texture2D"/> assets to disk.</summary>
    internal class TextureWriter : BaseAssetWriter
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Whether the writer can handle a given asset.</summary>
        /// <param name="asset">The asset value.</param>
        public override bool CanWrite(object asset)
        {
            return asset is Texture2D;
        }

        /// <summary>Write an asset instance to disk.</summary>
        /// <param name="asset">The asset value.</param>
        /// <param name="toPathWithoutExtension">The absolute path to the export file, without the file extension.</param>
        /// <param name="relativePath">The relative path within the content folder.</param>
        /// <param name="platform">The operating system running the unpacker.</param>
        /// <param name="dataFormat">The expected output format for data files.</param>
        /// <param name="error">An error phrase indicating why writing to disk failed (if applicable).</param>
        /// <returns>Returns whether writing to disk completed successfully.</returns>
        public override bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, DataFormat dataFormat, out string error)
        {
            Texture2D texture = (Texture2D)asset;

            using (Stream stream = File.Create($"{toPathWithoutExtension}.png"))
                texture.SaveAsPng(stream, texture.Width, texture.Height);

            error = null;
            return true;
        }
    }
}

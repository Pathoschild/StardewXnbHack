using StardewModdingAPI.Toolkit.Utilities;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes assets assets to disk.</summary>
    public interface IAssetWriter
    {
        /*********
        ** Methods
        *********/
        /// <summary>Whether the writer can handle a given asset.</summary>
        /// <param name="asset">The asset value.</param>
        bool CanWrite(object asset);

        /// <summary>Write an asset instance to disk.</summary>
        /// <param name="asset">The asset value.</param>
        /// <param name="toPathWithoutExtension">The absolute path to the export file, without the file extension.</param>
        /// <param name="relativePath">The relative path within the content folder.</param>
        /// <param name="platform">The operating system running the unpacker.</param>
        /// <param name="error">An error phrase indicating why writing to disk failed (if applicable).</param>
        /// <returns>Returns whether writing to disk completed successfully.</returns>
        bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, out string error);
    }
}

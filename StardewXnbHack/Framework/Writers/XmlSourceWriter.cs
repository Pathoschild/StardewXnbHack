using System.IO;
using BmFont;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes <see cref="XmlSource"/> assets to disk.</summary>
    internal class XmlSourceWriter : IAssetWriter
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Whether the writer can handle a given asset.</summary>
        /// <param name="asset">The asset value.</param>
        public bool CanWrite(object asset)
        {
            return asset is XmlSource;
        }

        /// <summary>Write an asset instance to disk.</summary>
        /// <param name="asset">The asset value.</param>
        /// <param name="toPathWithoutExtension">The absolute path to the export file, without the file extension.</param>
        /// <param name="relativePath">The relative path within the content folder.</param>
        /// <param name="error">An error phrase indicating why writing to disk failed (if applicable).</param>
        /// <returns>Returns whether writing to disk completed successfully.</returns>
        public bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, out string error)
        {
            XmlSource value = (XmlSource)asset;
            File.WriteAllText($"{toPathWithoutExtension}.xml", value.Source);

            error = null;
            return true;
        }
    }
}

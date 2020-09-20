using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Toolkit.Utilities;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes <see cref="Texture2D"/> assets to disk.</summary>
    public class TextureWriter : BaseAssetWriter
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
        /// <param name="error">An error phrase indicating why writing to disk failed (if applicable).</param>
        /// <returns>Returns whether writing to disk completed successfully.</returns>
        public override bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, out string error)
        {
            Texture2D texture = (Texture2D)asset;

            this.UnpremultiplyTransparency(texture);
            using (Stream stream = File.Create($"{toPathWithoutExtension}.png"))
                texture.SaveAsPng(stream, texture.Width, texture.Height);

            error = null;
            return true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Reverse premultiplication applied to an image asset by the XNA content pipeline.</summary>
        /// <param name="texture">The texture to adjust.</param>
        private void UnpremultiplyTransparency(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                Color pixel = data[i];
                if (pixel.A == 0)
                    continue;

                data[i] = new Color(
                    (byte)((pixel.R * 255) / pixel.A),
                    (byte)((pixel.G * 255) / pixel.A),
                    (byte)((pixel.B * 255) / pixel.A),
                    pixel.A
                ); // don't use named parameters, which are inconsistent between MonoGame (e.g. 'alpha') and XNA (e.g. 'a')
            }

            texture.SetData(data);
        }
    }
}

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Toolkit.Utilities;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes <see cref="SpriteFont"/> assets to disk.</summary>
    internal class SpriteFontWriter : BaseAssetWriter
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Whether the writer can handle a given asset.</summary>
        /// <param name="asset">The asset value.</param>
        public override bool CanWrite(object asset)
        {
            return asset is SpriteFont;
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
            SpriteFont font = (SpriteFont)asset;

            // get texture
            Texture2D texture = font.Texture;

            // save texture
            using (Stream stream = File.Create($"{toPathWithoutExtension}.png"))
            {
                if (texture.Format == SurfaceFormat.Dxt3) // MonoGame can't read DXT3 textures directly, need to export through GPU
                {
                    using RenderTarget2D renderTarget = this.RenderWithGpu(texture);
                    renderTarget.SaveAsPng(stream, texture.Width, texture.Height);
                }
                else
                    texture.SaveAsPng(stream, texture.Width, texture.Height);
            }

            // save font data
            var data = new
            {
                font.LineSpacing,
                font.Spacing,
                font.DefaultCharacter,
                font.Characters,
                Glyphs = font.GetGlyphs()
            };
            File.WriteAllText($"{toPathWithoutExtension}.{this.GetDataExtension()}", this.FormatData(data));

            error = null;
            return true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Draw a texture to a GPU render target.</summary>
        /// <param name="texture">The texture to draw.</param>
        private RenderTarget2D RenderWithGpu(Texture2D texture)
        {
            // set render target
            var gpu = texture.GraphicsDevice;
            RenderTarget2D target = new RenderTarget2D(gpu, texture.Width, texture.Height);
            gpu.SetRenderTarget(target);

            // render
            try
            {
                gpu.Clear(Color.Transparent); // set transparent background

                using SpriteBatch batch = new SpriteBatch(gpu);
                batch.Begin();
                batch.Draw(texture, Vector2.Zero, Color.White);
                batch.End();
            }
            finally
            {
                gpu.SetRenderTarget(null);
            }

            return target;
        }
    }
}

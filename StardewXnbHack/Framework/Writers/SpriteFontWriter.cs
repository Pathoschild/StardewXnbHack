using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            Texture2D texture = platform == Platform.Windows
                ? this.RequireField<Texture2D>(font, "textureValue")
                : this.RequireProperty<Texture2D>(font, "Texture");

            // save texture
            using (Stream stream = File.Create($"{toPathWithoutExtension}.png"))
            {
                if (platform.IsMono() && texture.Format == SurfaceFormat.Dxt3) // MonoGame can't read DXT3 textures directly, need to export through GPU
                {
                    using (RenderTarget2D renderTarget = this.RenderWithGpu(texture))
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
                Glyphs = this.GetGlyphs(font, platform)
            };
            File.WriteAllText($"{toPathWithoutExtension}.{this.GetDataExtension()}", this.FormatData(data));

            error = null;
            return true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the font glyph data for a MonoGame font.</summary>
        /// <param name="font">The sprite font.</param>
        /// <param name="platform">The operating system running the unpacker.</param>
        private IDictionary<char, object> GetGlyphs(SpriteFont font, Platform platform)
        {
            IDictionary<char, object> glyphs = new Dictionary<char, object>();

            if (platform == Platform.Windows)
            {
                // get internal sprite data
                IList<Rectangle> glyphData = this.RequireField<List<Rectangle>>(font, "glyphData");
                IList<Rectangle> croppingData = this.RequireField<List<Rectangle>>(font, "croppingData");
                IList<Vector3> kerning = this.RequireField<List<Vector3>>(font, "kerning");

                // replicate MonoGame structure for consistency (and readability)
                for (int i = 0; i < font.Characters.Count; i++)
                {
                    char ch = font.Characters[i];
                    glyphs[ch] = new
                    {
                        BoundsInTexture = glyphData[i],
                        Cropping = croppingData[i],
                        Character = ch,

                        LeftSideBearing = kerning[i].X,
                        Width = kerning[i].Y,
                        RightSideBearing = kerning[i].Z,

                        WidthIncludingBearings = kerning[i].X + kerning[i].Y + kerning[i].Z
                    };
                }
            }
            else
            {
                foreach (DictionaryEntry entry in this.InvokeRequiredMethod<IDictionary>(font, "GetGlyphs")) // method is public in Mono, but need reflection so code compiles on Windows
                    glyphs[(char)entry.Key] = entry.Value;
            }

            return glyphs;
        }

        /// <summary>Get a required font field using reflection.</summary>
        /// <typeparam name="T">The field type.</typeparam>
        /// <param name="font">The font instance for which to get a value.</param>
        /// <param name="name">The field name.</param>
        private T RequireField<T>(SpriteFont font, string name)
        {
            FieldInfo field = typeof(SpriteFont).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null)
                throw new InvalidOperationException($"Can't access {nameof(SpriteFont)}.{name} field");

            return (T)field.GetValue(font);
        }

        /// <summary>Get a required font property using reflection.</summary>
        /// <typeparam name="T">The field type.</typeparam>
        /// <param name="font">The font instance for which to get a value.</param>
        /// <param name="name">The field name.</param>
        private T RequireProperty<T>(SpriteFont font, string name)
        {
            PropertyInfo property = typeof(SpriteFont).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property == null)
                throw new InvalidOperationException($"Can't access {nameof(SpriteFont)}.{name} property");

            return (T)property.GetValue(font);
        }

        /// <summary>Invoke a required font method using reflection.</summary>
        /// <typeparam name="TReturn">The return type.</typeparam>
        /// <param name="font">The font instance for which to get a method.</param>
        /// <param name="name">The method name.</param>
        private TReturn InvokeRequiredMethod<TReturn>(SpriteFont font, string name)
        {
            MethodInfo method = typeof(SpriteFont).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null)
                throw new InvalidOperationException($"Can't access {nameof(SpriteFont)}.{name} method");

            return (TReturn)method.Invoke(font, null);
        }

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

                using (SpriteBatch batch = new SpriteBatch(gpu))
                {
                    batch.Begin();
                    batch.Draw(texture, Vector2.Zero, Color.White);
                    batch.End();
                }
            }
            finally
            {
                gpu.SetRenderTarget(null);
            }

            return target;
        }
    }
}

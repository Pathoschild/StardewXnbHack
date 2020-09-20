using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Toolkit.Utilities;
using System.Collections;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes <see cref="SpriteFont"/> assets to disk.</summary>
    public class SpriteFontWriter : BaseAssetWriter
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

            // save texture
            Texture2D texture = this.RequireField<Texture2D>(font, platform == Platform.Windows ? "textureValue" : "_texture");
            using (Stream stream = File.Create($"{toPathWithoutExtension}.png"))
                texture.SaveAsPng(stream, texture.Width, texture.Height);

            // save font data
            var data = new
            {
                font.LineSpacing,
                font.Spacing,
                font.DefaultCharacter,
                font.Characters,
                Glyphs = this.GetGlyphs(font)
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
        private IDictionary<char, object> GetGlyphs(SpriteFont font)
        {
            Platform platform = EnvironmentUtility.DetectPlatform();
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
                //use Mono exclusive method
                foreach (DictionaryEntry entry in this.RequireMethod<IDictionary>(font, "GetGlyphs"))
                    glyphs.Add((char)entry.Key, entry.Value);
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

        /// <summary>Get a required font method using reflection.</summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="font">The font instance for which to get a methoh.</param>
        /// <param name="name">The field name.</param>
        private T RequireMethod<T>(SpriteFont font, string name)
        {
            MethodInfo method = typeof(SpriteFont).GetMethod("GetGlyphs", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null)
                throw new InvalidOperationException($"Can't access {nameof(SpriteFont)}.{name} method");

            return (T)method.Invoke(font, null);
        }
    }
}

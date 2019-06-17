using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>Writes <see cref="SpriteFont"/> assets to disk.</summary>
    internal class SpriteFontWriter : IAssetWriter
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Whether the writer can handle a given asset.</summary>
        /// <param name="asset">The asset value.</param>
        public bool CanWrite(object asset)
        {
            return asset is SpriteFont;
        }

        /// <summary>Write an asset instance to disk.</summary>
        /// <param name="asset">The asset value.</param>
        /// <param name="toPathWithoutExtension">The absolute path to the export file, without the file extension.</param>
        /// <param name="relativePath">The relative path within the content folder.</param>
        /// <param name="error">An error phrase indicating why writing to disk failed (if applicable).</param>
        /// <returns>Returns whether writing to disk completed successfully.</returns>
        public bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, out string error)
        {
            SpriteFont font = (SpriteFont)asset;

            // get texture
            Texture2D texture = (Texture2D)font.GetType().GetField("textureValue", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(font);
            if (texture == null)
            {
                error = "can't retrieve font texture field";
                return false;
            }

            // save texture
            using (Stream stream = File.Create($"{toPathWithoutExtension}.png"))
                texture.SaveAsPng(stream, texture.Width, texture.Height);

            // save font data
            File.WriteAllText($"{toPathWithoutExtension}.json", JsonConvert.SerializeObject(asset, this.GetSpriteFontSettings()));

            error = null;
            return true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the JSON settings to serialise a <see cref="SpriteFont"/> instance.</summary>
        private JsonSerializerSettings GetSpriteFontSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new SpriteFontContractResolver()
            };
        }


        /*********
        ** Private classes
        *********/
        /// <summary>A JSON contract resolver which adds applicable private fields when serialising a <see cref="SpriteFont"/> instance.</summary>
        public class SpriteFontContractResolver : DefaultContractResolver
        {
            /// <summary>Get the properties to serialise.</summary>
            /// <param name="type">The type to create properties for.</param>
            /// <param name="memberSerialization">The member serialization mode for the type.</param>
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

                foreach (string fieldName in new[] { "glyphData", "croppingData", "kerning" })
                {
                    FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var prop = base.CreateProperty(field, memberSerialization);
                    prop.Writable = true;
                    prop.Readable = true;
                    properties.Add(prop);
                }
                return properties;
            }
        }
    }
}

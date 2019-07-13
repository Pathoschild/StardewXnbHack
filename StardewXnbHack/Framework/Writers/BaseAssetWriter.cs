using System;
using Newtonsoft.Json;
using StardewModdingAPI.Toolkit.Utilities;
using YamlDotNet.Serialization;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>The base class for an asset writer.</summary>
    internal abstract class BaseAssetWriter : IAssetWriter
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Whether the writer can handle a given asset.</summary>
        /// <param name="asset">The asset value.</param>
        public abstract bool CanWrite(object asset);

        /// <summary>Write an asset instance to disk.</summary>
        /// <param name="asset">The asset value.</param>
        /// <param name="toPathWithoutExtension">The absolute path to the export file, without the file extension.</param>
        /// <param name="relativePath">The relative path within the content folder.</param>
        /// <param name="platform">The operating system running the unpacker.</param>
        /// <param name="dataFormat">The expected output format for data files.</param>
        /// <param name="error">An error phrase indicating why writing to disk failed (if applicable).</param>
        /// <returns>Returns whether writing to disk completed successfully.</returns>
        public abstract bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, DataFormat dataFormat, out string error);


        /*********
        ** Protected methods
        *********/
        /// <summary>Get a text representation for the given asset.</summary>
        /// <param name="asset">The asset to serialise.</param>
        /// <param name="format">The data format.</param>
        protected string FormatData(object asset, DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Json:
                    return JsonConvert.SerializeObject(asset, Formatting.Indented);

                case DataFormat.Yaml:
                    return new Serializer().Serialize(asset);

                default:
                    throw new NotSupportedException($"Unknown data format '{format}'");
            }
        }

        /// <summary>Get the recommended file extension for the given format.</summary>
        /// <param name="format">The data format.</param>
        protected string GetExtension(DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Json:
                    return "json";

                case DataFormat.Yaml:
                    return "yaml";

                default:
                    throw new NotSupportedException($"Unknown data format '{format}'");
            }
        }
    }
}

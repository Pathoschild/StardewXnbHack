using System.Collections.Generic;
using System.IO;
using StardewXnbHack.ProgressHandling;

namespace StardewXnbHack.Framework
{
    /// <summary>The context info for the current unpack run.</summary>
    internal class UnpackContext : IUnpackContext
    {
        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        public string GamePath { get; set; }

        /// <inheritdoc />
        public string ContentPath { get; set; }

        /// <inheritdoc />
        public string ExportPath { get; set; }

        /// <inheritdoc />
        public IEnumerable<FileInfo> Files { get; set; }
    }
}

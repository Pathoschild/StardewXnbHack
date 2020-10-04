using System.Collections.Generic;
using System.IO;

namespace StardewXnbHack.ProgressHandling
{
    /// <summary>The context info for the current unpack run.</summary>
    public interface IUnpackContext
    {
        /// <summary>The absolute path to the game folder.</summary>
        string GamePath { get; }

        /// <summary>The absolute path to the content folder.</summary>
        string ContentPath { get; }

        /// <summary>The absolute path to the folder containing exported assets.</summary>
        string ExportPath { get; }

        /// <summary>The files found to unpack.</summary>
        IEnumerable<FileInfo> Files { get; }
    }
}

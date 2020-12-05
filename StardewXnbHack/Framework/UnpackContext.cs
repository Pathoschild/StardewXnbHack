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


        /*********
        ** Public methods
        *********/
        /// <summary>Try to detect the <see cref="GamePath"/> and <see cref="ContentPath"/> values.</summary>
        /// <param name="specifiedGamePath">The game path specified by the user, if any.</param>
        /// <param name="platform">The platform context.</param>
        public void TryDetectGamePaths(string specifiedGamePath, PlatformContext platform)
        {
            this.GamePath = specifiedGamePath ?? platform.DetectGameFolder();

            string relativeContentPath = platform.GetRelativeContentPath(this.GamePath);
            if (relativeContentPath != null)
                this.ContentPath = Path.Combine(this.GamePath, relativeContentPath);
        }
    }
}

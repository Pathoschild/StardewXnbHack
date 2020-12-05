using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewModdingAPI.Toolkit;
using StardewModdingAPI.Toolkit.Utilities;

namespace StardewXnbHack.Framework
{
    /// <summary>Provides platform-specific information.</summary>
    internal class PlatformContext
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The current platform.</summary>
        public Platform Platform { get; } = EnvironmentUtility.DetectPlatform();


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether any of the listed platforms is the current one.</summary>
        /// <param name="platforms">The platforms to match.</param>
        public bool Is(params Platform[] platforms)
        {
            return platforms.Contains(this.Platform);
        }

        /// <summary>Detect the game folder, if any.</summary>
        public string DetectGameFolder()
        {
            string assemblyDirPath = AppDomain.CurrentDomain.BaseDirectory;

            return this.IsGameFolder(assemblyDirPath)
                ? assemblyDirPath
                : new ModToolkit().GetGameFolders().FirstOrDefault()?.FullName;
        }

        /// <summary>Get the relative path from the game folder to the content folder, if any.</summary>
        /// <param name="gamePath">The game path.</param>
        public string GetRelativeContentPath(string gamePath)
        {
            if (gamePath == null)
                return null;

            foreach (string relativePath in this.GetPossibleRelativeContentPaths())
            {
                if (Directory.Exists(Path.Combine(gamePath, relativePath)))
                    return relativePath;
            }

            return null;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether a folder contains the game files.</summary>
        /// <param name="path">The absolute folder path to check.</param>
        private bool IsGameFolder(string path)
        {
            return
                File.Exists(Path.Combine(path, this.GetExecutableFileName()))
                && this.GetRelativeContentPath(path) != null;
        }

        /// <summary>Get the filename for the Stardew Valley executable.</summary>
        private string GetExecutableFileName()
        {
            return this.Platform == Platform.Windows
                ? "Stardew Valley.exe"
                : "StardewValley.exe";
        }

        /// <summary>Get the possible relative paths for the current platform.</summary>
        private IEnumerable<string> GetPossibleRelativeContentPaths()
        {
            // under game folder on most platforms
            if (this.Platform != Platform.Mac)
                yield return "Content";

            // MacOS
            else
            {
                // Steam paths
                // - game path: StardewValley/Contents/MacOS
                // - content:   StardewValley/Contents/Resources/Content
                yield return "../Resources/Content";

                // GOG paths
                // - game path: Stardew Valley.app/Contents/MacOS
                // - content:   Stardew Valley.app/Resources/Content
                yield return "../../Resources/Content";
            }
        }
    }
}

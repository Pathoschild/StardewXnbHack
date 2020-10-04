using System;
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

        /// <summary>Get the relative path from the game folder to the content folder.</summary>
        public string GetRelativeContentPath()
        {
            return this.Platform == Platform.Mac
                ? "../../Resources/Content"
                : "Content";
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
                && Directory.Exists(Path.Combine(path, this.GetRelativeContentPath()));
        }

        /// <summary>Get the filename for the Stardew Valley executable.</summary>
        private string GetExecutableFileName()
        {
            return this.Platform == Platform.Windows
                ? "Stardew Valley.exe"
                : "StardewValley.exe";
        }
    }
}

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

        /// <summary>Get the absolute paths to the game and content folders, if found.</summary>
        /// <param name="specifiedPath">The game path specified by the user, if any.</param>
        /// <param name="gamePath">The absolute path to the game folder, if found.</param>
        /// <param name="contentPath">The absolute path to the content folder, if found.</param>
        /// <returns>Returns whether both the game and content folders were found.</returns>
        public bool TryDetectGamePaths(string specifiedPath, out string gamePath, out string contentPath)
        {
            gamePath = null;
            contentPath = null;

            // check possible game paths
            foreach (string candidate in this.GetCandidateGamePaths(specifiedPath))
            {
                // detect paths
                string curGamePath = this.TryGamePath(candidate);
                string curContentPath = this.FindContentPath(curGamePath);

                // valid game install found
                if (curGamePath != null && curContentPath != null)
                {
                    gamePath = curGamePath;
                    contentPath = curContentPath;
                    return true;
                }

                // if game folder exists without a content folder, track the first found game path (i.e. the highest-priority one)
                gamePath ??= curGamePath;
            }

            return false;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the possible game paths.</summary>
        /// <param name="specifiedPath">The game path specified by the user, if any.</param>
        private IEnumerable<string> GetCandidateGamePaths(string specifiedPath = null)
        {
            // specified path
            if (!string.IsNullOrWhiteSpace(specifiedPath))
                yield return specifiedPath;

            // current working directory
            yield return AppDomain.CurrentDomain.BaseDirectory;

            // detected game path
            string detectedPath = new ModToolkit().GetGameFolders().FirstOrDefault()?.FullName;
            if (detectedPath != null)
                yield return detectedPath;
        }

        /// <summary>Get the absolute path to the game folder, if it's valid.</summary>
        /// <param name="path">The path to check for a game install.</param>
        private string TryGamePath(string path)
        {
            // game path exists
            if (path == null)
                return null;
            DirectoryInfo gameDir = new DirectoryInfo(path);
            if (!gameDir.Exists)
                return null;

            // has game files
            bool hasGameDll = File.Exists(Path.Combine(gameDir.FullName, "Stardew Valley.dll"));
            if (!hasGameDll)
                return null;

            // isn't the build folder when compiled directly
            bool isCompileFolder = File.Exists(Path.Combine(gameDir.FullName, "StardewXnbHack.exe.config"));
            if (isCompileFolder)
                return null;

            return gameDir.FullName;
        }

        /// <summary>Get the absolute path to the content folder for a given game, if found.</summary>
        /// <param name="gamePath">The absolute path to the game folder.</param>
        private string FindContentPath(string gamePath)
        {
            if (gamePath == null)
                return null;

            foreach (string relativePath in this.GetPossibleRelativeContentPaths())
            {
                DirectoryInfo folder = new DirectoryInfo(Path.Combine(gamePath, relativePath));
                if (folder.Exists)
                    return folder.FullName;
            }

            return null;
        }

        /// <summary>Get the possible relative paths for the current platform.</summary>
        private IEnumerable<string> GetPossibleRelativeContentPaths()
        {
            // under game folder on most platforms
            if (this.Platform != Platform.Mac)
                yield return "Content";

            // macOS
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

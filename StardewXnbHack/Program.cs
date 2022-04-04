using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Toolkit.Utilities;
using StardewValley;
using StardewXnbHack.Framework;
using StardewXnbHack.Framework.Writers;
using StardewXnbHack.ProgressHandling;

namespace StardewXnbHack
{
    /// <summary>The console app entry point.</summary>
    public static class Program
    {
        /*********
        ** Fields
        *********/
        /// <summary>The relative paths to search for unresolved assembly files.</summary>
        private static readonly string[] RelativeAssemblyProbePaths =
        {
            "", // app directory
            "smapi-internal"
        };


        /*********
        ** Public methods
        *********/
        /// <summary>The console app entry method.</summary>
        internal static void Main()
        {
            // set window title
            Console.Title = $"StardewXnbHack {Program.GetUnpackerVersion()}";

            // check platform
            Program.AssertPlatform();

            // Add fallback assembly resolution that loads DLLs from a 'smapi-internal' subfolder,
            // so it can be run from the game folder. This must be set before any references to
            // game or toolkit types (including IAssetWriter which references the toolkit's
            // Platform enum).
            AppDomain.CurrentDomain.AssemblyResolve += Program.CurrentDomain_AssemblyResolve;

            // launch app
            try
            {
                Program.Run();
            }
            catch (Exception ex)
            {
                // not in game folder
                if (ex is FileNotFoundException fileNotFoundEx)
                {
                    AssemblyName assemblyName = new AssemblyName(fileNotFoundEx.FileName);
                    if (assemblyName.Name == "Stardew Valley")
                    {
                        Console.WriteLine("Oops! StardewXnbHack must be placed in the Stardew Valley game folder.\nSee instructions: https://github.com/Pathoschild/StardewXnbHack#readme.");
                        DefaultConsoleLogger.PressAnyKeyToExit();
                        return;
                    }
                }

                // generic unhandled exception
                Console.WriteLine("Oops! Something went wrong running the unpacker:");
                Console.WriteLine(ex.ToString());
                DefaultConsoleLogger.PressAnyKeyToExit();
            }
        }

        /// <summary>Unpack all assets in the content folder and store them in the output folder.</summary>
        /// <param name="game">The game instance through which to unpack files, or <c>null</c> to launch a temporary internal instance.</param>
        /// <param name="gamePath">The absolute path to the game folder, or <c>null</c> to auto-detect it.</param>
        /// <param name="getLogger">Get a custom progress update logger, or <c>null</c> to use the default console logging. Receives the unpack context and default logger as arguments.</param>
        /// <param name="showPressAnyKeyToExit">Whether the default logger should show a 'press any key to exit' prompt when it finishes.</param>
        public static void Run(GameRunner game = null, string gamePath = null, Func<IUnpackContext, IProgressLogger, IProgressLogger> getLogger = null, bool showPressAnyKeyToExit = true)
        {
            // init logging
            UnpackContext context = new UnpackContext();
            IProgressLogger logger = new DefaultConsoleLogger(context, showPressAnyKeyToExit);
            logger.OnStepChanged(ProgressStep.Started, $"Running StardewXnbHack {Program.GetUnpackerVersion()}.");

            try
            {
                // get override logger
                if (getLogger != null)
                    logger = getLogger(context, logger);

                // start timer
                Stopwatch timer = new Stopwatch();
                timer.Start();

                // get asset writers
                var assetWriters = new IAssetWriter[]
                {
                    new MapWriter(),
                    new SpriteFontWriter(),
                    new TextureWriter(),
                    new XmlSourceWriter(),
                    new DataWriter() // check last due to more expensive CanWrite
                };

                // get paths
                var platform = new PlatformContext();
                {
                    if (platform.TryDetectGamePaths(gamePath, out gamePath, out string contentPath))
                    {
                        context.GamePath = gamePath;
                        context.ContentPath = contentPath;
                    }
                    else
                    {
                        logger.OnFatalError(gamePath == null
                            ? "Can't find the Stardew Valley folder. Try running StardewXnbHack from the game folder instead."
                            : $"Can't find the content folder for the game at {gamePath}. Is the game installed correctly?"
                        );
                        return;
                    }
                }
                context.ExportPath = Path.Combine(context.GamePath, "Content (unpacked)");
                logger.OnStepChanged(ProgressStep.GameFound, $"Found game folder: {context.GamePath}.");

                // symlink files on Linux/Mac
                if (platform.Is(Platform.Linux, Platform.Mac))
                {
                    foreach (string dirName in new[] { "lib", "lib64" })
                    {
                        string fullPath = Path.Combine(context.GamePath, dirName);
                        if (!Directory.Exists(dirName))
                            Process.Start("ln", $"-sf \"{fullPath}\"");
                    }
                }

                // load game instance
                bool disposeGame = false;
                if (game == null)
                {
                    logger.OnStepChanged(ProgressStep.LoadingGameInstance, "Loading game instance...");
                    game = Program.CreateTemporaryGameInstance(platform, context.ContentPath);
                    disposeGame = true;
                }

                // unpack files
                try
                {
                    logger.OnStepChanged(ProgressStep.UnpackingFiles, $"Unpacking files for Stardew Valley {Program.GetGameVersion()}...");

                    // collect files
                    DirectoryInfo contentDir = new DirectoryInfo(context.ContentPath);
                    FileInfo[] files = contentDir.EnumerateFiles("*.xnb", SearchOption.AllDirectories).ToArray();
                    context.Files = files;

                    // write assets
                    foreach (FileInfo file in files)
                    {
                        // prepare paths
                        string assetName = file.FullName.Substring(context.ContentPath.Length + 1, file.FullName.Length - context.ContentPath.Length - 5); // remove root path + .xnb extension
                        string relativePath = $"{assetName}.xnb";
                        string fileExportPath = Path.Combine(context.ExportPath, assetName);
                        Directory.CreateDirectory(Path.GetDirectoryName(fileExportPath));

                        // fallback logic
                        void ExportRawXnb()
                        {
                            File.Copy(file.FullName, $"{fileExportPath}.xnb", overwrite: true);
                        }

                        // show progress bar
                        logger.OnFileUnpacking(assetName);

                        // read asset
                        object asset;
                        try
                        {
                            asset = game.Content.Load<object>(assetName);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "This does not appear to be a MonoGame MGFX file!")
                                logger.OnFileUnpackFailed(relativePath, UnpackFailedReason.UnsupportedFileType, $"{nameof(Effect)} isn't a supported asset type.");
                            else
                                logger.OnFileUnpackFailed(relativePath, UnpackFailedReason.ReadError, $"read error: {ex.Message}");
                            ExportRawXnb();
                            continue;
                        }

                        // write asset
                        try
                        {
                            // get writer
                            IAssetWriter writer = assetWriters.FirstOrDefault(p => p.CanWrite(asset));

                            // write file
                            if (writer == null)
                            {
                                logger.OnFileUnpackFailed(relativePath, UnpackFailedReason.UnsupportedFileType, $"{asset.GetType().Name} isn't a supported asset type.");
                                ExportRawXnb();
                            }
                            else if (!writer.TryWriteFile(asset, fileExportPath, assetName, platform.Platform, out string writeError))
                            {
                                logger.OnFileUnpackFailed(relativePath, UnpackFailedReason.WriteError, $"{asset.GetType().Name} file could not be saved: {writeError}.");
                                ExportRawXnb();
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.OnFileUnpackFailed(relativePath, UnpackFailedReason.UnknownError, $"unhandled export error: {ex.Message}");
                        }
                        finally
                        {
                            game.Content.Unload();
                        }
                    }
                }
                finally
                {
                    if (disposeGame)
                        game.Dispose();
                }

                logger.OnStepChanged(ProgressStep.Done, $"Done! Unpacked {context.Files.Count()} files in {Program.GetHumanTime(timer.Elapsed)}.\nUnpacked into {context.ExportPath}.");
            }
            catch (Exception ex)
            {
                logger.OnFatalError($"Unhandled exception: {ex}");
            }
            finally
            {
                logger.OnEnded();
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Assert that the current platform matches the one StardewXnbHack was compiled for.</summary>
        private static void AssertPlatform()
        {
            bool isWindows = Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix;

#if IS_FOR_WINDOWS
            if (!isWindows)
            {
                Console.WriteLine("Oops! This is the Windows version of StardewXnbHack. Make sure to install the Windows version instead.");
                DefaultConsoleLogger.PressAnyKeyToExit();
            }
#else
            if (isWindows)
            {
                Console.WriteLine("Oops! This is the Linux/macOS version of StardewXnbHack. Make sure to install the version for your OS type instead.");
                DefaultConsoleLogger.PressAnyKeyToExit();
            }
#endif
        }

        /// <summary>Method called when assembly resolution fails, which may return a manually resolved assembly.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            // get assembly name
            AssemblyName name = new AssemblyName(e.Name);
            if (name.Name == null)
                return null;

            // check search folders
            foreach (string relativePath in Program.RelativeAssemblyProbePaths)
            {
                // get absolute path of search folder
                string searchPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), relativePath);
                if (!Directory.Exists(searchPath))
                    continue;

                // try to resolve DLL
                try
                {
                    foreach (FileInfo dll in new DirectoryInfo(searchPath).EnumerateFiles("*.dll"))
                    {
                        // get assembly name
                        string dllAssemblyName;
                        try
                        {
                            dllAssemblyName = AssemblyName.GetAssemblyName(dll.FullName).Name;
                        }
                        catch
                        {
                            continue;
                        }

                        // check for match
                        if (name.Name.Equals(dllAssemblyName, StringComparison.OrdinalIgnoreCase))
                            return Assembly.LoadFrom(dll.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error resolving assembly: {ex}");
                    return null;
                }
            }

            return null;
        }

        /// <summary>Create a temporary game instance for the unpacker.</summary>
        /// <param name="platform">The platform-specific context.</param>
        /// <param name="contentPath">The absolute path to the content folder to import.</param>
        private static GameRunner CreateTemporaryGameInstance(PlatformContext platform, string contentPath)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;

            try
            {
                GameRunner game = new GameRunner();
                GameRunner.instance = game;

                Game1.graphics.GraphicsProfile = GraphicsProfile.HiDef;

                game.RunOneFrame();

                return game;
            }
            finally
            {
                Console.ForegroundColor = foregroundColor;
                Console.WriteLine();
            }
        }

        /// <summary>Get a human-readable representation of elapsed time.</summary>
        /// <param name="time">The elapsed time.</param>
        private static string GetHumanTime(TimeSpan time)
        {
            List<string> parts = new List<string>(2);

            if (time.TotalMinutes >= 1)
                parts.Add($"{time.TotalMinutes:0} minute{(time.TotalMinutes >= 2 ? "s" : "")}");
            if (time.Seconds > 0)
                parts.Add($"{time.Seconds:0} second{(time.Seconds > 1 ? "s" : "")}");

            return string.Join(" ", parts);
        }

        /// <summary>Get the version string for StardewXnbHack.</summary>
        private static string GetUnpackerVersion()
        {
            var attribute = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attribute?.InformationalVersion ?? "<unknown version>";
        }

        /// <summary>Get the version string for Stardew Valley.</summary>
        private static string GetGameVersion()
        {
            StringBuilder version = new();

            // base version
            version.Append(Game1.version);
            version.Append(" (");

            // build number
            {
                string buildVersion = typeof(Game1).Assembly.GetName().Version?.ToString() ?? "unknown";
                if (buildVersion.StartsWith($"{Game1.version}."))
                    buildVersion = buildVersion.Substring(Game1.version.Length + 1);
                version.Append(buildVersion);
            }

            // version label
            if (!string.IsNullOrWhiteSpace(Game1.versionLabel))
            {
                version.Append(" \"");
                version.Append(Game1.versionLabel);
                version.Append('"');
            }

            version.Append(')');

            return version.ToString();
        }
    }
}

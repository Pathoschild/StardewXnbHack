using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using StardewModdingAPI.Toolkit;
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
        /// <summary>The asset writers which support saving data to disk.</summary>
        private static readonly IAssetWriter[] AssetWriters = {
            new MapWriter(),
            new SpriteFontWriter(),
            new TextureWriter(),
            new XmlSourceWriter(),
            new DataWriter() // check last due to more expensive CanWrite
        };


        /*********
        ** Public methods
        *********/
        /// <summary>The console app entry method.</summary>
        internal static void Main()
        {
            Program.Run();
        }

        /// <summary>Unpack all assets in the content folder and store them in the output folder.</summary>
        /// <param name="game">The game instance through which to unpack files, or <c>null</c> to launch a temporary internal instance.</param>
        /// <param name="gamePath">The absolute path to the game folder, or <c>null</c> to auto-detect it.</param>
        /// <param name="getLogger">Get a custom progress update logger, or <c>null</c> to use the default console logging. Receives the unpack context and default logger as arguments.</param>
        /// <param name="pressAnyKeyToExit">Whether the default logger shows a 'press any key to exit' prompt.</param>
        public static void Run(Game1 game = null, string gamePath = null, Func<IUnpackContext, IProgressLogger, IProgressLogger> getLogger = null, bool pressAnyKeyToExit = true)
        {
            // init logging
            UnpackContext context = new UnpackContext();
            IProgressLogger logger = new DefaultConsoleLogger(context, pressAnyKeyToExit);

            try
            {
                // get override logger
                if (getLogger != null)
                    logger = getLogger(context, logger);

                // start timer
                Stopwatch timer = new Stopwatch();
                timer.Start();

                // get game info
                Platform platform = EnvironmentUtility.DetectPlatform();
                context.GamePath = gamePath ?? new ModToolkit().GetGameFolders().FirstOrDefault()?.FullName;
                if (context.GamePath == null || !Directory.Exists(context.GamePath))
                {
                    logger.OnFatalError("Can't find Stardew Valley folder.");
                    return;
                }

                // get import/export paths
                context.ContentPath = Path.Combine(context.GamePath, "Content");
                context.ExportPath = Path.Combine(context.GamePath, "Content (unpacked)");
                logger.OnStepChanged(ProgressStep.GameFound, $"Found game folder: {context.GamePath}.");

                // symlink files on Linux/Mac
                if (platform == Platform.Linux || platform == Platform.Mac)
                {
                    foreach (string dirName in new[] { "Content", "lib", "lib64" })
                    {
                        string fullPath = Path.Combine(context.GamePath, dirName);
                        if (!Directory.Exists(fullPath))
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
                    logger.OnStepChanged(ProgressStep.UnpackingFiles, "Unpacking files...");

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
                        object asset = null;
                        try
                        {
                            asset = game.Content.Load<object>(assetName);
                        }
                        catch (Exception ex)
                        {
                            logger.OnFileUnpackFailed(relativePath, UnpackFailedReason.ReadError, $"read error: {ex.Message}");
                            ExportRawXnb();
                            continue;
                        }

                        // write asset
                        try
                        {
                            // get writer
                            IAssetWriter writer = Program.AssetWriters.FirstOrDefault(p => p.CanWrite(asset));

                            // write file
                            if (writer == null)
                            {
                                logger.OnFileUnpackFailed(relativePath, UnpackFailedReason.UnsupportedFileType, $"{asset.GetType().Name} isn't a supported asset type.");
                                ExportRawXnb();
                            }
                            else if (!writer.TryWriteFile(asset, fileExportPath, assetName, platform, out string writeError))
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
        /// <summary>Create a temporary game instance for the unpacker.</summary>
        /// <param name="platform">The OS running the unpacker.</param>
        /// <param name="contentPath">The path to the content folder to import.</param>
        private static Game1 CreateTemporaryGameInstance(Platform platform, string contentPath)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;

            try
            {

                Game1 game = new Game1();

                if (platform == Platform.Windows)
                {
                    game.Content.RootDirectory = contentPath;
                    MethodInfo startGameLoop = typeof(Game1).GetMethod("StartGameLoop", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    if (startGameLoop == null)
                        throw new InvalidOperationException("Can't locate game method 'StartGameLoop' to initialise internal game instance.");
                    startGameLoop.Invoke(game, new object[0]);
                }
                else
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
    }
}

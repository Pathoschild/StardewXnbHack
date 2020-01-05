using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using StardewModdingAPI.Toolkit;
using StardewModdingAPI.Toolkit.Utilities;
using StardewValley;
using StardewXnbHack.Framework;
using StardewXnbHack.Framework.Writers;

namespace StardewXnbHack
{
    /// <summary>The console app entry point.</summary>
    internal class Program
    {
        /*********
        ** Fields
        *********/
        /// <summary>The asset writers which support saving data to disk.</summary>
        private readonly IAssetWriter[] AssetWriters = {
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
        /// <param name="args">The arguments received by the app.</param>
        static void Main(string[] args)
        {
            new Program().Run();
        }

        /// <summary>Unpack all assets in the content folder and store them in the output folder.</summary>
        public void Run()
        {
            // get game info
            Platform platform = EnvironmentUtility.DetectPlatform();
            string gamePath = new ModToolkit().GetGameFolders().FirstOrDefault()?.FullName;
            if (gamePath == null)
            {
                this.PrintColor("Can't find Stardew Valley folder.", ConsoleColor.Red);
                return;
            }
            Console.WriteLine($"Found game folder: {gamePath}.");
            Console.WriteLine();

            // get import/export paths
            string contentPath = Path.Combine(gamePath, "Content");
            string exportPath = Path.Combine(gamePath, "Content (unpacked)");

            // symlink files on Linux/Mac
            if (platform == Platform.Linux || platform == Platform.Mac)
            {
                Process.Start("ln", $"-sf \"{Path.Combine(gamePath, "Content")}\"");
                Process.Start("ln", $"-sf \"{Path.Combine(gamePath, "lib")}\"");
                Process.Start("ln", $"-sf \"{Path.Combine(gamePath, "lib64")}\"");
            }

            // load game
            ConsoleProgressBar progressBar;
            Console.WriteLine("Loading game instance...");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            using (Game1 game = this.GetGameInstance(platform, contentPath))
            {
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Unpacking files...");

                // collect files
                DirectoryInfo contentDir = new DirectoryInfo(contentPath);
                FileInfo[] files = contentDir.EnumerateFiles("*.xnb", SearchOption.AllDirectories).ToArray();
                progressBar = new ConsoleProgressBar(files.Length);

                // write assets
                foreach (FileInfo file in files)
                {
                    // prepare paths
                    string assetName = file.FullName.Substring(contentPath.Length + 1, file.FullName.Length - contentPath.Length - 5); // remove root path + .xnb extension
                    string fileExportPath = Path.Combine(exportPath, assetName);
                    Directory.CreateDirectory(Path.GetDirectoryName(fileExportPath));

                    // show progress bar
                    progressBar.Increment();
                    progressBar.Print(assetName);

                    // read asset
                    object asset = null;
                    try
                    {
                        asset = game.Content.Load<object>(assetName);
                    }
                    catch (Exception ex)
                    {
                        progressBar.Erase();
                        this.PrintColor($"{assetName} => read error: {ex.Message}", ConsoleColor.Red);
                        continue;
                    }

                    // write asset
                    try
                    {
                        // get writer
                        IAssetWriter writer = this.AssetWriters.FirstOrDefault(p => p.CanWrite(asset));

                        // write file
                        if (writer == null)
                        {
                            progressBar.Erase();
                            this.PrintColor($"{assetName}.xnb ({asset.GetType().Name}) isn't a supported asset type.", ConsoleColor.DarkYellow);
                            File.Copy(file.FullName, $"{fileExportPath}.xnb", overwrite: true);
                        }
                        else if (!writer.TryWriteFile(asset, fileExportPath, assetName, platform, out string writeError))
                        {
                            progressBar.Erase();
                            this.PrintColor($"{assetName}.xnb ({asset.GetType().Name}) could not be saved: {writeError}.", ConsoleColor.DarkYellow);
                            File.Copy(file.FullName, $"{fileExportPath}.xnb", overwrite: true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{assetName} => export error: {ex.Message}");
                        Console.ResetColor();
                    }
                    finally
                    {
                        game.Content.Unload();
                    }
                }
            }

            progressBar.Erase();
            Console.WriteLine($"Done! Unpacked files to {exportPath}.");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Print a message to the console with a foreground color.</summary>
        /// <param name="message">The message to print.</param>
        /// <param name="color">The foreground color to use.</param>
        private void PrintColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>Get an initialised instance of Stardew Valley.</summary>
        /// <param name="platform">The OS running the unpacker.</param>
        /// <param name="contentPath">The path to the content folder to import.</param>
        private Game1 GetGameInstance(Platform platform, string contentPath)
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
    }
}

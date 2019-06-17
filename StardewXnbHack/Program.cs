using System;
using System.IO;
using System.Linq;
using System.Reflection;
using StardewModdingAPI.Toolkit;
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
        /// <summary>The folder containing the executable file.</summary>
        private string GamePath;

        /// <summary>The absolute path to the folder containing XNB files to unpack.</summary>
        private string ContentPath => Path.Combine(this.GamePath, "Content");

        /// <summary>The absolute path to the folder in which to save unpacked files.</summary>
        private string ExportPath => Path.Combine(this.GamePath, "Content (unpacked)");

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
        static void Main()
        {
            new Program().Run();
        }

        /// <summary>Unpack all assets in the content folder and store them in the output folder.</summary>
        public void Run()
        {
            // find game folder
            ModToolkit toolkit = new ModToolkit();
            this.GamePath = toolkit.GetGameFolders().FirstOrDefault()?.FullName;
            if (this.GamePath == null)
            {
                this.PrintColor("Can't find Stardew Valley folder.", ConsoleColor.Red);
                return;
            }
            Console.WriteLine($"Found game folder: {this.GamePath}.");
            Console.WriteLine();

            // load game
            ConsoleProgressBar progressBar;
            Console.WriteLine("Loading game instance...");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            using (Game1 game = this.GetGameInstance())
            {
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Unpacking files...");

                // collect files
                DirectoryInfo contentDir = new DirectoryInfo(this.ContentPath);
                FileInfo[] files = contentDir.EnumerateFiles("*.xnb", SearchOption.AllDirectories).ToArray();
                progressBar = new ConsoleProgressBar(files.Length);

                // write assets
                foreach (FileInfo file in files)
                {
                    // prepare paths
                    string assetName = file.FullName.Substring(this.ContentPath.Length + 1, file.FullName.Length - this.ContentPath.Length - 5); // remove root path + .xnb extension
                    string exportPath = Path.Combine(this.ExportPath, assetName);
                    Directory.CreateDirectory(Path.GetDirectoryName(exportPath));

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
                            File.Copy(file.FullName, $"{exportPath}.xnb", overwrite: true);
                        }
                        else if (!writer.TryWriteFile(asset, exportPath, assetName, out string writeError))
                        {
                            progressBar.Erase();
                            this.PrintColor($"{assetName}.xnb ({asset.GetType().Name}) could not be saved: {writeError}.", ConsoleColor.DarkYellow);
                            File.Copy(file.FullName, $"{exportPath}.xnb", overwrite: true);
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
            Console.WriteLine($"Done! Unpacked files to {this.ExportPath}.");
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
        private Game1 GetGameInstance()
        {
            Game1 game = new Game1();
            game.Content.RootDirectory = this.ContentPath;
            MethodInfo startGameLoop = typeof(Game1).GetMethod("StartGameLoop", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (startGameLoop == null)
                throw new InvalidOperationException("Can't locate game method 'StartGameLoop' to initialise internal game instance.");
            startGameLoop.Invoke(game, new object[0]);
            return game;
        }
    }
}

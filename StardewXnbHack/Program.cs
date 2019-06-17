using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
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
            this.GamePath = this.GetInstallPaths().FirstOrDefault(Directory.Exists);
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

        /// <summary>The default file paths where Stardew Valley can be installed.</summary>
        /// <remarks>Derived from the SMAPI installer.</remarks>
        private IEnumerable<string> GetInstallPaths()
        {
            // default paths
            foreach (string programFiles in new[] { @"C:\Program Files", @"C:\Program Files (x86)" })
            {
                yield return $@"{programFiles}\GalaxyClient\Games\Stardew Valley";
                yield return $@"{programFiles}\GOG Galaxy\Games\Stardew Valley";
                yield return $@"{programFiles}\Steam\steamapps\common\Stardew Valley";
            }

            // Windows registry
            IDictionary<string, string> registryKeys = new Dictionary<string, string>
            {
                [@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 413150"] = "InstallLocation", // Steam
                [@"SOFTWARE\WOW6432Node\GOG.com\Games\1453375253"] = "PATH", // GOG on 64-bit Windows
            };
            foreach (var pair in registryKeys)
            {
                string path = this.GetLocalMachineRegistryValue(pair.Key, pair.Value);
                if (!string.IsNullOrWhiteSpace(path))
                    yield return path;
            }

            // via Steam library path
            string steamPath = this.GetCurrentUserRegistryValue(@"Software\Valve\Steam", "SteamPath");
            if (steamPath != null)
                yield return Path.Combine(steamPath.Replace('/', '\\'), @"steamapps\common\Stardew Valley");
        }

        /// <summary>Get the value of a key in the Windows HKLM registry.</summary>
        /// <param name="key">The full path of the registry key relative to HKLM.</param>
        /// <param name="name">The name of the value.</param>
        private string GetLocalMachineRegistryValue(string key, string name)
        {
            RegistryKey localMachine = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : Registry.LocalMachine;
            RegistryKey openKey = localMachine.OpenSubKey(key);
            if (openKey == null)
                return null;
            using (openKey)
                return (string)openKey.GetValue(name);
        }

        /// <summary>Get the value of a key in the Windows HKCU registry.</summary>
        /// <param name="key">The full path of the registry key relative to HKCU.</param>
        /// <param name="name">The name of the value.</param>
        private string GetCurrentUserRegistryValue(string key, string name)
        {
            RegistryKey currentuser = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64) : Registry.CurrentUser;
            RegistryKey openKey = currentuser.OpenSubKey(key);
            if (openKey == null)
                return null;
            using (openKey)
                return (string)openKey.GetValue(name);
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

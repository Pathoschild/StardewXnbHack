using System;
using System.Linq;
using StardewXnbHack.ProgressHandling;

namespace StardewXnbHack.Framework
{
    /// <summary>Report updates to the console while the unpacker is running.</summary>
    internal class DefaultConsoleLogger : IProgressLogger
    {
        /*********
        ** Fields
        *********/
        /// <summary>The context info for the current unpack run.</summary>
        private readonly IUnpackContext Context;

        /// <summary>The current progress bar written to the console.</summary>
        private ConsoleProgressBar ProgressBar;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="context">The context info for the current unpack run.</param>
        public DefaultConsoleLogger(IUnpackContext context)
        {
            this.Context = context;
        }

        /// <inheritdoc />
        public void OnStartError(string error)
        {
            this.PrintColor(error, ConsoleColor.Red);
        }

        /// <inheritdoc />
        public void OnStepChanged(ProgressStep step, string message)
        {
            this.ProgressBar?.Erase();
            Console.WriteLine(message);
        }

        /// <inheritdoc />
        public void OnFileUnpacking(string relativePath)
        {
            if (this.ProgressBar == null)
                this.ProgressBar = new ConsoleProgressBar(this.Context.Files.Count());

            this.ProgressBar.Increment();
            this.ProgressBar.Print(relativePath);
        }

        /// <inheritdoc />
        public void OnFileUnpackFailed(string relativePath, UnpackFailedReason errorCode, string errorMessage)
        {
            ConsoleColor color = errorCode == UnpackFailedReason.UnsupportedFileType
                ? ConsoleColor.DarkYellow
                : ConsoleColor.Red;

            this.ProgressBar.Erase();
            this.PrintColor($"{relativePath} => {errorMessage}", color);
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
    }
}

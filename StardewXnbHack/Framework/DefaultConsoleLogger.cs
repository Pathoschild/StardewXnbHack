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

        /// <summary>Whether to show a 'press any key to exit' prompt on end.</summary>
        private readonly bool ShowPressAnyKeyToExit;

        /// <summary>The current progress bar written to the console.</summary>
        private ConsoleProgressBar ProgressBar;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="context">The context info for the current unpack run.</param>
        /// <param name="showPressAnyKeyToExit">Whether to show a 'press any key to exit' prompt on end.</param>
        public DefaultConsoleLogger(IUnpackContext context, bool showPressAnyKeyToExit)
        {
            this.Context = context;
            this.ShowPressAnyKeyToExit = showPressAnyKeyToExit;
        }

        /// <inheritdoc />
        public void OnFatalError(string error)
        {
            this.PrintColor(error, ConsoleColor.Red);
        }

        /// <inheritdoc />
        public void OnStepChanged(ProgressStep step, string message)
        {
            this.ProgressBar?.Erase();

            if (step == ProgressStep.Done)
                Console.WriteLine();

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

        /// <inheritdoc />
        public void OnEnded()
        {
            if (this.ShowPressAnyKeyToExit)
                DefaultConsoleLogger.PressAnyKeyToExit();
        }

        /// <summary>Show a 'press any key to exit' message and wait for a key press.</summary>
        public static void PressAnyKeyToExit()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
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

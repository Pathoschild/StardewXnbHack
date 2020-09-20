using System;

namespace StardewXnbHack.Framework
{
    /// <summary>Manages a progress bar written to the console.</summary>
    public class ConsoleProgressBar
    {
        /*********
        ** Fields
        *********/
        /// <summary>The total number of steps to perform.</summary>
        protected readonly int TotalSteps;

        /// <summary>The current step being performed.</summary>
        protected int CurrentStep;

        /// <summary>The last line to which the progress bar was output, if any.</summary>
        protected int OutputLine = -1;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="totalSteps">The total number of steps to perform.</param>
        public ConsoleProgressBar(int totalSteps)
        {
            this.TotalSteps = totalSteps;
        }

        /// <summary>Increment the current step.</summary>
        public virtual void Increment()
        {
            this.CurrentStep++;
        }

        /// <summary>Print a progress bar to the console.</summary>
        /// <param name="message">The message to print.</param>
        /// <param name="removePrevious">Whether to remove the previously output progress bar.</param>
        public virtual void Print(string message, bool removePrevious = true)
        {
            if (removePrevious)
                this.Erase();

            int percentage = (int)((this.CurrentStep / (this.TotalSteps * 1m)) * 100);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{"".PadRight(percentage / 10, '#')}{"".PadRight(10 - percentage / 10, ' ')} {percentage}%]  {message}");
            Console.ResetColor();

            this.OutputLine = Console.CursorTop - 1;
        }

        /// <summary>Remove the last progress bar written to the console.</summary>
        /// <remarks>Derived from <a href="https://stackoverflow.com/a/8946847/262123" />.</remarks>
        public virtual void Erase()
        {
            if (this.OutputLine == -1)
                return;

            bool isLastLine = this.OutputLine == Console.CursorTop - 1;
            int currentLine = isLastLine
                ? this.OutputLine
                : Console.CursorTop;

            Console.SetCursorPosition(0, this.OutputLine);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLine);

            this.OutputLine = -1;
        }
    }
}

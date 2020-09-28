namespace StardewXnbHack.ProgressHandling
{
    /// <summary>Logs updates while the unpacker is running.</summary>
    public interface IProgressLogger
    {
        /*********
        ** Methods
        *********/
        /// <summary>Log an error which prevented the unpack from starting (e.g. game folder missing).</summary>
        /// <param name="error">The error message indicating why the unpack can't start.</param>
        void OnStartError(string error);

        /// <summary>Log a step transition in the overall unpack process.</summary>
        /// <param name="step">The new step.</param>
        /// <param name="message">The default log message for the step transition.</param>
        void OnStepChanged(ProgressStep step, string message);

        /// <summary>Log a file being unpacked.</summary>
        /// <param name="relativePath">The relative path of the file within the content folder.</param>
        void OnFileUnpacking(string relativePath);

        /// <summary>Log a file which couldn't be unpacked.</summary>
        /// <param name="relativePath">The relative path of the file within the content folder.</param>
        /// <param name="errorCode">An error code indicating why unpacking failed.</param>
        /// <param name="errorMessage">An error message indicating why unpacking failed.</param>
        void OnFileUnpackFailed(string relativePath, UnpackFailedReason errorCode, string errorMessage);
    }
}

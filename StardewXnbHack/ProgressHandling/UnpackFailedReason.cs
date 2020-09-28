namespace StardewXnbHack.ProgressHandling
{
    /// <summary>An error code indicating why unpacking failed for a file.</summary>
    public enum UnpackFailedReason
    {
        /// <summary>An error occurred trying to read the raw XNB asset.</summary>
        ReadError,

        /// <summary>The XNB asset was successfully loaded, but its file format can't be unpacked.</summary>
        UnsupportedFileType,

        /// <summary>The XNB asset was successfully loaded, but an error occurred trying to save the unpacked file.</summary>
        WriteError,

        /// <summary>An unhandled error occurred.</summary>
        UnknownError
    }
}

namespace StardewXnbHack.ProgressHandling
{
    /// <summary>A step in the overall unpack process.</summary>
    public enum ProgressStep
    {
        /// <summary>The game folder was located, but unpacking hasn't started yet.</summary>
        GameFound,

        /// <summary>The temporary game instance is being loaded.</summary>
        LoadingGameInstance,

        /// <summary>The files are being unpacked.</summary>
        UnpackingFiles,

        /// <summary>The overall unpack process completed successfully.</summary>
        Done
    }
}

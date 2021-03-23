using StardewModdingAPI.Toolkit.Utilities;

namespace StardewXnbHack.Framework
{
    internal static class PlatformExtensions
    {
        /// <summary>Get whether the platform uses Mono.</summary>
        /// <param name="platform">The current platform.</param>
        public static bool IsMono(this Platform platform)
        {
            return platform == Platform.Linux || platform == Platform.Mac;
        }
    }
}

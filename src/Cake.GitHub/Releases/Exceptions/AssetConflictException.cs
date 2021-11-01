namespace Cake.GitHub
{
    /// <summary>
    /// Thrown when multiple assets with the same file name are added to a GitHub Release.
    /// </summary>
    public sealed class AssetConflictException : GitHubReleaseException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AssetConflictException"/>.
        /// </summary>
        public AssetConflictException(string message) : base(message)
        { }
    }
}

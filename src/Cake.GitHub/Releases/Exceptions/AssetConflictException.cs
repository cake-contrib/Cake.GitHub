#nullable enable

namespace Cake.GitHub
{
    /// <summary>
    /// Thrown when multiple assets with the same file name are added to a GitHub Release.
    /// </summary>
    public sealed class AssetConflictException : GitHubReleaseException
    {
        public AssetConflictException(string message) : base(message)
        { }
    }
}

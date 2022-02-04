namespace Cake.GitHub
{
    /// <summary>
    /// Thrown when a GitHub Release with the same tag name already exists
    /// </summary>
    public class ReleaseExistsException : GitHubReleaseException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReleaseExistsException"/>
        /// </summary>
        public ReleaseExistsException(string message) : base(message)
        { }
    }
}

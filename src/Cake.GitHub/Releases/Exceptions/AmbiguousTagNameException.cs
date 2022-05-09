namespace Cake.GitHub
{
    /// <summary>
    /// Thrown if multiple releases with the same tag name are encountered.
    /// </summary>
    public class AmbiguousTagNameException : GitHubReleaseException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AmbiguousTagNameException"/>.
        /// </summary>
        public AmbiguousTagNameException(string message) 
            : base(message)
        { }
    }
}

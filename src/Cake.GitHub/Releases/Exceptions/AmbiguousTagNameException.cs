namespace Cake.GitHub
{
    /// <summary>
    /// Thrown if multiple releases with the same tag name are encountered.
    /// </summary>
    public class AmbiguousTagNameException : GitHubReleaseException
    {
        public AmbiguousTagNameException(string message) : base(message)
        { }
    }
}

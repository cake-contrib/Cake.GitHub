using System;

namespace Cake.GitHub
{
    /// <summary>
    /// Base class for all exceptions thrown by the Cake.GitHubReleases.
    /// </summary>
    public abstract class GitHubReleaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GitHubReleaseException"/>.
        /// </summary>
        protected GitHubReleaseException()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="GitHubReleaseException"/>.
        /// </summary>
        protected GitHubReleaseException(string message) 
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="GitHubReleaseException"/>.
        /// </summary>
        protected GitHubReleaseException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}

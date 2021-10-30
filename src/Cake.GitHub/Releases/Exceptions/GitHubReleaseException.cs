#nullable enable

using System;

namespace Cake.GitHub
{
    /// <summary>
    /// Base class for all exceptions thrown by the Cake.GitHubReleases
    /// </summary>
    public abstract class GitHubReleaseException : Exception
    {
        protected GitHubReleaseException()
        { }

        protected GitHubReleaseException(string message) : base(message)
        { }

        protected GitHubReleaseException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}

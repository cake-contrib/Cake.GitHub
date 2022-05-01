using System;

namespace Cake.GitHub
{
    /// <summary>
    /// Base class for exceptions that can be thrown while updating GitHub Issues (or Pull Requests)
    /// </summary>
    public abstract class GitHubIssueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubIssueException"/> class with the specified error message.
        /// </summary>
        protected GitHubIssueException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubIssueException"/> class with the specified error message and inner exception.
        /// </summary>
        protected GitHubIssueException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}

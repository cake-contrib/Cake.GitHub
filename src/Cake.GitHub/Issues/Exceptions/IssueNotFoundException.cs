using System;

namespace Cake.GitHub
{
    /// <summary>
    /// Thrown when an Issue or Pull Request was not found.
    /// </summary>
    public sealed class IssueNotFoundException : GitHubIssueException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IssueNotFoundException"/> with the specified error message and inner exception.
        /// </summary>
        public IssueNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

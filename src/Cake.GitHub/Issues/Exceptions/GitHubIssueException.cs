using System;

namespace Cake.GitHub
{
    public abstract class GitHubIssueException : Exception
    {
        protected GitHubIssueException(string message) : base(message)
        { }

        protected GitHubIssueException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}

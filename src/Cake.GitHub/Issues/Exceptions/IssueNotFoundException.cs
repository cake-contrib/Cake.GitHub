using System;

namespace Cake.GitHub
{
    internal class IssueNotFoundException : GitHubIssueException
    {
        public IssueNotFoundException(string message) : base(message)
        { }

        public IssueNotFoundException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}

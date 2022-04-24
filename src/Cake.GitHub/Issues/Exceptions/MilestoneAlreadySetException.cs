using System;

namespace Cake.GitHub
{
    internal class MilestoneAlreadySetException : GitHubIssueException
    {
        public MilestoneAlreadySetException(string message) : base(message)
        { }

        public MilestoneAlreadySetException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}

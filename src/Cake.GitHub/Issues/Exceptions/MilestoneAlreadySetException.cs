namespace Cake.GitHub
{
    /// <summary>
    /// Thrown when an Issue's or Pull Request's Milestone is already set.
    /// </summary>
    public sealed class MilestoneAlreadySetException : GitHubIssueException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MilestoneAlreadySetException"/> with the specified error message.
        /// </summary>
        public MilestoneAlreadySetException(string message)
            : base(message)
        { }
    }
}

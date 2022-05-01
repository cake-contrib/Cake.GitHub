namespace Cake.GitHub
{
    /// <summary>
    /// Thrown when no matching Milestone was found.
    /// </summary>
    public sealed class MilestoneNotFoundException : GitHubIssueException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MilestoneNotFoundException"/> with the specified error message.
        /// </summary>
        /// <param name="message"></param>
        public MilestoneNotFoundException(string message) : base(message)
        { }
    }
}

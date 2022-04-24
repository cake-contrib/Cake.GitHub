namespace Cake.GitHub
{
    internal class MilestoneNotFoundException : GitHubIssueException
    {
        public MilestoneNotFoundException(string message) : base(message)
        { }
    }
}

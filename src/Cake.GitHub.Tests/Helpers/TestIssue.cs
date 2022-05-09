using Octokit;

namespace Cake.GitHub.Tests
{
    internal class TestIssue : Issue
    {
        public new int Number
        {
            get => base.Number;
            set => base.Number = value;
        }

        public new Milestone Milestone
        {
            get => base.Milestone;
            set => base.Milestone = value;
        }
    }
}

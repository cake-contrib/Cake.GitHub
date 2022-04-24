using Octokit;

namespace Cake.GitHub.Tests
{
    internal class TestMilestone : Milestone
    {
        public new int Number
        {
            get => base.Number;
            set => base.Number = value;
        }

        public new string Title
        {
            get => base.Title;
            set => base.Title = value;
        }
    }
}

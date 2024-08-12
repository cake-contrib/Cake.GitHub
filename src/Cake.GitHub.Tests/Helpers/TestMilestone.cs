using Octokit;

namespace Cake.GitHub.Tests
{
    internal class TestMilestone : Milestone
    {
        public TestMilestone(int number = default, string title = default)
            : base(
                url: default,
                htmlUrl: default,
                id: default,
                number: number,
                nodeId: default,
                state: default,
                title: title,
                description: default,
                creator: default,
                openIssues: default,
                closedIssues: default,
                createdAt: default,
                dueOn: default,
                closedAt: default,
                updatedAt: default)
        { }
    }
}

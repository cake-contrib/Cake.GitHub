using Octokit;
using System;

namespace Cake.GitHub.Tests
{
    internal class TestIssue : Issue
    {
        public TestIssue(int number = default, Milestone milestone = default)
            : base(
                url: default,
                htmlUrl: default,
                commentsUrl: default,
                eventsUrl: default,
                number: number,
                state: default,
                title: default,
                body: default,
                closedBy: default,
                user: default,
                labels: Array.Empty<Label>(),
                assignee: default,
                assignees: Array.Empty<User>(),
                milestone: milestone,
                comments: 0,
                pullRequest: default,
                closedAt: default,
                createdAt: default,
                updatedAt: default,
                id: default,
                nodeId: default,
                locked: default,
                repository: default,
                reactions: default,
                activeLockReason: default,
                stateReason: default)
        { }
    }
}

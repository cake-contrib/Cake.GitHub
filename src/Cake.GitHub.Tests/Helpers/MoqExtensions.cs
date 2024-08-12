using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.Language;
using Moq.Language.Flow;
using Octokit;

namespace Cake.GitHub.Tests
{
    internal static class MoqExtensions
    {
        public static IReturnsResult<TMock> ThrowsNotFoundAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock) where TMock : class =>
            mock.ThrowsAsync(new NotFoundException(string.Empty, HttpStatusCode.NotFound));

        public static IReturnsResult<IReleasesClient> ReturnsEmptyListAsync(this IReturns<IReleasesClient, Task<IReadOnlyList<Release>>> mock) =>
            mock.ReturnsAsync(Array.Empty<Release>());

        public static IReturnsResult<TMock> ReturnsCompletedTask<TMock>(this IReturns<TMock, Task> mock) where TMock : class =>
            mock.Returns(Task.CompletedTask);

        public static IReturnsResult<IIssuesClient> SetupUpdate(this Mock<IIssuesClient> mock) =>
            mock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IssueUpdate>()))
                .ReturnsAsync((string owner, string repo, int number, IssueUpdate update) => new TestIssue(number));

        public static IReturnsResult<IMilestonesClient> ReturnsMilestonesAsync(this IReturns<IMilestonesClient, Task<IReadOnlyList<Milestone>>> mock, params Milestone[] milestones) =>
            mock.ReturnsAsync(milestones);

        public static IReturnsResult<IMilestonesClient> ReturnsEmptyListAsync(this IReturns<IMilestonesClient, Task<IReadOnlyList<Milestone>>> mock) =>
            mock.ReturnsAsync(Array.Empty<Milestone>());
    }
}

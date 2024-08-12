using Moq;
using Octokit;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Cake.GitHub.Tests
{
    /// <summary>
    /// Tests for <see cref="GitHubIssueUpdater"/>
    /// </summary>
    public class GitHubIssueUpdaterTests
    {
        private readonly XunitCakeLog _testLog;
        private readonly GitHubClientMock _clientMock;


        public GitHubIssueUpdaterTests(ITestOutputHelper testOutputHelper)
        {
            _testLog = new XunitCakeLog(testOutputHelper);
            _clientMock = new GitHubClientMock();
        }


        [Theory]
        [InlineData(null, "repo", "milestone", "owner")]
        [InlineData("", "repo", "milestone", "owner")]
        [InlineData(" ", "repo", "milestone", "owner")]
        [InlineData("\t", "repo", "milestone", "owner")]
        [InlineData("owner", null, "milestone", "repository")]
        [InlineData("owner", "", "milestone", "repository")]
        [InlineData("owner", " ", "milestone", "repository")]
        [InlineData("owner", "\t", "milestone", "repository")]
        [InlineData("owner", "repo", null, "milestoneTitle")]
        [InlineData("owner", "repo", "", "milestoneTitle")]
        [InlineData("owner", "repo", " ", "milestoneTitle")]
        [InlineData("owner", "repo", "\t", "milestoneTitle")]
        public async Task SetMilestoneAsync_checks_string_parameters_for_null_or_whitespace(string owner, string repo, string milestoneTitle, string expectedParameterName)
        {
            // ARRANGE
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.SetMilestoneAsync(owner: owner, repository: repo, number: 23, milestoneTitle: milestoneTitle, new GitHubSetMilestoneSettings()));

            // ASSERT
            var argumentNullException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal(expectedParameterName, argumentNullException.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-23)]
        public async Task SetMilestoneAsync_checks_issue_or_pr_number(int number)
        {
            // ARRANGE
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.SetMilestoneAsync(owner: "owner", repository: "repo", number: number, milestoneTitle: "milestone", new GitHubSetMilestoneSettings()));

            // ASSERT
            var argumentNullException = Assert.IsType<ArgumentOutOfRangeException>(ex);
            Assert.Equal("number", argumentNullException.ParamName);
        }

        [Fact]
        public async Task SetMilestoneAsync_throws_IssueNotFoundException_if_issue_does_not_exist()
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var number = 23;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, number))
                .ThrowsNotFoundAsync();

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.SetMilestoneAsync(owner: owner, repository: repo, number: number, milestoneTitle: "milestone", new GitHubSetMilestoneSettings()));

            // ASSERT
            Assert.IsType<IssueNotFoundException>(ex);
        }

        [Fact]
        public async Task SetMilestoneAsync_throws_MilestoneNotFoundException_if_milestone_does_not_exist()
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var number = 23;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, number))
                .ReturnsAsync(new TestIssue(number));

            _clientMock.Issues.Milestone
                .Setup(x => x.GetAllForRepository(owner, repo, It.IsAny<MilestoneRequest>()))
                .ReturnsMilestonesAsync(
                    new TestMilestone(number: 1, title: "Milestone 1"),
                    new TestMilestone(number: 1, title: "Milestone 2")
                );

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.SetMilestoneAsync(owner: owner, repository: repo, number: number, milestoneTitle: "milestone", new GitHubSetMilestoneSettings()));

            // ASSERT
            Assert.IsType<MilestoneNotFoundException>(ex);
            _clientMock.Issues.Milestone.Verify(x => x.GetAllForRepository(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MilestoneRequest>()), Times.Once);

            // When requesting milestones, request both open and closes milestones
            _clientMock.Issues.Milestone.Verify(x => x.GetAllForRepository(owner, repo, It.Is<MilestoneRequest>(x => x.State == ItemStateFilter.All)), Times.Once);
        }

        [Theory]
        [InlineData("Milestone 1", "Milestone 1", true)]
        [InlineData("milestone 1", "milestone 1", true)]
        [InlineData("Milestone 1", "milestone 1", false)]
        [InlineData("milestone 1", "Milestone 1", false)]
        public async Task SetMilestoneAsync_uses_case_sensitive_comparison_for_milestone_titles(string existingMilestoneTitle, string requestedMilestoneTitle, bool expectMatch)
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var number = 23;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, number))
                .ReturnsAsync(new TestIssue(number));

            _clientMock.Issues.Mock.SetupUpdate();

            _clientMock.Issues.Milestone
                .Setup(x => x.GetAllForRepository(owner, repo, It.IsAny<MilestoneRequest>()))
                .ReturnsMilestonesAsync(
                    new TestMilestone(number: number, title: existingMilestoneTitle)
                );

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.SetMilestoneAsync(owner: owner, repository: repo, number: number, milestoneTitle: requestedMilestoneTitle, new GitHubSetMilestoneSettings()));

            // ASSERT
            if (expectMatch)
            {
                Assert.Null(ex);
            }
            else
            {
                Assert.IsType<MilestoneNotFoundException>(ex);
            }
        }

        [Fact]
        public async Task SetMilestoneAsync_sets_the_milestone_to_the_expected_value()
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var issueNumber = 23;
            var milestoneTitle = "Milestone 1";
            var milestoneNumber = 42;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, issueNumber))
                .ReturnsAsync(new TestIssue(issueNumber));

            _clientMock.Issues.Milestone
                .Setup(x => x.GetAllForRepository(owner, repo, It.IsAny<MilestoneRequest>()))
                .ReturnsMilestonesAsync(
                    new TestMilestone(number: milestoneNumber, title: milestoneTitle)
                );

            _clientMock.Issues.Mock.SetupUpdate();

            // ACT 
            await sut.SetMilestoneAsync(owner: owner, repository: repo, number: issueNumber, milestoneTitle: milestoneTitle, new GitHubSetMilestoneSettings());

            // ASSERT
            _clientMock.Issues.Mock.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IssueUpdate>()), Times.Once);
            _clientMock.Issues.Mock.Verify(x => x.Update(owner, repo, issueNumber, It.Is<IssueUpdate>(x => x.Milestone == milestoneNumber)), Times.Once);
        }

        [Fact]
        public async Task SetMilestoneAsync_throws_MilestoneAlreadySetException_when_the_issues_milestone_is_already_set()
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var issueNumber = 23;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, issueNumber))
                .ReturnsAsync(new TestIssue(number: issueNumber, milestone: new TestMilestone(number: 1, title: "Milestone 1")));

            _clientMock.Issues.Milestone
                .Setup(x => x.GetAllForRepository(owner, repo, It.IsAny<MilestoneRequest>()))
                .ReturnsMilestonesAsync(
                    new TestMilestone(number: 1, title: "Milestone 1"),
                    new TestMilestone(number: 2, title: "Milestone 2")
                );

            _clientMock.Issues.Mock.SetupUpdate();

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.SetMilestoneAsync(owner: owner, repository: repo, number: issueNumber, milestoneTitle: "Milestone 2", new GitHubSetMilestoneSettings()));

            // ASSERT
            Assert.IsType<MilestoneAlreadySetException>(ex);
            _clientMock.Issues.Mock.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IssueUpdate>()), Times.Never);
        }

        [Fact]
        public async Task SetMilestoneAsync_performs_no_update_if_milestone_is_already_set_to_expected_value()
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var issueNumber = 23;
            var milestoneTitle = "Milestone 1";
            var milestoneNumber = 42;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, issueNumber))
                .ReturnsAsync(new TestIssue(number: issueNumber, milestone: new TestMilestone(milestoneNumber)));

            _clientMock.Issues.Milestone
                .Setup(x => x.GetAllForRepository(owner, repo, It.IsAny<MilestoneRequest>()))
                .ReturnsMilestonesAsync(new TestMilestone(number: milestoneNumber, title: milestoneTitle));

            // ACT 
            await sut.SetMilestoneAsync(owner: owner, repository: repo, number: issueNumber, milestoneTitle: milestoneTitle, new GitHubSetMilestoneSettings());

            // ASSERT
            _clientMock.Issues.Mock.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IssueUpdate>()), Times.Never);
        }

        [Fact]
        public async Task SetMilestoneAsync_overwrites_the_milestone_if_overwrite_setting_is_true()
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var issueNumber = 23;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, issueNumber))
                .ReturnsAsync(new TestIssue(number: issueNumber, milestone: new TestMilestone(number: 1, title: "Milestone 1")));

            _clientMock.Issues.Milestone
                .Setup(x => x.GetAllForRepository(owner, repo, It.IsAny<MilestoneRequest>()))
                .ReturnsMilestonesAsync(
                    new TestMilestone(number: 1, title: "Milestone 1"),
                    new TestMilestone(number: 2, title: "Milestone 2")
                );

            _clientMock.Issues.Mock.SetupUpdate();

            var settings = new GitHubSetMilestoneSettings()
            {
                Overwrite = true
            };

            // ACT 
            await sut.SetMilestoneAsync(owner: owner, repository: repo, number: issueNumber, milestoneTitle: "Milestone 2", settings);

            // ASSERT
            _clientMock.Issues.Mock.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IssueUpdate>()), Times.Once);
            _clientMock.Issues.Mock.Verify(x => x.Update(owner, repo, issueNumber, It.Is<IssueUpdate>(x => x.Milestone == 2)), Times.Once);
        }

        [Fact]
        public async Task SetMilestoneAsync_creates_a_new_milestone_if_CreateMilestone_setting_is_true()
        {
            // ARRANGE
            var owner = "owner";
            var repo = "repo";
            var issueNumber = 23;
            var sut = new GitHubIssueUpdater(_testLog, _clientMock.Object);

            _clientMock.Issues.Mock
                .Setup(x => x.Get(owner, repo, issueNumber))
                .ReturnsAsync(new TestIssue(issueNumber));

            _clientMock.Issues.Milestone
                .Setup(x => x.GetAllForRepository(owner, repo, It.IsAny<MilestoneRequest>()))
                .ReturnsEmptyListAsync();

            _clientMock.Issues.Milestone
                .Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewMilestone>()))
                .ReturnsAsync((string repo, string owner, NewMilestone newMilestone) => new TestMilestone(number: 2, title: newMilestone.Title));

            _clientMock.Issues.Mock.SetupUpdate();

            var settings = new GitHubSetMilestoneSettings()
            {
                CreateMilestone = true
            };

            // ACT 
            await sut.SetMilestoneAsync(owner: owner, repository: repo, number: issueNumber, milestoneTitle: "Milestone 2", settings);

            // ASSERT
            _clientMock.Issues.Mock.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IssueUpdate>()), Times.Once);
            _clientMock.Issues.Mock.Verify(x => x.Update(owner, repo, issueNumber, It.Is<IssueUpdate>(x => x.Milestone == 2)), Times.Once);

            _clientMock.Issues.Milestone.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewMilestone>()), Times.Once);
            _clientMock.Issues.Milestone.Verify(x => x.Create(owner, repo, It.Is<NewMilestone>(x => x.Title == "Milestone 2")), Times.Once);
        }
    }
}

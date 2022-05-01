using Cake.Core.Diagnostics;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.GitHub
{
    internal sealed class GitHubIssueUpdater
    {
        private readonly ICakeLog _cakeLog;
        private readonly IGitHubClient _gitHubClient;


        public GitHubIssueUpdater(ICakeLog cakeLog, IGitHubClient gitHubClient)
        {
            _cakeLog = cakeLog;
            _gitHubClient = gitHubClient;
        }


        public async Task SetMilestoneAsync(string owner, string repository, int number, string milestoneTitle, GitHubSetMilestoneSettings settings)
        {
            if (String.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Value must not be null or whitespace", nameof(owner));

            if (String.IsNullOrWhiteSpace(repository))
                throw new ArgumentException("Value must not be null or whitespace", nameof(repository));

            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number), "Value must not be negative");

            if (String.IsNullOrWhiteSpace(milestoneTitle))
                throw new ArgumentException("Value must not be null or whitespace", nameof(milestoneTitle));

            _cakeLog.Information($"Setting Milestone for Issue or Pull Request {number}");

            LogSettings(owner, repository, number, milestoneTitle, settings);

            // Get issue or PR (every PR is also an issue and can be retrieved via the Issues API)
            var issue = await GetIssueAsync(owner, repository, number);
            var displayName = $"{(issue.PullRequest == null ? "Issue" : "Pull Request")} {number}";

            // Get the milestone matching the specified title
            var milestone = await GetOrCreateMilestoneAsync(owner, repository, milestoneTitle, settings);


            // Update Issue or PR
            if (issue.Milestone == null)
            {
                _cakeLog.Verbose($"{displayName} is not yet assigned to a Milestone, setting to Milestone {milestone.Number} '{milestone.Title}'");
                await _gitHubClient.Issue.Update(owner, repository, number, new IssueUpdate() { Milestone = milestone.Number });
            }
            else
            {
                if (issue.Milestone.Number == milestone.Number)
                {
                    _cakeLog.Verbose($"{displayName} is already assigned to Milestone {milestone.Number} '{milestone.Title}'");
                    return;
                }
                else if (settings.Overwrite)
                {
                    _cakeLog.Verbose($"Reassigning {displayName} Milestone {milestone.Number} '{milestone.Title}' because the 'Overwrite' setting was set to true.");
                    await _gitHubClient.Issue.Update(owner, repository, number, new IssueUpdate() { Milestone = milestone.Number });
                }
                else
                {
                    throw new MilestoneAlreadySetException($"{displayName} is already assigned to Milestone {issue.Milestone.Number} '{issue.Milestone.Title}'");
                }
            }
        }

        private void LogSettings(string owner, string repository, int number, string milestoneTitle, GitHubSetMilestoneSettings settings)
        {
            const int padding = -29;

            _cakeLog.Debug("Setting GitHub Milestone with the following settings:");
            _cakeLog.Debug($"\t{"Owner",padding}: '{owner}'");
            _cakeLog.Debug($"\t{"Repository",padding}: '{repository}'");
            _cakeLog.Debug($"\t{"Issue or Pull Requets Number",padding}: '{number}'");
            _cakeLog.Debug($"\t{"MilestoneTitle",padding}: '{milestoneTitle}'");
            _cakeLog.Debug($"\t{nameof(settings.Overwrite),padding}: '{settings.Overwrite}'");
            _cakeLog.Debug($"\t{nameof(settings.CreateMilestone),padding}: '{settings.CreateMilestone}'");
        }

        private async Task<Milestone> GetOrCreateMilestoneAsync(string owner, string repository, string milestoneTitle, GitHubSetMilestoneSettings settings)
        {
            _cakeLog.Verbose($"Looking up Milestone with title '{milestoneTitle}'");

            // Get all milestones and filter client-side since we only know the name and not the milestone number
            var milestones = await _gitHubClient.Issue.Milestone.GetAllForRepository(owner, repository, new MilestoneRequest() { State = ItemStateFilter.All });
            var milestone = milestones.SingleOrDefault(x => StringComparer.Ordinal.Equals(x.Title, milestoneTitle));

            if (milestone == null)
            {
                if (settings.CreateMilestone)
                {
                    _cakeLog.Verbose($"No Milestone titled '{milestoneTitle}' was not found in repository {owner}/{repository}. Creating new milestone.");
                    milestone = await _gitHubClient.Issue.Milestone.Create(owner, repository, new NewMilestone(milestoneTitle));
                    _cakeLog.Verbose($"Created Milestone {milestone.Number}");
                    return milestone;
                }
                else
                {
                    throw new MilestoneNotFoundException($"No Milestone titled '{milestoneTitle}' was not found in repository {owner}/{repository}");
                }
            }
            else
            {
                _cakeLog.Verbose($"Found Milestone {milestone.Number} with matching title");
                return milestone;
            }
        }

        private async Task<Issue> GetIssueAsync(string owner, string repository, int number)
        {
            _cakeLog.Verbose($"Retrieving Issue or Pull Request '{number}'");
            Issue issue;
            try
            {
                issue = await _gitHubClient.Issue.Get(owner, repository, number);
            }
            catch (NotFoundException ex)
            {
                throw new IssueNotFoundException($"Issue or Pull Request with number '{number}' was not found in repository {owner}/{repository}", ex);
            }

            return issue;
        }
    }
}

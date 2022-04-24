using Cake.Core;
using Cake.Core.Annotations;
using Octokit;
using System.Threading.Tasks;

namespace Cake.GitHub
{
    [CakeAliasCategory("GitHub")]
    public static partial class GitHubAliases
    {
        //TODO: Allow specifxing milestone number instead of milestone name

        [CakeMethodAlias]
        public static async Task GitHubSetMilestoneAsync(
            this ICakeContext context,
            string? userName,
            string apiToken,
            string owner,
            string repository,
            int number,
            string milestoneTitle,
            GitHubSetMilestoneSettings? settings = null)
        {
            settings = settings ?? new GitHubSetMilestoneSettings();

            var connection = CreateConnection(userName, apiToken);
            var githubClient = new GitHubClient(connection);
            var issueUpdater = new GitHubIssueUpdater(context.Log, githubClient);

            await issueUpdater.SetMilestoneAsync(
                owner: owner,
                repository: repository,
                number: number,
                milestoneTitle: milestoneTitle,
                settings: settings
            );
        }
    }
}

using Cake.Core;
using Cake.Core.Annotations;
using Octokit;
using System.Threading.Tasks;

namespace Cake.GitHub
{
    [CakeAliasCategory("GitHub")]
    public static partial class GitHubAliases
    {
        /// <summary>
        /// Sets the Milestone for an Issue or Pull Request.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="userName">The user name to use for authentication (pass <c>null</c> when using an access token).</param>
        /// <param name="apiToken">The access token or password to use for authentication.</param>
        /// <param name="owner">The owner (user or group) of the repository.</param>
        /// <param name="repository">The name of the repository.</param>
        /// <param name="number">The number of the Issue or Pull Request to set the Milestone for.</param>
        /// <param name="milestoneTitle">
        /// The title of the milestone to assign the Issue or Pull Request to.
        /// Note that GitHub treats Milestone titles case-sensitive.
        /// </param>
        /// <param name="settings">Additional settings for updating the Milestone (optional).</param>
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

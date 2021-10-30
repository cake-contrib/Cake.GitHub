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
        /// Creates a new GitHub Release with the specified settings.
        /// </summary>
        [CakeMethodAlias]
        public static async Task<GitHubRelease> GitHubCreateReleaseAsync(
            this ICakeContext context,
            string userName,
            string apiToken,
            string owner,
            string repository,
            string tagName,
            GitHubCreateReleaseSettings settings = null)
        {
            settings = settings ?? new GitHubCreateReleaseSettings();

            var connection = CreateConnection(userName, apiToken);
            var githubClient = new GitHubClient(connection);
            var releaseCreator = new GitHubReleaseCreator(context.Log, context.FileSystem, githubClient);

            return await releaseCreator.CreateReleaseAsync(
                owner: owner,
                repository: repository,
                tagName: tagName,
                settings: settings
            );
        }
    }
}

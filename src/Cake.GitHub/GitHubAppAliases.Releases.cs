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
        /// <param name="context">The Cake context</param>
        /// <param name="userName">The user name to use for authentication (pass <c>null</c> when using an access token).</param>
        /// <param name="apiToken">The access token or password to use for authentication.</param>
        /// <param name="owner">The owner (user or group) of the repository to create a Release in.</param>
        /// <param name="repository">The name of the repository to create a release in.</param>
        /// <param name="tagName">
        /// The name of the tag to create a release for. 
        /// If the tag does not yet exist, a new tag will be created (using either the HEAD of the default branch or the commit specified in <see cref="GitHubCreateReleaseSettings.TargetCommitish"/>).
        /// If the tag already exists, the existing tag will be used and <see cref="GitHubCreateReleaseSettings.TargetCommitish" /> will be ignored.
        /// </param>
        /// <param name="settings">Additional settings for creating the release.</param>
        [CakeMethodAlias]
        public static async Task<GitHubRelease> GitHubCreateReleaseAsync(
            this ICakeContext context,
            string userName,
            string apiToken,
            string owner,
            string repository,
            string tagName,
            GitHubCreateReleaseSettings? settings = null)
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

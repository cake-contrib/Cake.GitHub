using Cake.Core;
using Cake.Core.Annotations;
using System;
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
            GitHubCreateReleaseSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            var releaseCreator = new GitHubReleaseCreator(context.Log, context.FileSystem, new GitHubClientFactory());
            return await releaseCreator.CreateRelease(
                userName: userName, 
                apiToken: apiToken, 
                owner: owner, 
                repository: repository, 
                tagName: tagName, 
                settings: settings
            );
        }
    }
}

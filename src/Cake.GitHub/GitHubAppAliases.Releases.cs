namespace Cake.GitHub
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Core;
    using Core.Annotations;
    using Core.Diagnostics;
    using Core.IO;
    using Octokit;

    [CakeAliasCategory("GitHub")]
    public static partial class GitHubAliases
    {
        // TODO: Make alaias more similar to GitHubStatus()
        /// <summary>
        /// Creates a new GitHub Release with the specified settings.
        /// </summary>
        [CakeMethodAlias]
        public static async Task<GitHubRelease> GitHubCreateReleaseAsync(this ICakeContext context, GitHubCreateReleaseSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            var releaseCreator = new GitHubReleaseCreator(context.Log, context.FileSystem, new GitHubClientFactory());
            return await releaseCreator.CreateRelease(settings);
        }
    }
}

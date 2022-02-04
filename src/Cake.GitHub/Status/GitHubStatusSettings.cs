namespace Cake.GitHub
{
    public class GitHubStatusSettings : GitHubSettingsBase
    {
        /// <summary>
        /// The state of the status. Can be one of error, failure, pending, or success.
        /// </summary>
        public GitHubStatusState State { get; set; }

        /// <summary>
        /// The target URL to associate with this status. This URL will be linked from the GitHub UI to 
        /// allow users to easily see the source of the status.
        /// <para />
        /// For example, if your continuous integration system is posting build status, you would want to 
        /// provide the deep link for the build output for this specific SHA: 
        /// <para />
        /// <c>http://ci.example.com/user/repo/build/sha</c>
        /// </summary>
        public string? TargetUrl { get; set; }

        /// <summary>
        /// A short description of the status.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// A string label to differentiate this status from the status of other systems.
        /// <para />
        /// The default value is <c>default</c>.
        /// </summary>
        public string? Context { get; set; }
    }
}
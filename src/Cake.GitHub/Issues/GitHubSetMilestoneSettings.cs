namespace Cake.GitHub
{
    public class GitHubSetMilestoneSettings : GitHubSettingsBase
    {
        /// <summary>
        /// Gets or sets whether to replace a Issue's or Pull Request's milestone if it is already set
        /// </summary>
        public bool Overwrite { get; set; }
    }
}

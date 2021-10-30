#nullable enable

using System;

namespace Cake.GitHub
{
    //TODO: Merge with GitHubSettingsBase
    /// <summary>
    /// Base class for all GitHub settings types
    /// </summary>
    public abstract class GitHubSettings
    {
        /// <summary>
        /// Gets of sets the host name of the GitHub server to use (default: <c>github.com</c>)
        /// </summary>
        public string HostName { get; set; } = "github.com";
    }
}

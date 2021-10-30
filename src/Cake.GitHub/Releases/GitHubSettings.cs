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
        /// The owner of the repository on GitHub
        /// </summary>
        public string RepositoryOwner { get; set; } = "";

        /// <summary>
        /// The name of the repository on GitHub
        /// </summary>
        public string RepositoryName { get; set; } = "";

        /// <summary>
        /// Gets or sets the access token used to authenticate to GitHub.
        /// Set to <c>null</c> to make unauthenticated requests.
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Gets of sets the host name of the GitHub server to use (default: <c>github.com</c>)
        /// </summary>
        public string HostName { get; set; } = "github.com";


        protected GitHubSettings(string repositoryOwner, string repositoryName)
        {
            if (String.IsNullOrWhiteSpace(repositoryOwner))
                throw new ArgumentException("Value must not be null or whitespace", nameof(repositoryOwner));

            if (String.IsNullOrWhiteSpace(repositoryName))
                throw new ArgumentException("Value must not be null or whitespace", nameof(repositoryName));

            RepositoryOwner = repositoryOwner;
            RepositoryName = repositoryName;
        }
    }
}

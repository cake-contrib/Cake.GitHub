#nullable enable

using System;
using Octokit;

namespace Cake.GitHub
{
    internal class GitHubClientFactory : IGitHubClientFactory
    {
        public IGitHubClient CreateClient(string hostName, string? accessToken)
        {
            if (String.IsNullOrWhiteSpace(hostName))
                throw new ArgumentException("Value must not be null or whitespace", nameof(hostName));

            var client = new GitHubClient(new ProductHeaderValue("Cake.GitHubReleases"), new Uri($"https://{hostName}/"));

            if (!String.IsNullOrEmpty(accessToken))
            {
                client.Credentials = new Credentials(accessToken);
            }

            return client;
        }
    }
}

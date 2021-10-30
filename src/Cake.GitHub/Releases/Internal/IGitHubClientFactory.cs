#nullable enable 

using Octokit;

namespace Cake.GitHub
{
    internal interface IGitHubClientFactory
    {
        IGitHubClient CreateClient(string hostName, string? accessToken);
    }
}


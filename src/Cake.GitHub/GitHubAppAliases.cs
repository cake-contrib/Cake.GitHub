namespace Cake.GitHub
{
    using System;
    using System.Diagnostics;
    using Core;
    using Core.Annotations;
    using Core.Diagnostics;
    using Core.IO;
    using Octokit;

    /// <summary>
    /// <para>Contains functionality related to GitHub.</para>
    /// <para>
    /// It allows you to run calls against GitHub with just one line of code. In order to use the exposed
    /// commands you have to add the following line at top of your build.cake file.
    /// </para>
    /// <code>
    /// #addin Cake.GitHub
    /// </code>
    /// </summary>
    [CakeAliasCategory("GitHub")]
    public static partial class GitHubAliases
    {
        private static ApiConnection CreateApiConnection(string userName, string apiToken)
        {
            Credentials credentials;

            if (string.IsNullOrWhiteSpace(userName))
            {
                credentials = new Credentials(apiToken);
            }
            else
            {
                credentials = new Credentials(userName, apiToken);
            }

            var connection = new Connection(new ProductHeaderValue("Cake.GitHub"))
            {
                Credentials = credentials
            };

            var apiConnection = new Octokit.ApiConnection(connection);
            return apiConnection;
        }
    }
}

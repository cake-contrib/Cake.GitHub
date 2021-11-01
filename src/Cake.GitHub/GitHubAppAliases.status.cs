namespace Cake.GitHub
{
    using System;
    using System.Diagnostics;
    using Core;
    using Core.Annotations;
    using Core.Diagnostics;
    using Core.IO;
    using Octokit;

    [CakeAliasCategory("GitHub")]
    public static partial class GitHubAliases
    {
        /// <summary>
        /// Updates the status for a specific build.
        /// </summary>
        /// <param name="context">The Cake context</param>
        /// <param name="userName">The user name to use for authentication (pass <c>null</c> when using an access token).</param>
        /// <param name="apiToken">The access token or password to use for authentication.</param>
        /// <param name="owner">The owner (user or group) of the repository to report the status for.</param>
        /// <param name="repository">The name of the repository to report the status for.</param>
        /// <param name="reference">The git reference to report the status for.</param>
        /// <param name="settings">The status settings.</param>
        /// <example>
        /// <code>
        /// GitHubStatus("user", "apiToken", "cake-contrib", "cake.github", new GitHubStatusSettings
        /// {
        ///     AppId = appId,
        ///     Version = "1.0.160901.1",
        ///     ShortVersion = "1.0-beta2",
        ///     Notes = "Uploaded via continuous integration."
        /// });
        /// </code>
        /// </example>
        [CakeAliasCategory("Deployment")]
        [CakeMethodAlias]
        public static GitHubStatusResult GitHubStatus(this ICakeContext context, string? userName, string apiToken, string owner, string repository, string reference, GitHubStatusSettings? settings)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            settings = settings ?? new GitHubStatusSettings();

            var connection = CreateConnection(userName, apiToken);

            try
            {
                var commitStatus = new NewCommitStatus
                {
                    Description = settings.Description,
                    Context = settings.Context,
                    TargetUrl = settings.TargetUrl
                };

                switch (settings.State)
                {
                    case GitHubStatusState.Success:
                        commitStatus.State = CommitState.Success;
                        break;

                    case GitHubStatusState.Pending:
                        commitStatus.State = CommitState.Pending;
                        break;

                    case GitHubStatusState.Failure:
                        commitStatus.State = CommitState.Failure;
                        break;

                    case GitHubStatusState.Error:
                        commitStatus.State = CommitState.Error;
                        break;
                }

                var commitStatusClient = new CommitStatusClient(new ApiConnection(connection));
                var createStatusTask = commitStatusClient.Create(owner, repository, reference, commitStatus);
                createStatusTask.Wait();

                var taskResult = createStatusTask.Result;

                var result = new GitHubStatusResult
                {
                    Id = taskResult.Id.ToString(),
                    Url = taskResult.Url,
                    State = taskResult.State.ToString()
                };

                return result;
            }
            catch (Exception ex)
            {
                do context.Log.Error(ex.Message); while ((ex = ex.InnerException) != null);

                throw new Exception("Failed to create status.");
            }
        }
    }
}

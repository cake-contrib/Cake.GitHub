# Cake.GitHub

Add-in for Cake that allows integration with GitHub. The following integrations are supported:

* [Status](#status)
* [Create Release](#create-release)
* [Set Milestone](#set-milestone)

## Status

Allows updating the status of a build in GitHub.

API documentation: https://docs.github.com/en/rest/reference/repos#create-a-commit-status

```cs
GitHubStatus("username", "apitoken", "owner", "repository", "commitSha", new GitHubStatusSettings
{
    State = GitHubStatusState.Pending,
    TargetUrl = "url-to-build-server",
    Description = "Build is pending",
    Context = "default"
})
```

## Create Release

Allows creating a [GitHub Release](https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases)

API documentation: https://docs.github.com/en/rest/reference/repos#create-a-release

```cs
Task("CreateGitHubRelease")
.Does(async () =>
{
    await GitHubCreateReleaseAsync(
        /// The user name to use for authentication (pass null when using an access token).
        userName: "user",
        /// The access token or password to use for authentication.
        apiToken: "apitoken",
        // The owner (user or group) of the repository to create a release in.
        owner: "owner", 
        /// The name of the repository to create a release in.
        repository: "repository"
        /// The name of the tag to create a release for. 
        /// If the tag does not yet exist, a new tag will be created (using either the HEAD of the default branch or the commit specified in the settings).
        /// If the tag already exists, the existing tag will be used and the commit specified in the settings will be ignored.
        tagName: "v1.2.3",

        // Specify additional settings for the release (optional)
        settings: GitHubCreateReleaseSettings() 
        {
            // The id of the commit to create the release from 
            // (uses the HEAD commit of the repo's default branch if not specified)
            TargetCommitish = "abc123",

            // Set the name of the release (defaults to the tag name when not specified)
            Name = $"v1.2.3",

            // The release's description as Markdown string (optional)
            Body = "Description",
            
            // Set to true to create a draft release (default: false)
            Draft = false,
            
            // Set to true to mark the release as prerelease (default: false)
            Prerelease = false,

            // Overwrite will delete any existing commit with the same tag name if it exists
            Overwrite = false
        }
    );

});
```

## Set Milestone

Assigns a Issue of Pull Request to a [Milestone](https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/about-milestones).

API documentation: https://docs.github.com/en/rest/issues/issues#update-an-issue

```cs
Task("SetMilestone")
.Does(async () =>
{
    await GitHubSetMilestoneAsync(
        // The user name to use for authentication (pass null when using an access token).
        userName: "user",

        // The access token or password to use for authentication.
        apiToken: "apitoken",

        // The owner (user or group) of the repository.
        owner: "owner",

        // The name of the repository.
        repository: "repository",

        // The number of the issue or pull request to set the milestone for.
        number: 23,

        // The title of the milestone to assign the issue or pull request to.
        // Note that GitHub treats milestone titles *case-sensitive*.
        milestoneTitle: "Milestone 1",

        // Specify additional settings for updating the milestone (optional)
        settings: GitHubSetMilestoneSettings()
        {
            // Set to true to set the issue's or pull request's milestone even if it is already set to a different milestone (default: false)
            Overwrite = false,

            // Set to true to create a milestone with the specified title if no such milestone exists (default: false)
            // When set to false, GitHubSetMilestoneAsync() will fail if no matching milestone is found.
            CreateMilestone = false
        }
    );
});
```

# Cake.GitHub

Add-in for Cake that allows integration with GitHub. The following integrations are support:

* Status
* Create Release
 
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

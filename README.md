# Cake.GitHub

Add-in for Cake that allows integration with GitHub. The following integrations are support:

* Status
 
## Status

Allows updating the status of a build in GitHub.

API documentation: https://developer.github.com/v3/repos/statuses/#create-a-status

```
GitHubStatus("username", "apitoken", "owner", "repository", new GitHubStatusSettings
{
    State = GitHubStatusState.Pending,
    TargetUrl = "url-to-build-server",
    Description = "Build is pending",
    Context = "default"
})
```



using Octokit;

namespace Cake.GitHub.Tests
{
    public class TestReleaseAsset : ReleaseAsset
    {
        public TestReleaseAsset(string name)
            : base(
                url: default,
                id: default,
                nodeId: default,
                name: name,
                label: default,
                state: default,
                contentType: default,
                size: default,
                downloadCount: default,
                createdAt: default,
                updatedAt: default,
                browserDownloadUrl: default,
                uploader: default)
        { }
    }
}

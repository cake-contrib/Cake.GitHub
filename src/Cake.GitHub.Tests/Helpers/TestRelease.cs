using Octokit;
using System;

namespace Cake.GitHub.Tests
{
    internal class TestRelease : Release
    {
        public TestRelease(long id = default, string tagName = default, bool draft = default)
            : base(
                url: default,
                htmlUrl: default,
                assetsUrl: default,
                uploadUrl: default,
                id: id,
                nodeId: default,
                tagName: tagName,
                targetCommitish: default,
                name: default,
                body: default,
                draft: draft,
                prerelease: default,
                createdAt: default,
                publishedAt: default,
                author: default,
                tarballUrl: default,
                zipballUrl: default,
                assets: Array.Empty<ReleaseAsset>())
        { }
    }
}

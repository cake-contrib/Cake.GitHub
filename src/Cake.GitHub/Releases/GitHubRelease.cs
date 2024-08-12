﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cake.GitHub
{
    /// <summary>
    /// Provides information about a GitHub Release.
    /// </summary>
    public sealed class GitHubRelease
    {
        private readonly List<GitHubReleaseAsset> _assets = new List<GitHubReleaseAsset>();

        /// <summary>
        /// Gets the release's id
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the url of the release in the GitHub web interface.
        /// </summary>
        public string HtmlUrl { get; }

        /// <summary>
        /// Gets the name of the tag the release was created for.
        /// </summary>
        public string TagName { get; }

        /// <summary>
        /// Gets the SHA of the git commit the release refers to.
        /// </summary>
        public string TargetCommitish { get; }

        /// <summary>
        /// Gets the release's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the release's description (a Markdown string).
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Gets whether the release is a draft release.
        /// </summary>
        public bool Draft { get; }

        /// <summary>
        /// Gets whether the release was marked as prerelease.
        /// </summary>
        public bool Prerelease { get; }

        /// <summary>
        /// Gets the time the release was created.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Gets the time the release was published.
        /// </summary>
        public DateTime? PublishedAt { get; }

        /// <summary>
        /// Gets the release's assets.
        /// </summary>
        public IReadOnlyList<GitHubReleaseAsset> Assets => _assets;


        /// <summary>
        /// Initializes a new instance of <see cref="GitHubRelease"/>
        /// </summary>
        public GitHubRelease(long id, string htmlUrl, string tagName, string targetCommitish, string name, string body, bool draft, bool prerelease, DateTime createdAt, DateTime? publishedAt)
        {
            Id = id;
            HtmlUrl = htmlUrl;
            TagName = tagName;
            TargetCommitish = targetCommitish;
            Name = name;
            Body = body;
            Draft = draft;
            Prerelease = prerelease;
            CreatedAt = createdAt;
            PublishedAt = publishedAt;
        }


        internal void Add(GitHubReleaseAsset asset) => _assets.Add(asset);

        /// <summary>
        /// Converts a Octokit <see cref="Release" /> to a <see cref="GitHubRelease"/>
        /// </summary>
        internal static GitHubRelease FromRelease(Release release)
        {
            var githubRelease = new GitHubRelease(
                id: release.Id,
                htmlUrl: release.HtmlUrl,
                tagName: release.TagName,
                targetCommitish: release.TargetCommitish,
                name: release.Name,
                body: release.Body,
                draft: release.Draft,
                prerelease: release.Prerelease,
                createdAt: release.CreatedAt.DateTime,
                publishedAt: release.PublishedAt?.DateTime
            );

            foreach (var asset in release.Assets ?? Enumerable.Empty<ReleaseAsset>())
            {
                githubRelease.Add(GitHubReleaseAsset.FromReleaseAsset(asset));
            }

            return githubRelease;
        }
    }
}

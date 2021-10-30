using System;
using Octokit;

namespace Cake.GitHub
{
    /// <summary>
    /// Provides information about a Release asset (a file uploaded as part of a release).
    /// </summary>
    public sealed class GitHubReleaseAsset
    {
        /// <summary>
        /// Gets the asset's id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the asset's file name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the asset's size in bytes.
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// Gets the asset's content type.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the asset's download url.
        /// </summary>
        public string BrowserDownloadUrl { get; }


        public GitHubReleaseAsset(int id, string name, long size, string contentType, string browserDownloadUrl)
        {
            Id = id;
            Name = name;
            Size = size;
            ContentType = contentType;
            BrowserDownloadUrl = browserDownloadUrl;
        }


        /// <summary>
        /// Converts a Octokit <see cref="ReleaseAsset" /> to a <see cref="GitHubReleaseAsset"/>
        /// </summary>
        internal static GitHubReleaseAsset FromReleaseAsset(ReleaseAsset asset)
        {
            if (asset is null)
                throw new ArgumentNullException(nameof(asset));

            return new GitHubReleaseAsset(
                id: asset.Id,
                name: asset.Name,
                size: asset.Size,
                contentType: asset.ContentType,
                browserDownloadUrl: asset.BrowserDownloadUrl
            );
        }
    }
}

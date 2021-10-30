#nullable enable 

using System;
using System.Collections.Generic;
using Cake.Core.IO;

namespace Cake.GitHub
{
    /// <summary>
    /// Settings for creating a GitHub Release
    /// </summary>
    public sealed class GitHubCreateReleaseSettings : GitHubSettings
    {
        /// <summary>
        /// Gets the name of the tag to create a release for.
        /// Tag will be created if it does not yet exist.
        /// </summary>
        public string TagName { get; }

        /// <summary>
        /// Gets or sets the Git commit it to create the tag from.
        /// Can be any branch of commit SHA.
        /// Value is unused if tag specified by <see cref="TagName"/> already exits.
        /// </summary>
        public string? TargetCommitish { get; set; }

        /// <summary>
        /// Gets or sets the name of the release to.
        /// Defaults to <see cref="TagName"/> when not specified.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the release's description
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Gets or sets whether to create a draft release
        /// </summary>
        public bool Draft { get; set; }

        /// <summary>
        /// Gets or sets whether to mark the release as "prerelease"
        /// </summary>
        public bool Prerelease { get; set; }

        /// <summary>
        /// Gets or sets the paths of the files to upload as release assets.
        /// </summary>
        public IList<FilePath> Assets { get; set; } = new List<FilePath>();

        /// <summary>
        /// Gets or sets whether to replace an existing release
        /// </summary>
        /// <remarks>
        /// When set to true, an existing release with the specified tag name will be deleted before a new release is created.
        /// </remarks>
        public bool Overwrite { get; set; }


        internal ICollection<FilePath> AssetsOrEmpty => Assets ?? Array.Empty<FilePath>();

        internal string NameOrTagName => String.IsNullOrEmpty(Name) ? TagName : Name!;


        /// <summary>
        /// Initializes a new instance of <see cref="GitHubCreateReleaseSettings" />
        /// </summary>
        /// <param name="repositoryOwner">The name of the repository's owner on GitHub to create a release for.</param>
        /// <param name="repositoryName">The name of the repository on GitHub to create a release for.</param>
        /// <param name="tagName">The name of the tag to create a release for. Tag will be created if it does not yet exist.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="repositoryOwner"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="repositoryName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="tagName"/> is <c>null</c> or whitespace.</exception>
        public GitHubCreateReleaseSettings(string repositoryOwner, string repositoryName, string tagName) : base(repositoryOwner, repositoryName)
        {
            if (String.IsNullOrWhiteSpace(tagName))
                throw new ArgumentException("Value must not be null or whitespace", nameof(tagName));

            TagName = tagName;
        }
    }
}

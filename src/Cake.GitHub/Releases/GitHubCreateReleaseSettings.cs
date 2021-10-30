#nullable enable 

using System;
using System.Collections.Generic;
using Cake.Core.IO;

namespace Cake.GitHub
{
    /// <summary>
    /// Settings for creating a GitHub Release
    /// </summary>
    public sealed class GitHubCreateReleaseSettings : GitHubSettingsBase
    {
        /// <summary>
        /// Gets or sets the Git commit it to create the tag from.
        /// Can be any branch of commit SHA.
        /// Value is unused if a tag already exists
        /// </summary>
        public string? TargetCommitish { get; set; }

        /// <summary>
        /// Gets or sets the name of the release to.
        /// Defaults to the tag name when not specified.
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
    }
}

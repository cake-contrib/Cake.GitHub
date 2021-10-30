#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Octokit;

namespace Cake.GitHub
{
    internal sealed class GitHubReleaseCreator
    {
        private readonly ICakeLog m_CakeLog;
        private readonly IFileSystem m_FileSystem;
        private readonly IGitHubClientFactory m_ClientFactory;


        public GitHubReleaseCreator(ICakeLog cakeLog, IFileSystem fileSystem, IGitHubClientFactory clientFactory)
        {
            m_CakeLog = cakeLog ?? throw new ArgumentNullException(nameof(cakeLog));
            m_FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            m_ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }


        public async Task<GitHubRelease> CreateRelease(string? userName, string apiToken, string owner, string repository, string tagName, GitHubCreateReleaseSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));
            
            m_CakeLog.Information($"Creating new GitHub Release '{(String.IsNullOrEmpty(settings.Name) ? tagName : settings.Name)}'");

            ValidateSettings(settings);

            LogSettings(owner, repository, tagName, settings);

            var release = await CreateNewRelease(userName: userName, apiToken: apiToken, owner: owner, repository: repository, tagName: tagName, settings: settings);

            return release;
        }


        private void ValidateSettings(GitHubCreateReleaseSettings settings)
        {
            m_CakeLog.Debug("Validating GitHub Release settings");

            var assetNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var filePath in settings.AssetsOrEmpty)
            {
                var file = m_FileSystem.GetFile(filePath);

                if (!file.Exists)
                    throw new FileNotFoundException($"Cannot create GitHub Release with asset '{filePath}' because the file does not exist");

                var fileName = file.Path.GetFilename().ToString();
                if (assetNames.Contains(fileName))
                {
                    throw new AssetConflictException($"Cannot create GitHub release with multiple assets named '{fileName}'");
                }
                assetNames.Add(fileName);
            }
        }

        private void LogSettings(string owner, string repository, string tagName, GitHubCreateReleaseSettings settings)
        {
            const int padding = -16;

            m_CakeLog.Debug("Creating GitHub Release with the following settings:");
            m_CakeLog.Debug($"\t{"Owner",padding}: '{owner}'");
            m_CakeLog.Debug($"\t{"Repository",padding}: '{repository}'");
            m_CakeLog.Debug($"\t{"TagName" ,padding}: '{tagName}'");
            m_CakeLog.Debug($"\t{nameof(settings.TargetCommitish),padding}: '{settings.TargetCommitish}'");
            m_CakeLog.Debug($"\t{nameof(settings.Name),padding}: '{settings.Name}'");
            m_CakeLog.Debug($"\t{nameof(settings.Body),padding}: '{settings.Body}'");
            m_CakeLog.Debug($"\t{nameof(settings.Draft),padding}: {settings.Draft}");
            m_CakeLog.Debug($"\t{nameof(settings.Overwrite),padding}: {settings.Overwrite}");
            m_CakeLog.Debug($"\t{nameof(settings.Prerelease),padding}: {settings.Prerelease}");
            if (settings.AssetsOrEmpty.Any())
            {
                m_CakeLog.Debug($"\t{nameof(settings.Assets),padding}:");
                foreach (var asset in settings.AssetsOrEmpty)
                {
                    m_CakeLog.Debug($"\t\t- {asset}");
                }
            }
        }

        private async Task<GitHubRelease> CreateNewRelease(string userName, string apiToken, string owner, string repository, string tagName, GitHubCreateReleaseSettings settings)
        {
            //TODO: Use CreateApiConnection()
            var client = m_ClientFactory.CreateClient(settings.HostName, apiToken);

            // 
            // Check for existing release and delete if necessary
            //

            var existingRelease = await TryGetReleaseAsync(client: client, owner: owner, repository: repository, tagName: tagName);
            if (existingRelease != null)
            {
                if (settings.Overwrite)
                {
                    await DeleteReleaseAsync(client, owner, repository, existingRelease);
                }
                else
                {
                    throw new ReleaseExistsException($"A release for tag '{tagName}' already exist in repository {owner}/{repository}");
                }
            }

            //
            // Create new release
            //

            m_CakeLog.Verbose("Creating Release");
            var newRelease = new NewRelease(tagName)
            {
                TargetCommitish = settings.TargetCommitish,
                Name = settings.Name,
                Body = settings.Body,
                Draft = settings.Draft,
                Prerelease = settings.Prerelease
            };

            var createdRelease = await client.Repository.Release.Create(owner, repository, newRelease);
            m_CakeLog.Debug($"Created release with id '{createdRelease.Id}'");

            var result = GitHubRelease.FromRelease(createdRelease);

            //
            // Upload assets
            //
            if (settings.AssetsOrEmpty.Count > 0)
            {
                m_CakeLog.Verbose("Uploading Release Assets");
                foreach (var filePath in settings.AssetsOrEmpty)
                {
                    m_CakeLog.Debug($"Uploading asset '{filePath}'");
                    using var stream = m_FileSystem.GetFile(filePath).OpenRead();
                    var assetUpload = new ReleaseAssetUpload()
                    {
                        FileName = filePath.GetFilename().ToString(),
                        ContentType = "application/octet-stream",
                        RawData = stream
                    };
                    var asset = await client.Repository.Release.UploadAsset(createdRelease, assetUpload);
                    result.Add(GitHubReleaseAsset.FromReleaseAsset(asset));
                }
            }

            m_CakeLog.Verbose(settings.AssetsOrEmpty.Count > 0
                ? $"Successfully created new GitHub Release and uploaded '{settings.AssetsOrEmpty.Count}' assets"
                : "Successfully created new GitHub Release");

            return result;
        }

        private async Task<Release?> TryGetReleaseAsync(IGitHubClient client, string owner, string repository, string tagName)
        {
            try
            {
                var release = await client.Repository.Release.Get(owner, repository, tagName);
                m_CakeLog.Verbose($"Found existing release for tag '{tagName}'");
                return release;
            }
            catch (NotFoundException)
            {
                // in case a release is a draft release, the tag has not been created yet and the release cannot be found via Get()
                // to retrieve it, get all releases and search for the tag name                
                var allReleases = await client.Repository.Release.GetAll(owner, repository);
                var matchingReleases = allReleases.Where(x => StringComparer.Ordinal.Equals(x.TagName, tagName)).ToArray();

                Release? release;
                if(matchingReleases.Length == 0)
                {
                    release = null;
                }
                else if(matchingReleases.Length == 1)
                {
                    release = matchingReleases[0];
                }
                else 
                {
                    throw new AmbiguousTagNameException($"There are multiple existing releases for tag name '{tagName}'");
                }
                
                if (release != null)
                    m_CakeLog.Verbose($"Found existing draft release for tag '{tagName}'");

                return release;
            }
        }

        private async Task DeleteReleaseAsync(IGitHubClient client, string owner, string repository, Release release)
        {
            m_CakeLog.Verbose($"Deleting existing release '{release.Name ?? release.TagName}'");
            await client.Repository.Release.Delete(owner, repository, release.Id);

            // For non-draft releases, the tag must be deleted as well
            // Otherwise, when a new release is created with the same tag name, the existing tag will be reused
            // and the new release might point to the wrong commit
            if (!release.Draft)
            {
                m_CakeLog.Debug($"Deleting tag '{release.TagName}'");
                await client.Git.Reference.Delete(owner, repository, $"tags/{release.TagName}");
            }
        }
    }
}


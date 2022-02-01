using Cake.Core.IO;
using Moq;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Cake.GitHub.Tests
{
    /// <summary>
    /// Tests for <see cref="GitHubReleaseCreator"/>
    /// </summary>
    public class GitHubReleaseCreatorTests
    {
        private readonly XunitCakeLog _testLog;
        private readonly GitHubClientMock _clientMock;
        private readonly Mock<IFileSystem> _fileSystemMock;


        public GitHubReleaseCreatorTests(ITestOutputHelper testOutputHelper)
        {
            _testLog = new XunitCakeLog(testOutputHelper);
            _clientMock = new GitHubClientMock();
            _fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
        }

// Disable nullable reference types for xunit data source, otherwise there is a compile error when building in VS 2017
#nullable disable       
        public static IEnumerable<object[]> CreateReleaseTestCases()
        {
            static object[] TestCase(string id, string owner, string repository, string tagName, GitHubCreateReleaseSettings settings, NewRelease expectedRelease, IReadOnlyList<ReleaseAssetUpload> expectedAssetUploads)
            {
                return new object[] { id, owner, repository, tagName, settings, expectedRelease, expectedAssetUploads };
            }

            yield return TestCase(
                "T01",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings(),
                new NewRelease("tagName"),
                Array.Empty<ReleaseAssetUpload>()
            );

            yield return TestCase(
                "T02",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { Name = "ReleaseName" },
                new NewRelease("tagName") { Name = "ReleaseName" },
                Array.Empty<ReleaseAssetUpload>()
            );

            yield return TestCase(
                "T03",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { TargetCommitish = "abc123" },
                new NewRelease("tagName") { TargetCommitish = "abc123" },
                Array.Empty<ReleaseAssetUpload>()
            );

            yield return TestCase(
                "T04",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { Body = "Release Body" },
                new NewRelease("tagName") { Body = "Release Body" },
                Array.Empty<ReleaseAssetUpload>()
            );

            yield return TestCase(
                "T05",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { Draft = false },
                new NewRelease("tagName") { Draft = false },
                Array.Empty<ReleaseAssetUpload>()
            );
            yield return TestCase(
                "T06",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { Draft = true },
                new NewRelease("tagName") { Draft = true },
                Array.Empty<ReleaseAssetUpload>()
            );

            yield return TestCase(
                "T07",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { Prerelease = false },
                new NewRelease("tagName") { Prerelease = false },
                Array.Empty<ReleaseAssetUpload>()
            );
            yield return TestCase(
                "T08",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { Prerelease = true },
                new NewRelease("tagName") { Prerelease = true },
                Array.Empty<ReleaseAssetUpload>()
            );

            // Assets can be null or empty => no upload
            yield return TestCase(
                "T09",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings() { Assets = null! },
                new NewRelease("tagName"),
                Array.Empty<ReleaseAssetUpload>()
            );
            yield return TestCase(
                 "T10",
                 "owner",
                "repo",
                "tagName",
                 new GitHubCreateReleaseSettings() { Assets = Array.Empty<FilePath>() },
                 new NewRelease("tagName"),
                 Array.Empty<ReleaseAssetUpload>()
             );

            yield return TestCase(
                "T11",
                "owner",
                "repo",
                "tagName",
                new GitHubCreateReleaseSettings()
                {
                    Assets = new List<FilePath>()
                    {
                        new FilePath("file1.ext"),
                        new FilePath("dir/file2.ext")
                    }
                },
                new NewRelease("tagName"),
                new[]
                {
                    new ReleaseAssetUpload() { FileName = "file1.ext", ContentType = "application/octet-stream"},
                    new ReleaseAssetUpload() { FileName = "file2.ext", ContentType = "application/octet-stream"},
                }
            );

        }
#nullable enable

        [Theory]
        [InlineData(null, "repo", "tag", "owner")]
        [InlineData("", "repo", "tag", "owner")]
        [InlineData(" ", "repo", "tag", "owner")]
        [InlineData("\t", "repo", "tag", "owner")]
        [InlineData("owner", null, "tag", "repository")]
        [InlineData("owner", "", "tag", "repository")]
        [InlineData("owner", " ", "tag", "repository")]
        [InlineData("owner", "\t", "tag", "repository")]
        [InlineData("owner", "repo", null, "tagName")]
        [InlineData("owner", "repo", "", "tagName")]
        [InlineData("owner", "repo", " ", "tagName")]
        [InlineData("owner", "repo", "\t", "tagName")]
        public async Task CreateReleaseAsync_checks_string_parameters_for_null_or_whitespace(string owner, string repo, string tagName, string expectedParameterName)
        {
            // ARRANGE
            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.CreateReleaseAsync(owner: owner, repository: repo, tagName: tagName, new GitHubCreateReleaseSettings()));

            // ASSERT
            var argumentNullException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal(expectedParameterName, argumentNullException.ParamName);
        }

        [Theory]
        [MemberData(nameof(CreateReleaseTestCases))]
        public async Task CreateReleaseAsync_creates_the_expected_release(string id, string owner, string repository, string tagName, GitHubCreateReleaseSettings settings, NewRelease expectedRelease, IReadOnlyList<ReleaseAssetUpload> expectedAssetUploads)
        {
            // ARRANGE
            _ = id;
            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            var createdRelease = new TestRelease() { Id = 123, TagName = tagName };

            _clientMock.Repository.Release
                .Setup(x => x.Get(owner, repository, It.IsAny<string>()))
                .ThrowsNotFoundAsync();

            _clientMock.Repository.Release
                .Setup(x => x.GetAll(owner, repository))
                .ReturnsEmptyListAsync();

            _clientMock.Repository.Release
                .Setup(x => x.Create(owner, repository, It.IsAny<NewRelease>()))
                .ReturnsAsync(createdRelease);

            _clientMock.Repository.Release
                .Setup(x => x.UploadAsset(createdRelease, It.IsAny<ReleaseAssetUpload>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Release release, ReleaseAssetUpload assetUpload, CancellationToken _) => new TestReleaseAsset() { Name = assetUpload.FileName });

            foreach (var filePath in settings.AssetsOrEmpty)
            {
                _fileSystemMock
                    .Setup(x => x.GetFile(filePath))
                    .Returns((FilePath path) => new FakeFile(path) { Exists = true });
            }

            // ACT 
            var release = await sut.CreateReleaseAsync(owner, repository, tagName, settings);

            // ASSERT
            Assert.NotNull(release);
            Assert.Equal(123, release.Id);

            _clientMock.Repository.Release.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()), Times.Once);
            _clientMock.Repository.Release.Verify(x => x.Create(owner, repository, It.Is<NewRelease>(actual => MatchRelease(expectedRelease, actual))), Times.Once);

            _clientMock.Repository.Release.Verify(x => x.UploadAsset(It.IsAny<Release>(), It.IsAny<ReleaseAssetUpload>(), It.IsAny<CancellationToken>()), Times.Exactly(expectedAssetUploads.Count));
            foreach (var expectedAssetUpload in expectedAssetUploads)
            {
                _clientMock.Repository.Release.Verify(x => x.UploadAsset(createdRelease, It.Is<ReleaseAssetUpload>(actual => MatchReleaseAssetUpload(expectedAssetUpload, actual)), It.IsAny<CancellationToken>()), Times.Once);
            }

            _fileSystemMock.Verify(x => x.GetFile(It.IsAny<FilePath>()), Times.Exactly(expectedAssetUploads.Count * 2));
        }

        [Fact]
        public async Task CreateReleaseAsync_fails_if_asset_to_upload_does_not_exist()
        {
            // ARRANGE
            var assetPath = new FilePath("does-not-exist");

            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            _fileSystemMock
                    .Setup(x => x.GetFile(assetPath))
                    .Returns((FilePath path) => new FakeFile(path) { Exists = false });

            var owner = "owner";
            var repository = "repo";
            var tagName = "tag";
            var settings = new GitHubCreateReleaseSettings()
            {
                Assets = new[] { assetPath }
            };

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.CreateReleaseAsync(owner, repository, tagName, settings));

            // ASSERT
            Assert.IsType<FileNotFoundException>(ex);
        }

        [Fact]
        public async Task CreateReleaseAsync_fails_if_multiple_assets_with_the_same_name_are_added()
        {
            // ARRANGE
            var asset1 = new FilePath("asset1.zip");
            var asset2 = new FilePath("dir/asset1.zip");

            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            _fileSystemMock
                    .Setup(x => x.GetFile(asset1))
                    .Returns((FilePath path) => new FakeFile(path) { Exists = true });

            _fileSystemMock
                    .Setup(x => x.GetFile(asset2))
                    .Returns((FilePath path) => new FakeFile(path) { Exists = true });

            var owner = "owner";
            var repository = "repo";
            var tagName = "tag";
            var settings = new GitHubCreateReleaseSettings()
            {
                Assets = new[] { asset1, asset2 }
            };

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.CreateReleaseAsync(owner, repository, tagName, settings));

            // ASSERT
            Assert.IsType<AssetConflictException>(ex);
            Assert.Equal("Cannot create GitHub release with multiple assets named 'asset1.zip'", ex.Message);
        }

        [Fact]
        public async Task CreateReleaseAsync_throws_ReleaseExistsException_if_a_release_with_the_specified_tag_name_already_exists()
        {
            // ARRANGE
            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            var owner = "owner";
            var repository = "repo";
            var tagName = "tagName";
            var settings = new GitHubCreateReleaseSettings();

            _clientMock.Repository.Release
                .Setup(x => x.Get(owner, repository, tagName))
                .ReturnsAsync(new TestRelease() { Id = 123, TagName = tagName });

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.CreateReleaseAsync(owner, repository, tagName, settings));

            // ASSERT
            Assert.IsType<ReleaseExistsException>(ex);
            _clientMock.Repository.Release.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()), Times.Never);
            _clientMock.Repository.Release.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _clientMock.Repository.Release.Verify(x => x.Get(owner, repository, tagName), Times.Once);
        }

        [Fact]
        public async Task CreateReleaseAsync_throws_ReleaseExistsException_if_a_draf_release_with_the_specified_tag_name_already_exists()
        {
            // ARRANGE
            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            var owner = "owner";
            var repository = "repo";
            var tagName = "tagName";
            var settings = new GitHubCreateReleaseSettings();

            // draft releases cannot be retrieved by tag name but are included when using GetAll()
            _clientMock.Repository.Release
                .Setup(x => x.Get(owner, repository, tagName))
                .ThrowsNotFoundAsync();

            _clientMock.Repository.Release
                .Setup(x => x.GetAll(owner, repository))
                .ReturnsAsync(new[] { new TestRelease() { Id = 123, Draft = true, TagName = tagName } });

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.CreateReleaseAsync(owner, repository, tagName, settings));

            // ASSERT
            Assert.IsType<ReleaseExistsException>(ex);
            _clientMock.Repository.Release.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()), Times.Never);
            _clientMock.Repository.Release.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _clientMock.Repository.Release.Verify(x => x.Get(owner, repository, tagName), Times.Once);

        }


        [Fact]
        public async Task CreateReleaseAsync_deletes_an_existing_release_with_the_same_tag_name_if_overwrite_is_set_to_true()
        {
            // ARRANGE
            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            var owner = "owner";
            var repository = "repo";
            var tagName = "tagName";
            var settings = new GitHubCreateReleaseSettings() { Overwrite = true };

            var existingRelease = new TestRelease() { Id = 123, TagName = tagName };
            var createdRelease = new TestRelease() { Id = 456, TagName = tagName };

            _clientMock.Repository.Release
                .Setup(x => x.Get(owner, repository, tagName))
                .ReturnsAsync(existingRelease);

            _clientMock.Repository.Release
                .Setup(x => x.Delete(owner, repository, It.IsAny<int>()))
                .ReturnsCompletedTask();

            _clientMock.Repository.Release
                .Setup(x => x.Create(owner, repository, It.IsAny<NewRelease>()))
                .ReturnsAsync(createdRelease);

            _clientMock.Git.Reference
                .Setup(x => x.Delete(owner, repository, $"tags/{tagName}"))
                .ReturnsCompletedTask();


            // ACT 
            var release = await sut.CreateReleaseAsync(owner, repository, tagName, settings);

            // ASSERT
            Assert.NotNull(release);
            Assert.Equal(createdRelease.Id, release.Id);
            _clientMock.Repository.Release.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _clientMock.Repository.Release.Verify(x => x.Delete(owner, repository, existingRelease.Id), Times.Once);
            _clientMock.Repository.Release.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()), Times.Once);
            _clientMock.Repository.Release.Verify(x => x.Create(owner, repository, It.Is<NewRelease>(actual => actual.TagName == tagName)), Times.Once);
            _clientMock.Git.Reference.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _clientMock.Git.Reference.Verify(x => x.Delete(owner, repository, $"tags/{tagName}"), Times.Once);
        }

        [Fact]
        public async Task CreateReleaseAsync_deletes_an_existing_draft_release_with_the_same_tag_name_if_overwrite_is_set_to_true()
        {
            // ARRANGE
            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            var owner = "owner";
            var repository = "repo";
            var tagName = "tagName";
            var settings = new GitHubCreateReleaseSettings() { Overwrite = true };

            var existingRelease = new TestRelease() { Id = 123, Draft = true, TagName = tagName };
            var createdRelease = new TestRelease() { Id = 456, TagName = tagName };

            // Draft releases cannot be retrieved via the tag name but are returned by GetAll()
            _clientMock.Repository.Release
                .Setup(x => x.Get(owner, repository, tagName))
                .ThrowsNotFoundAsync();

            _clientMock.Repository.Release
                .Setup(x => x.GetAll(owner, repository))
                .ReturnsAsync(new[] { existingRelease });

            _clientMock.Repository.Release
                .Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsCompletedTask();

            _clientMock.Repository.Release
                .Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()))
                .ReturnsAsync(createdRelease);

            _clientMock.Git.Reference
                .Setup(x => x.Delete(owner, repository, It.IsAny<string>()))
                .ThrowsAsync(new ApiValidationException());

            // ACT 
            var release = await sut.CreateReleaseAsync(owner, repository, tagName, settings);

            // ASSERT
            Assert.NotNull(release);
            Assert.Equal(createdRelease.Id, release.Id);
            _clientMock.Repository.Release.Verify(x => x.Delete("owner", "repo", existingRelease.Id));
            _clientMock.Repository.Release.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewRelease>()), Times.Once);
            // for draft releases, there is no need to delete any tags
            _clientMock.Git.Reference.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [Fact]
        public async Task CreateReleaseAsync_throws_AmbiguousTagNameException_if_multiple_draft_releases_with_the_same_tag_name_exist()
        {
            // ARRANGE
            var sut = new GitHubReleaseCreator(_testLog, _fileSystemMock.Object, _clientMock.Object);

            var owner = "owner";
            var repository = "repo";
            var tagName = "tagName";
            var settings = new GitHubCreateReleaseSettings() { Overwrite = true };

            var existingRelease1 = new TestRelease() { Id = 123, Draft = true, TagName = tagName };
            var existingRelease2 = new TestRelease() { Id = 456, Draft = true, TagName = tagName };

            // Draft releases cannot be retrieved via the tag name but are returned by GetAll()
            _clientMock.Repository.Release
                .Setup(x => x.Get(owner, repository, tagName))
                .ThrowsNotFoundAsync();

            _clientMock.Repository.Release
                .Setup(x => x.GetAll(owner, repository))
                .ReturnsAsync(new[] { existingRelease1, existingRelease2 });

            // ACT 
            var ex = await Record.ExceptionAsync(async () => await sut.CreateReleaseAsync(owner, repository, tagName, settings));

            // ASSERT
            Assert.IsType<AmbiguousTagNameException>(ex);
        }


        private bool MatchRelease(NewRelease expected, NewRelease actual)
        {
            var match = true;

            match &= StringComparer.Ordinal.Equals(expected.TagName, actual.TagName);
            match &= StringComparer.Ordinal.Equals(expected.TargetCommitish, actual.TargetCommitish);
            match &= StringComparer.Ordinal.Equals(expected.Name, actual.Name);
            match &= StringComparer.Ordinal.Equals(expected.Body, actual.Body);
            match &= (expected.Draft == actual.Draft);
            match &= (expected.Prerelease == actual.Prerelease);

            return match;
        }

        private bool MatchReleaseAssetUpload(ReleaseAssetUpload expected, ReleaseAssetUpload actual)
        {
            var match = true;

            match &= StringComparer.Ordinal.Equals(expected.FileName, actual.FileName);
            match &= StringComparer.Ordinal.Equals(expected.ContentType, actual.ContentType);
            match &= StringComparer.Ordinal.Equals(expected.Timeout, actual.Timeout);
            match &= (actual.RawData != null);

            return match;
        }        
    }
}

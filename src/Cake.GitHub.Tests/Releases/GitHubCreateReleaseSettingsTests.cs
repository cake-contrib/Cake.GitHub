using System;
using Xunit;

namespace Cake.GitHub.Tests
{
    /// <summary>
    /// Tests for <see cref="GitHubReleaseCreateSettings"/>
    /// </summary>
    public class GitHubCreateReleaseSettingsTests
    {
        [Theory]
        [InlineData(null, "repo", "tagName")]
        [InlineData("", "repo", "tagName")]
        [InlineData(" ", "repo", "tagName")]
        [InlineData("\t", "repo", "tagName")]
        [InlineData("owner", null, "tagName")]
        [InlineData("owner", "", "tagName")]
        [InlineData("owner", " ", "tagName")]
        [InlineData("owner", "\t", "tagName")]
        [InlineData("owner", "repo", null)]
        [InlineData("owner", "repo", "")]
        [InlineData("owner", "repo", " ")]
        [InlineData("owner", "repo", "\t")]
        public void Constructor_parameter_must_not_be_null_or_whitespace(string owner, string repository, string tagName)
        {
            // ARRANGE

            // ACT
            var ex = Record.Exception(() => new GitHubCreateReleaseSettings(repositoryOwner: owner, repositoryName: repository, tagName: tagName));

            // ASSERT
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void Properties_have_expected_default_values()
        {
            // ARRANGE
            var sut = new GitHubCreateReleaseSettings("owner", "repo", "tagName");

            // ACT

            // ASSERT
            Assert.Equal("owner", sut.RepositoryOwner);
            Assert.Equal("repo", sut.RepositoryName);
            Assert.Equal("tagName", sut.TagName);
            Assert.Null(sut.TargetCommitish);
            Assert.Null(sut.Name);
            Assert.Null(sut.Body);
            Assert.False(sut.Draft);
            Assert.False(sut.Prerelease);
            Assert.False(sut.Overwrite);
            Assert.NotNull(sut.Assets);
            Assert.Empty(sut.Assets);
        }
    }
}

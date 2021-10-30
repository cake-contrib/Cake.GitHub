using Xunit;

namespace Cake.GitHub.Tests
{
    /// <summary>
    /// Tests for <see cref="GitHubCreateReleaseSettings"/>
    /// </summary>
    public class GitHubCreateReleaseSettingsTests
    {

        [Fact]
        public void Properties_have_expected_default_values()
        {
            // ARRANGE
            var sut = new GitHubCreateReleaseSettings();

            // ACT

            // ASSERT
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

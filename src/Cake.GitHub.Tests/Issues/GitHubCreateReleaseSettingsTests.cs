using Xunit;

namespace Cake.GitHub.Tests
{
    /// <summary>
    /// Tests for <see cref="GitHubSetMilestoneSettings"/>
    /// </summary>
    public class GitHubSetMilestoneSettingsTests
    {
        [Fact]
        public void Properties_have_expected_default_values()
        {
            // ARRANGE
            var sut = new GitHubSetMilestoneSettings();

            // ACT

            // ASSERT
            Assert.False(sut.Overwrite);
            Assert.False(sut.CreateMilestone);
        }
    }
}

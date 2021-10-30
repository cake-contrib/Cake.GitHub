using System;
using Octokit;
using Xunit;

namespace Cake.GitHub.Tests
{
    /// <summary>
    /// Tests for <see cref="GitHubClientFactory"/>
    /// </summary>
    public class GitHubClientFactoryTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void HostName_must_not_be_null_or_whitespace(string hostName)
        {
            // ARRANGE
            var sut = new GitHubClientFactory();

            // ACT 
            var ex = Record.Exception(() => sut.CreateClient(hostName: hostName, accessToken: null));

            // ASSERT
            var argumentException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal("hostName", argumentException.ParamName);
        }

        [Fact]
        public void AccessToken_is_optional()
        {
            // ARRANGE
            var sut = new GitHubClientFactory();

            // ACT 
            var client = sut.CreateClient(hostName: "github.com", accessToken: null);

            // ASSERT
            var githubClient = Assert.IsType<GitHubClient>(client);
            Assert.Equal(AuthenticationType.Anonymous, githubClient.Credentials.AuthenticationType);
        }

        [Fact]
        public void AccessToken_is_added_to_the_client_if_specified()
        {
            // ARRANGE
            var accessToken = "my-access-token";
            var sut = new GitHubClientFactory();

            // ACT 
            var client = sut.CreateClient("github.com", accessToken);

            // ASSERT
            var githubClient = Assert.IsType<GitHubClient>(client);
            Assert.Equal(AuthenticationType.Oauth, githubClient.Credentials.AuthenticationType);
            Assert.Null(githubClient.Credentials.Login);
            Assert.Equal(accessToken, githubClient.Credentials.Password);
        }
    }
}

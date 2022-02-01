using Moq;
using Octokit;

namespace Cake.GitHub.Tests
{
    internal class GitHubClientMock
    {
        public class RepositoriesClientMock
        {
            private readonly Mock<IRepositoriesClient> _mock = new Mock<IRepositoriesClient>(MockBehavior.Strict);

            public IRepositoriesClient Object => _mock.Object;

            public Mock<IReleasesClient> Release { get; } = new Mock<IReleasesClient>(MockBehavior.Strict);


            public RepositoriesClientMock()
            {
                _mock.Setup(x => x.Release).Returns(Release.Object);

            }
        }

        public class GitDatabaseClientMock
        {
            private readonly Mock<IGitDatabaseClient> _mock = new Mock<IGitDatabaseClient>(MockBehavior.Strict);


            public IGitDatabaseClient Object => _mock.Object;


            public Mock<IReferencesClient> Reference { get; } = new Mock<IReferencesClient>(MockBehavior.Strict);


            public GitDatabaseClientMock()
            {
                _mock.Setup(x => x.Reference).Returns(Reference.Object);
            }
        }

        private readonly Mock<IGitHubClient> _mock = new Mock<IGitHubClient>(MockBehavior.Strict);


        public IGitHubClient Object => _mock.Object;


        public RepositoriesClientMock Repository { get; } = new RepositoriesClientMock();

        public GitDatabaseClientMock Git { get; } = new GitDatabaseClientMock();


        public GitHubClientMock()
        {
            _mock.Setup(x => x.Repository).Returns(Repository.Object);
            _mock.Setup(x => x.Git).Returns(Git.Object);
        }
    }
}

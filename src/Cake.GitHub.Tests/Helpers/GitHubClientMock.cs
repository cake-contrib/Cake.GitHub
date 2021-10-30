using Moq;
using Octokit;

namespace Cake.GitHub.Tests
{
    internal class GitHubClientMock
    {
        public class RepositoriesClientMock
        {
            private readonly Mock<IRepositoriesClient> m_Mock = new Mock<IRepositoriesClient>(MockBehavior.Strict);

            public IRepositoriesClient Object => m_Mock.Object;

            public Mock<IReleasesClient> Release { get; } = new Mock<IReleasesClient>(MockBehavior.Strict);


            public RepositoriesClientMock()
            {
                m_Mock.Setup(x => x.Release).Returns(Release.Object);

            }
        }

        public class GitDatabaseClientMock
        {
            private readonly Mock<IGitDatabaseClient> m_Mock = new Mock<IGitDatabaseClient>(MockBehavior.Strict);


            public IGitDatabaseClient Object => m_Mock.Object;


            public Mock<IReferencesClient> Reference { get; } = new Mock<IReferencesClient>(MockBehavior.Strict);


            public GitDatabaseClientMock()
            {
                m_Mock.Setup(x => x.Reference).Returns(Reference.Object);
            }
        }

        private readonly Mock<IGitHubClient> m_Mock = new Mock<IGitHubClient>(MockBehavior.Strict);


        public IGitHubClient Object => m_Mock.Object;


        public RepositoriesClientMock Repository { get; } = new RepositoriesClientMock();

        public GitDatabaseClientMock Git { get; } = new GitDatabaseClientMock();


        public GitHubClientMock()
        {
            m_Mock.Setup(x => x.Repository).Returns(Repository.Object);
            m_Mock.Setup(x => x.Git).Returns(Git.Object);
        }
    }
}

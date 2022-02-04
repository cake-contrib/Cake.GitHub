using Octokit;

namespace Cake.GitHub.Tests
{
    internal class TestRelease : Release
    {
        public new int Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        public new bool Draft
        {
            get => base.Draft;
            set => base.Draft = value;
        }

        public new string TagName
        {
            get => base.TagName;
            set => base.TagName = value;
        }
    }
}

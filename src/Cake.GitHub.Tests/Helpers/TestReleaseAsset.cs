using Octokit;

namespace Cake.GitHub.Tests
{
    public class TestReleaseAsset : ReleaseAsset
    {
        public new string Name
        {
            get => base.Name;
            set => base.Name = value;
        }
    }
}

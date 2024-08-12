using System;
using System.IO;
using Cake.Core.IO;

namespace Cake.GitHub.Tests
{
    internal class FakeFile : IFile
    {
        public FilePath Path { get; set; }

        public long Length
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public FileAttributes Attributes
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool Exists { get; set; }

        public bool Hidden => throw new NotImplementedException();

        Core.IO.Path IFileSystemInfo.Path => Path;


        public FakeFile(FilePath path)
        {
            Path = path;
        }

        public void Copy(FilePath destination, bool overwrite) => throw new NotImplementedException();

        public void Delete() => throw new NotImplementedException();

        public void Move(FilePath destination) => throw new NotImplementedException();

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare) => new MemoryStream();
    }
}

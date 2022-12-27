namespace BlackSugar.Model
{
    public class FakeFileData : IFileData
    {
        public FileAttributes Attributes { get; set; }  

        public string Name {get; set;}

        public string FullName {get; set;}

        public DateTime CreationTime {get; set;}

        public DateTime LastAccesTime {get; set;}

        public DateTime LastWriteTime {get; set;}

        public string TypeName {get; set;}

        public long Length {get; set;}

        public bool IsFile {get; set;}

        public bool IsDirectory {get; set;}

        public bool IsDrive {get; set;}

        public DateTime LastWriteTimeUtc { get; set; }

        public ExFileAttributes ExAttributes { get; set; }
    }
}
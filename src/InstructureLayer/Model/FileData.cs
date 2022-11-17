using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BlackSugar.WinApi;

namespace BlackSugar.Model
{
    public interface IFileData
    {
        FileAttributes Attributes { get; }
        string Name { get; }
        string FullName { get; }
        //DateTime CreationTime { get; }
        //DateTime LastAccessTime { get; }
        DateTime LastWriteTimeUtc { get; }
        string TypeName { get; }
        long Length { get; }
        bool IsFile { get; }
        bool IsDirectory { get; }
        bool IsDrive { get; }
        ExFileAttributes ExAttributes { get;}

    }

    public class FileData : IFileData
    {
        public FileAttributes Attributes { get; }
        public bool IsFile => (Attributes & FileAttributes.Directory) == 0;
        public bool IsDirectory => (Attributes & FileAttributes.Directory) != 0;
        //public DateTime CreationTimeUtc { get; }
        //public DateTime CreationTime => CreationTimeUtc.ToLocalTime();
        //public DateTime LastAccessTimeUtc { get; }
        //public DateTime LastAccessTime => LastAccessTimeUtc.ToLocalTime();
        public DateTime LastWriteTimeUtc { get; }
        public DateTime LastWriteTime => LastWriteTimeUtc.ToLocalTime();
        public long Length { get; }
        public string Name { get; }
        public string FullName { get; }
        public string TypeName { get; }
        public bool IsDrive => (ExAttributes & ExFileAttributes.Drive) == 0;
        public ExFileAttributes ExAttributes { get; }

        internal FileData(ref string fullName, ref DirectoryUtil.NativeMethods.WIN32_FIND_DATA findData)
        {
            Attributes = findData.dwFileAttributes;
            //CreationTimeUtc = findData.ToCreationTimeUtc;
            //LastAccessTimeUtc = findData.ToLastAccessTimeUtc;
            LastWriteTimeUtc = findData.ToLastWriteTimeUtc;
            Length = ((long)findData.nFileSizeHigh << 32) + findData.nFileSizeLow;
            Name = findData.cFileName;
            FullName = fullName;
            TypeName = FileUtil.GetFileType(fullName) ?? string.Empty;
            ExAttributes = ExFileAttributes.None;
        }

        internal FileData(ref string fullName, NetShare.NativeMethods.SHARE_INFO_1 shareInfo, string server)
        {
            Attributes = File.GetAttributes(fullName);
            Name = shareInfo.shi1_netname + "(" + server.TrimEnd('\\') + ")";
            FullName = fullName;
            TypeName = string.Empty;
            ExAttributes = ExFileAttributes.NetShare;
        }

        internal FileData(string fullName, FileUtil.NativeMethods.SHFILEINFO fileInfo, bool server = false)
        {
            Attributes = server ? FileAttributes.Directory : File.GetAttributes(fullName);
            Name = fileInfo.szDisplayName;
            FullName = fullName;
            TypeName = fileInfo.szTypeName;
            ExAttributes = server ? ExFileAttributes.Server : ExFileAttributes.None;
        }

        internal FileData(DriveInfo drive)
        {
            Attributes = drive.RootDirectory.Attributes;
            FullName = drive.RootDirectory.FullName;
            Name = drive.VolumeLabel + "(" + drive.Name.TrimEnd('\\') + ")";
            //IsDrive = true;
            TypeName = string.Empty;
            ExAttributes = ExFileAttributes.Drive;
        }

        public override string ToString() => FullName;
    }

    public enum ExFileAttributes
    {
        Undefined = -1,
        None = 1,
        Drive,
        SpecsialFolder,
        NetShare,
        Server
    }

}

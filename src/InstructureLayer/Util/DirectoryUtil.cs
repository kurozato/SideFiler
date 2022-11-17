using System.Collections.Generic;
using System.IO;
using BlackSugar.Model;

namespace BlackSugar.WinApi
{
    public static partial class DirectoryUtil
    {
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumerateFullName(path, searchPattern, searchOption, true, false);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumerateFullName(path, searchPattern, searchOption, false, true);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumerateFullName(path, searchPattern, searchOption, true, true);
        }

        public static IEnumerable<IFileData> EnumerateFilesData(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumerateFileData(path, searchPattern, searchOption, true, false);
        }

        public static IEnumerable<IFileData> EnumerateDirectoriesData(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumerateFileData(path, searchPattern, searchOption, false, true);
        }

        public static IEnumerable<IFileData> EnumerateFileSystemEntriesData(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumerateFileData(path, searchPattern, searchOption, true, true);
        }

        public static IEnumerable<IFileData> EnumerateDriveData()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                    yield return new FileData(drive);
            }
        }

        public static IEnumerable<IFileData?> EnumerateSpecsialFolderData()
        {

            yield return GetSpecialFolder(Environment.SpecialFolder.Desktop);
            yield return GetSpecialFolder(Environment.SpecialFolder.MyDocuments);
            yield return GetSpecialFolder(Environment.SpecialFolder.MyPictures);
            yield return GetSpecialFolder(Environment.SpecialFolder.MyVideos);
            yield return GetSpecialFolder(Environment.SpecialFolder.MyMusic);
            yield return FileUtil.Create(FileUtil.GetDownloadsFolderPath());
        }

        private static IFileData GetSpecialFolder(Environment.SpecialFolder specialFolder)
        {
            return FileUtil.Create(Environment.GetFolderPath(specialFolder));
        }


    }
        
}

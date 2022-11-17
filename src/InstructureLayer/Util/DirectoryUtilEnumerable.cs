using BlackSugar.Model;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace BlackSugar.WinApi
{
    public static partial class DirectoryUtil
    {
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern SafeFindFileHandle FindFirstFileEx(
                string lpFileName,
                FINDEX_INFO_LEVELS fInfoLevelId,
                out WIN32_FIND_DATA lpFindFileData,
                FINDEX_SEARCH_OPS fSearchOp,
                IntPtr lpSearchFilter,
                FIND_FIRST_EX dwAdditionalFlags);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, BestFitMapping = false)]
            internal static extern bool FindNextFile(SafeFindFileHandle hFindFile, out WIN32_FIND_DATA
                lpFindFileData);

            [DllImport("kernel32.dll")]
            internal static extern bool FindClose(IntPtr handle);

            internal enum FINDEX_INFO_LEVELS
            {
                Standard = 0,
                Basic = 1
            }

            internal enum FINDEX_SEARCH_OPS
            {
                SearchNameMatch = 0,
                SearchLimitToDirectories = 1,
                SearchLimitToDevices = 2
            }

            internal enum FIND_FIRST_EX
            {
                CaseSensitive = 1,
                LargeFetch = 2
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            [BestFitMapping(false)]
            internal struct WIN32_FIND_DATA
            {
                public FileAttributes dwFileAttributes;
                public FILE_TIME ftCreationTime;
                public FILE_TIME ftLastAccessTime;
                public FILE_TIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
                public uint dwReserved0;
                public uint dwReserved1;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                public string cAlternateFileName;

                internal bool IsRelative => cFileName == "." || cFileName == "..";
                internal bool IsFile => (dwFileAttributes & FileAttributes.Directory) == 0;
                internal bool IsDirectory => (dwFileAttributes & FileAttributes.Directory) != 0;

                internal DateTime ToCreationTimeUtc => DateTime.FromFileTimeUtc(ftCreationTime.ToTicks());
                internal DateTime ToLastAccessTimeUtc => DateTime.FromFileTimeUtc(ftLastAccessTime.ToTicks());
                internal DateTime ToLastWriteTimeUtc => DateTime.FromFileTimeUtc(ftLastWriteTime.ToTicks());

                public override string ToString() => cFileName;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct FILE_TIME
            {
                public FILE_TIME(long fileTime)
                {
                    ftTimeLow = (uint)fileTime;
                    ftTimeHigh = (uint)(fileTime >> 32);
                }

                public long ToTicks()
                {
                    return ((long)ftTimeHigh << 32) + ftTimeLow;
                }

                internal uint ftTimeLow;
                internal uint ftTimeHigh;
            }

            internal sealed class SafeFindFileHandle : SafeHandleZeroOrMinusOneIsInvalid
            {
                internal SafeFindFileHandle() : base(true)
                {
                }

                protected override bool ReleaseHandle()
                {
                    return FindClose(handle);
                }
            }
        }

        private interface ISelector<T>
        {
            T Create(ref string fullName, ref NativeMethods.WIN32_FIND_DATA findData);
        }

        private class FullNameSelector : ISelector<string>
        {
            public string Create(ref string fullName, ref NativeMethods.WIN32_FIND_DATA findData) => fullName;
        }

        private class FileDataSelector : ISelector<FileData>
        {
            public FileData Create(ref string fullName, ref NativeMethods.WIN32_FIND_DATA findData) => new FileData(ref fullName, ref findData);
        }

        private static IEnumerable<string> EnumerateFullName(string path, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs)
        {
            return Enumerate(path, searchPattern, searchOption, includeFiles, includeDirs, new FullNameSelector());
        }

        private static IEnumerable<FileData> EnumerateFileData(string path, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs)
        {
            return Enumerate(path, searchPattern, searchOption, includeFiles, includeDirs, new FileDataSelector());
        }

        private static IEnumerable<T> Enumerate<T>(string path, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs, ISelector<T> selector)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (searchPattern == null)
                throw new ArgumentNullException(nameof(searchPattern));
            if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
                throw new ArgumentOutOfRangeException(nameof(searchOption));

            return EnumerateCore(Path.GetFullPath(path).TrimEnd('\\'), searchPattern, searchOption, includeFiles, includeDirs, selector);
        }

        private static IEnumerable<T> EnumerateCore<T>(string dir, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs, ISelector<T> selector)
        {
            // extend MAX_PATH
            var search = (dir.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase)
                                ? @"\\?\UNC\" + dir.Substring(2)
                                : @"\\?\" + dir) + @"\" + searchPattern;

            Queue<string>? subDirs = null;

            using (var fileHandle = NativeMethods.FindFirstFileEx(search,
                                                                  NativeMethods.FINDEX_INFO_LEVELS.Basic,
                                                                  out var findData,
                                                                  NativeMethods.FINDEX_SEARCH_OPS.SearchNameMatch,
                                                                  IntPtr.Zero,
                                                                  NativeMethods.FIND_FIRST_EX.LargeFetch))
            {
                if (fileHandle.IsInvalid) yield break;

                do
                {
                    if (findData.IsRelative) continue;

                    var path = dir + @"\" + findData.cFileName;

                    if (findData.IsFile)
                    {
                        if (includeFiles)
                            yield return selector.Create(ref path, ref findData);
                    }
                    else if (findData.IsDirectory)
                    {
                        if (includeDirs)
                            yield return selector.Create(ref path, ref findData);

                        if (searchOption == SearchOption.AllDirectories)
                        {
                            subDirs = subDirs ?? new Queue<string>();
                            subDirs.Enqueue(path);
                        }
                    }

                } while (NativeMethods.FindNextFile(fileHandle, out findData));
            }

            if (subDirs == null) yield break;

            while (subDirs.Count > 0)
            {
                foreach (var path in EnumerateCore(subDirs.Dequeue(), searchPattern, searchOption, includeFiles, includeDirs, selector))
                    yield return path;
            }
        }
    }
}

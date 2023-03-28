using BlackSugar.Model;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;


namespace BlackSugar.WinApi
{
    public static class FileUtil
    {
        internal static class NativeMethods
        {
            public enum FileFuncFlags : uint
            {
                FO_MOVE = 0x1,
                FO_COPY = 0x2,
                FO_DELETE = 0x3,
                FO_RENAME = 0x4
            }

            [Flags]
            public enum FILEOP_FLAGS : ushort
            {
                FOF_NONE = 0x0,

                FOF_MULTIDESTFILES = 0x1,
                FOF_CONFIRMMOUSE = 0x2,
                /// <summary>
                /// Don't create progress/report
                /// </summary>
                FOF_SILENT = 0x4,
                FOF_RENAMEONCOLLISION = 0x8,
                /// <summary>
                /// Don't prompt the user.
                /// </summary>
                FOF_NOCONFIRMATION = 0x10,
                /// <summary>
                /// Fill in SHFILEOPSTRUCT.hNameMappings.
                /// Must be freed using SHFreeNameMappings
                /// </summary>
                FOF_WANTMAPPINGHANDLE = 0x20,
                FOF_ALLOWUNDO = 0x40,
                /// <summary>
                /// On *.*, do only files
                /// </summary>
                FOF_FILESONLY = 0x80,
                /// <summary>
                /// Don't show names of files
                /// </summary>
                FOF_SIMPLEPROGRESS = 0x100,
                /// <summary>
                /// Don't confirm making any needed dirs
                /// </summary>
                FOF_NOCONFIRMMKDIR = 0x200,
                /// <summary>
                /// Don't put up error UI
                /// </summary>
                FOF_NOERRORUI = 0x400,
                /// <summary>
                /// Dont copy NT file Security Attributes
                /// </summary>
                FOF_NOCOPYSECURITYATTRIBS = 0x800,
                /// <summary>
                /// Don't recurse into directories.
                /// </summary>
                FOF_NORECURSION = 0x1000,
                /// <summary>
                /// Don't operate on connected elements.
                /// </summary>
                FOF_NO_CONNECTED_ELEMENTS = 0x2000,
                /// <summary>
                /// During delete operation, 
                /// warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
                /// </summary>
                FOF_WANTNUKEWARNING = 0x4000,
                /// <summary>
                /// Treat reparse points as objects, not containers
                /// </summary>
                FOF_NORECURSEREPARSE = 0x8000
            }

            //[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
            //If you use the above you may encounter an invalid memory access exception (when using ANSI
            //or see nothing (when using unicode) when you use FOF_SIMPLEPROGRESS flag.
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct SHFILEOPSTRUCT
            {
                public IntPtr hwnd;
                public FileFuncFlags wFunc;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pFrom;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pTo;
                public FILEOP_FLAGS fFlags;
                [MarshalAs(UnmanagedType.Bool)]
                public bool fAnyOperationsAborted;
                public IntPtr hNameMappings;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string lpszProgressTitle;
            }

            [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
            internal static extern int SHFileOperation([In] ref SHFILEOPSTRUCT lpFileOp);


            //************************************************************//

            [DllImport("shell32.dll", CharSet = CharSet.Unicode, BestFitMapping = false)]
            internal static extern int SHGetFileInfo(
                string pszPath,
                int dwFileAttributes,
                out SHFILEINFO psfi,
                uint cbfileInfo,
                SHGFI uFlags);

            /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
            private const int MAX_PATH = 260;
            /// <summary>Maximal Length of unmanaged Typename</summary>
            private const int MAX_TYPE = 80;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            [BestFitMapping(false)]
            internal struct SHFILEINFO
            {
                //public SHFILEINFO(bool b)
                //{
                //    hIcon = IntPtr.Zero;
                //    iIcon = 0;
                //    dwAttributes = 0;
                //    szDisplayName = "";
                //    szTypeName = "";
                //}
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
                public string szTypeName;
            };

     

            [Flags]
            internal enum SHGFI : int
            {
                /// <summary>get icon</summary>
                Icon = 0x000000100,
                /// <summary>get display name</summary>
                DisplayName = 0x000000200,
                /// <summary>get type name</summary>
                TypeName = 0x000000400,
                /// <summary>get attributes</summary>
                Attributes = 0x000000800,
                /// <summary>get icon location</summary>
                IconLocation = 0x000001000,
                /// <summary>return exe type</summary>
                ExeType = 0x000002000,
                /// <summary>get system icon index</summary>
                SysIconIndex = 0x000004000,
                /// <summary>put a link overlay on icon</summary>
                LinkOverlay = 0x000008000,
                /// <summary>show icon in selected state</summary>
                Selected = 0x000010000,
                /// <summary>get only specified attributes</summary>
                Attr_Specified = 0x000020000,
                /// <summary>get large icon</summary>
                LargeIcon = 0x000000000,
                /// <summary>get small icon</summary>
                SmallIcon = 0x000000001,
                /// <summary>get open icon</summary>
                OpenIcon = 0x000000002,
                /// <summary>get shell size icon</summary>
                ShellIconSize = 0x000000004,
                /// <summary>pszPath is a pidl</summary>
                PIDL = 0x000000008,
                /// <summary>use passed dwFileAttribute</summary>
                UseFileAttributes = 0x000000010,
                /// <summary>apply the appropriate overlays</summary>
                AddOverlays = 0x000000020,
                /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
                OverlayIndex = 0x000000040,
            }

            //************************************************************//

            internal static class KnownFolder
            {
                public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
            }

            [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
            internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

            [DllImport("user32.dll")]
            internal static extern long DestroyIcon(IntPtr hIcon);
        }

        private const string NullZero = "\0";


        internal static void FileOperateCore(List<string> pFroms, string? pTo, NativeMethods.FileFuncFlags funcFlags, NativeMethods.FILEOP_FLAGS fileOpFlags, IntPtr handle)
        {
            string pFrom = string.Empty;
            foreach (string file in pFroms)
                pFrom += file + NullZero;

            NativeMethods.SHFILEOPSTRUCT shfos;
            shfos.hwnd = handle;
            shfos.wFunc = funcFlags;
            shfos.pFrom = pFrom + NullZero;
            shfos.pTo = (pTo == null) ? string.Empty : pTo + NullZero + NullZero;
            
            shfos.fFlags = NativeMethods.FILEOP_FLAGS.FOF_ALLOWUNDO | fileOpFlags;
            shfos.fAnyOperationsAborted = true;
            shfos.hNameMappings = IntPtr.Zero;
            shfos.lpszProgressTitle = string.Empty;

            NativeMethods.SHFileOperation(ref shfos);
        }

        public static void Copy(List<string> targets, string toFolder, IntPtr handle)
        {
            FileOperateCore(targets, toFolder, NativeMethods.FileFuncFlags.FO_COPY, NativeMethods.FILEOP_FLAGS.FOF_RENAMEONCOLLISION, handle);
        }

        public static void Move(List<string> targets, string toFolder, IntPtr handle)
        {
            FileOperateCore(targets, toFolder, NativeMethods.FileFuncFlags.FO_MOVE, NativeMethods.FILEOP_FLAGS.FOF_NONE, handle);
        }

        public static void Delete(List<string> targets, IntPtr handle)
        {
            FileOperateCore(targets, null, NativeMethods.FileFuncFlags.FO_DELETE, NativeMethods.FILEOP_FLAGS.FOF_NONE, handle);
        }

        public static void Rename(List<string> targets, string name, IntPtr handle)
        {
            FileOperateCore(targets, name, NativeMethods.FileFuncFlags.FO_RENAME, NativeMethods.FILEOP_FLAGS.FOF_NONE, handle);
        }

        //************************************************************//


        public static string? GetFileType(string? path)
        {
            if (path == null) return null;

            var info = new NativeMethods.SHFILEINFO();
            int cbFileInfo = Marshal.SizeOf(info);
            var result = NativeMethods.SHGetFileInfo(path, 0, out info, (uint)cbFileInfo, NativeMethods.SHGFI.TypeName);
            
            if (result == (int)IntPtr.Zero) return null;

            return info.szTypeName;
        }

        public static FileData? Create(string path, bool server = false)
        {
            var info = new NativeMethods.SHFILEINFO();
            int cbFileInfo = Marshal.SizeOf(info);
            var result = NativeMethods.SHGetFileInfo(path, 0, out info, (uint)cbFileInfo, NativeMethods.SHGFI.TypeName | NativeMethods.SHGFI.DisplayName);

            if (result == (int)IntPtr.Zero) return null;

            

            return new FileData(path, info, server);
        }

        public static string GetDownloadsFolderPath()
        {
            string downloads;
            NativeMethods.SHGetKnownFolderPath(NativeMethods.KnownFolder.Downloads, 0, IntPtr.Zero, out downloads);
            return downloads;
        }

        public static ExFileAttributes GetExFileAttributes(string? path, bool server = false)
        {
            if (path == null) return ExFileAttributes.Undefined;

            if (server) return ExFileAttributes.Server;

            if (IsSystemFolder(path)) return ExFileAttributes.SpecsialFolder;

            return ExFileAttributes.None;

        }

        internal static bool IsSystemFolder(string path)
        {
            foreach (Environment.SpecialFolder spFolder in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                if(Environment.GetFolderPath(spFolder) == path)
                    return true;
            }
            if (GetDownloadsFolderPath() == path)
                return true;

            return false;
        }

        public static IntPtr GetSHSmallIconHandle(string path)
        {
            var info = new NativeMethods.SHFILEINFO();
            int cbFileInfo = Marshal.SizeOf(info);
            var result = NativeMethods.SHGetFileInfo(path, 0, out info, (uint)cbFileInfo, NativeMethods.SHGFI.Icon | NativeMethods.SHGFI.SmallIcon);

            if (result == (int)IntPtr.Zero) return IntPtr.Zero;

            return info.hIcon;
        }

        public static void DestroyIcon(IntPtr hIcon)
        {
            NativeMethods.DestroyIcon(hIcon);
        }
    }
}

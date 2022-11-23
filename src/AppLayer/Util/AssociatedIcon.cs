using BlackSugar.Model;
using MaterialDesignThemes.Wpf.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BlackSugar.WinApi
{
    public class AssociatedIcon
    {
        private class NativeMethods
        {
            [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon")]
            internal static extern IntPtr ExtractAssociatedIcon(
              IntPtr hInst,
              [MarshalAs(UnmanagedType.LPStr)] string lpIconPath,
              ref int lpiIcon
            );

            [DllImport("user32.dll")]
            internal static extern long DestroyIcon(IntPtr hIcon);
        }

        public static BitmapSource Create(string path)
        {
            IntPtr hInst = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
            int lpiIcon = 0;

            IntPtr hIcon = NativeMethods.ExtractAssociatedIcon(hInst, path, ref lpiIcon);

            try
            {
                var icon = Icon.FromHandle(hIcon);
                var source = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
            finally
            {
                NativeMethods.DestroyIcon(hIcon);
            }
        }


        public static BitmapSource Create(string path, IntPtr hInst)
        {
            int lpiIcon = 0;

            IntPtr hIcon = NativeMethods.ExtractAssociatedIcon(hInst, path, ref lpiIcon);

            try
            {
                var icon = Icon.FromHandle(hIcon);
                var source = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
            finally
            {
                NativeMethods.DestroyIcon(hIcon);
            }
        }

        private static Dictionary<string, BitmapSource> _icons = new Dictionary<string, BitmapSource>();
        private static string[] _ignoreExt = new string[] { ".EXE", ".LNK" };

        public const string KEY_FOLDER = "????";

        public static bool Contains(string extension, FileAttributes attributes)
        {
            if (attributes.HasFlag(FileAttributes.Directory))
                return true;

            return (!_ignoreExt.Any(i => i == extension?.ToUpper()) && _icons.ContainsKey(extension));
        }

        public static BitmapSource GetCacheSource(string extension, FileAttributes attributes)
        {
            if (attributes.HasFlag(FileAttributes.Directory))
                return _icons[KEY_FOLDER];

            return _icons[extension];
        }

        public static void SetCacheSource(string extension, BitmapSource source)
        {
            if (!_ignoreExt.Any(i => i == extension?.ToUpper()))
                _icons.Add(extension, source);
        }
              
       public static BitmapSource GetFolderSource(Bitmap folderIcon)
        {
            var hBitmap = folderIcon.GetHbitmap();
            var source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
           
            source.Freeze();
            return source;
        }

        public static void Clear()
        {
            var foler = _icons[KEY_FOLDER];
            _icons.Clear();
            _icons.Add("", foler);
        }

        //public static BitmapSource CreateZ(string path)
        //{
        //    var ext = Path.GetExtension(path);
        //    if(!_ignoreExt.Any(i => i == ext?.ToUpper()) && _icons.ContainsKey(ext))
        //        return _icons[ext];

        //    return Create(path);

        //}
    }
}

using BlackSugar.Model;
using BlackSugar.WinApi;
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

namespace BlackSugar.Views
{
    public class FileIcon
    {
        private static Dictionary<string, BitmapSource> _icons = new Dictionary<string, BitmapSource>();
        private static readonly string[] _ignoreExt = new string[] { ".EXE", ".LNK" };

        private static void Save(BitmapSource source, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);
            }
        }

        private static BitmapSource GetBitmapSource(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        public const string KEY_FOLDER = "????";

        public static BitmapSource Create(string path)
        {
            using(var exIcon = new ExtraIcon(path))
            {
                var source = Imaging.CreateBitmapSourceFromHIcon(exIcon.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
        }

        public static BitmapSource GetFolderSource(Bitmap folderIcon)
        {
            var hBitmap = folderIcon.GetHbitmap();
            var source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            source.Freeze();
            return source;
        }

        public static bool Contains(string extension, FileAttributes attributes)
        {
            if (attributes.HasFlag(FileAttributes.Directory))
                return true;

            return (!_ignoreExt.Any(i => i == extension?.ToUpper()) && _icons.ContainsKey(extension?.ToUpper()));
        }

        public static BitmapSource GetCacheSource(string extension, FileAttributes attributes)
        {
            if (attributes.HasFlag(FileAttributes.Directory))
                return _icons[KEY_FOLDER];

            return _icons[extension.ToUpper()];
        }

        public static void SetCacheSource(string extension, BitmapSource source)
        {
            if (!_ignoreExt.Any(i => i == extension?.ToUpper()))
                _icons.Add(extension.ToUpper(), source);
        }

        public static void Clear()
        {
            var foler = _icons[KEY_FOLDER];
            _icons.Clear();
            _icons.Add(KEY_FOLDER, foler);
        }

        public static void Save()
        {
            var items = new Dictionary<string, BitmapSource>(_icons);
            items.Remove(KEY_FOLDER);
            foreach(var ignore in _ignoreExt)
                items.Remove(ignore);

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var item in items)
            {
                Save(item.Value, Path.Combine(baseDir, "icon",  item.Key.Replace(".", "ICON_") + ".exim"));
            }
        }

        public static void Read()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            foreach (var path in DirectoryUtil.EnumerateDirectories(Path.Combine(baseDir, "icon"), "*.exim"))
            {
                _icons.Add(path.Replace(baseDir, "").Replace("ICON_", "."), GetBitmapSource(path));
            }
        }
    }
}

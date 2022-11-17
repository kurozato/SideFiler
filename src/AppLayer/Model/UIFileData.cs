using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace BlackSugar.Model
{
    public class UIFileData
    {
        //for View Binding
        public BitmapSource? Icon { get; } //=> BlackSugar.WinApi.AssociatedIcon.CreateZ(FullName);
        public string Size { get; } //=> Length == 0 ? "" : Math.Ceiling(Length / 1024.0).ToString("#,### KB");
        public string LastWriteTimeSt { get; } //=> LastWriteTime == DateTime.MinValue.ToLocalTime() ? "" : LastWriteTime.ToString("yyyy/MM/dd HH:mm");

        //public FileAttributes Attributes { get; }
        //public bool IsFile => (Attributes & FileAttributes.Directory) == 0;
        //public bool IsDirectory => (Attributes & FileAttributes.Directory) != 0;
        //public DateTime CreationTimeUtc { get; }
        //public DateTime CreationTime => CreationTimeUtc.ToLocalTime();
        //public DateTime LastAccessTimeUtc { get; }
        //public DateTime LastAccessTime => LastAccessTimeUtc.ToLocalTime();
        //public DateTime LastWriteTimeUtc { get; }
        //public DateTime LastWriteTime => LastWriteTimeUtc.ToLocalTime();
        //public long Length { get; }
        public string Name { get; }
        public string FullName { get; }
        public string TypeName { get; }

        //public bool IsDrive => (ExAttributes & ExFileAttributes.Drive) == 0;
        public ExFileAttributes ExAttributes { get; }

        public UIFileData(IFileData file)
        {
            Icon = null;//BlackSugar.WinApi.AssociatedIcon.Create(file.FullName);
            Name = file.Name;
            LastWriteTimeSt = file.LastWriteTimeUtc.ToLocalTime() == DateTime.MinValue.ToLocalTime() ? "" : file.LastWriteTimeUtc.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
            TypeName = file.TypeName;
            Size = file.Length == 0 ? "" : Math.Ceiling(file.Length / 1024.0).ToString("#,### KB");

            FullName = file.FullName;
            ExAttributes = file.ExAttributes;

            //Attributes = file.Attributes;

            //CreationTimeUtc = file.CreationTimeUtc;
            //LastAccessTimeUtc = file.LastAccessTimeUtc;
            //LastWriteTimeUtc = file.LastWriteTimeUtc;

            //Length = file.Length;
            //ExAttributes = file.ExAttributes;
        }

        public static UIFileData? Create(IFileData? file)
        {
            if (file != null)
                return new UIFileData(file);
            else
                return null;
        }

        public UIFileData(IFileData file, BitmapSource icon)
        {
            Icon = icon;
            Name = file.Name;
            LastWriteTimeSt = file.LastWriteTimeUtc.ToLocalTime() == DateTime.MinValue.ToLocalTime() ? "" : file.LastWriteTimeUtc.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
            TypeName = file.TypeName;
            Size = file.Length == 0 ? "" : Math.Ceiling(file.Length / 1024.0).ToString("#,### KB");

            FullName = file.FullName;
            ExAttributes = file.ExAttributes;

            //Attributes = file.Attributes;
            //Name = file.Name;
            //FullName = file.FullName;
            ////CreationTimeUtc = file.CreationTimeUtc;
            ////LastAccessTimeUtc = file.LastAccessTimeUtc;
            //LastWriteTimeUtc = file.LastWriteTimeUtc;
            //TypeName = file.TypeName;
            //Length = file.Length;
            //ExAttributes = file.ExAttributes;
        }

        public static UIFileData? Create(IFileData? file, BitmapSource icon)
        {
            if (file != null)
                return new UIFileData(file, icon);
            else
                return null;
        }
    }
}

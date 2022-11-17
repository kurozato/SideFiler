using BlackSugar.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using BlackSugar.Service.Model;
using System.Collections.Specialized;

namespace BlackSugar.Views
{
    public class ClipboardHelper
    {
        public static string[] GetFiles()
        {
            if (Clipboard.ContainsFileDropList())
            {
                var fdList = Clipboard.GetFileDropList();
                string[] ary = new string[fdList.Count];
                fdList.CopyTo(ary, 0);

                return ary;
            }
            return null;
        }

        public static void SetFiles(IEnumerable<IFileData> files, Effect effect)
        {
            var filePaths = new StringCollection();
            foreach (var f in files)
                filePaths.Add(f.FullName);

            if (filePaths.Count == 0) return;

            if (effect == Effect.Copy)
                Clipboard.SetFileDropList(filePaths);

            if (effect == Effect.Move)
            {
                var aryFile = new string[filePaths.Count];
                filePaths.CopyTo(aryFile, 0);
                IDataObject data = new DataObject(DataFormats.FileDrop, aryFile);
                byte[] bs = new byte[] { (byte)DragDropEffects.Move, 0, 0, 0 };
                var ms = new MemoryStream(bs);
                try
                {
                    data.SetData("Preferred DropEffect", ms);
                    Clipboard.SetDataObject(data, true);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (ms != null) ms.Dispose();
                }
            }
        }

        public static Effect GetDropEffect()
        {
            var data = Clipboard.GetDataObject();
            if (data != null)
            {
                var ms = data.GetData("Preferred DropEffect") as MemoryStream;
                try
                {
                    if (ms != null)
                    {
                        var dde = (DragDropEffects)ms.ReadByte();

                        if (dde == (DragDropEffects.Copy | DragDropEffects.Link))
                        {
                            return Effect.Copy;
                        }
                        else if (dde == DragDropEffects.Move)
                        {
                            return Effect.Move;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (ms != null) ms.Dispose();
                }
            }

            return Effect.Undefined;

        }
    }
}

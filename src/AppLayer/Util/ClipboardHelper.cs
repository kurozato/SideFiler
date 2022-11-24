using BlackSugar.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using BlackSugar.Service.Model;
using System.Collections.Specialized;

namespace BlackSugar.Views
{
    public interface IClipboardHelper
    {
        string[] GetFiles();
        void SetFiles(IEnumerable<UIFileData> files, Effect effect);
        Effect GetDropEffect();

    }

    public class ClipboardHelper : IClipboardHelper
    {
        private const string PreferredDropEffect = "Preferred DropEffect";

        public string[] GetFiles()
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

        public void SetFiles(IEnumerable<UIFileData> files, Effect effect)
        {
            var filePaths = new StringCollection();
            foreach (var f in files)
                filePaths.Add(f.FullName);

            if (filePaths.Count == 0) return;

            if (effect == Effect.Move || effect == Effect.Copy)
            {
                var aryFile = new string[filePaths.Count];
                filePaths.CopyTo(aryFile, 0);
                IDataObject data = new DataObject(DataFormats.FileDrop, aryFile);

                byte[] bs;
                if(effect == Effect.Move)
                    bs = new byte[] { (byte)DragDropEffects.Move, 0, 0, 0 };
                else
                    bs = new byte[] { (byte)(DragDropEffects.Copy | DragDropEffects.Link), 0, 0, 0 };

                var ms = new MemoryStream(bs);
                try
                {
                    data.SetData(PreferredDropEffect, ms);
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

        public Effect GetDropEffect()
        {
            var data = Clipboard.GetDataObject();
            if (data != null)
            {
                var ms = data.GetData(PreferredDropEffect) as MemoryStream;
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
                    else
                    {
                        if (Clipboard.ContainsFileDropList())
                            return Effect.Copy;
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

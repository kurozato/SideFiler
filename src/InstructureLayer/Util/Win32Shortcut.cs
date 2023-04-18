using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Shellify;
using Shellify.Core;
using System.Diagnostics;

namespace BlackSugar.WinApi
{
    public class ShellLnk
    {
        public string Arguments { get; }
        public string Target { get; }
        public string WorkingDirectory { get; }
        public ProcessWindowStyle WindowStyle { get; }

        internal ShellLnk(ShellLinkFile lnk)
        {
            Arguments = lnk.Arguments;
            WorkingDirectory = lnk.WorkingDirectory;

            if (lnk.LinkInfo.LinkInfoFlags.HasFlag(LinkInfoFlags.VolumeIDAndLocalBasePath))
                Target = lnk.LinkInfo.LocalBasePath;

            if (lnk.LinkInfo.LinkInfoFlags.HasFlag(LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix))
                Target = lnk.LinkInfo.CommonNetworkRelativeLink.NetName;

            switch (lnk.Header.ShowCommand)
            {
                case ShowCommand.Normal:
                    WindowStyle = ProcessWindowStyle.Normal;
                    break;
                case ShowCommand.Maximized:
                    WindowStyle = ProcessWindowStyle.Maximized;
                    break;
                case ShowCommand.MinimizedNoActive:
                    WindowStyle = ProcessWindowStyle.Minimized;
                    break;
                default:
                    WindowStyle = ProcessWindowStyle.Normal;
                    break;
            }
        }

        public static ShellLnk? Load(string path)
        {
            if (Path.GetExtension(path).ToUpper() != ".LNK") return null;

            var lnk = ShellLinkFile.Load(path);
            if (lnk.Header.LinkFlags.HasFlag(LinkFlags.HasLinkInfo))
                return new ShellLnk(lnk);
            else
                return null; 
        }

        public ProcessStartInfo ToStartInfo()
            => new ProcessStartInfo
            {
                FileName = Target,
                Arguments = Arguments,
                WindowStyle = WindowStyle,
                UseShellExecute = true,
            };
    }

    
    //public class Win32Shortcut
    //{
    //    public enum ActiveSize
    //    {
    //        Undefined = 0,
    //        Nomal = 1,
    //        Maximized = 3,
    //        Minimized = 7,
    //    }

    //    public string Arguments { get; }
    //    public string Description { get; }
    //    public string FullName { get;  }
    //    public string Hotkey { get; }
    //    public string IconLocation { get; }
    //    public string TargetPath { get;  }
    //    public ActiveSize WindowStyle { get;  }
    //    public string WorkingDirectory { get; }

    //    internal Win32Shortcut(dynamic lnk)
    //    {
    //        Arguments = lnk.Arguments;
    //        Description = lnk.Description;
    //        FullName = lnk.FullName;
    //        Hotkey = lnk.Hotkey;
    //        IconLocation = lnk.IconLocation;
    //        TargetPath = lnk.TargetPath;
    //        WindowStyle = (ActiveSize)Enum.ToObject(typeof(ActiveSize), lnk.WindowStyle);
    //        WorkingDirectory = lnk.WorkingDirectory;
    //    }

    //    public override string ToString() => TargetPath + " " + Arguments;

    //    public static Win32Shortcut? Get(string fullPath)
    //    {
    //        if (Path.GetExtension(fullPath).ToUpper() != ".LNK") return null;

    //        dynamic? shell = null;   // IWshRuntimeLibrary.WshShell
    //        dynamic? lnk = null;     // IWshRuntimeLibrary.IWshShortcut
    //        try
    //        {
    //            var type = Type.GetTypeFromProgID("WScript.Shell");
    //            shell = Activator.CreateInstance(type!); //type is null throw ex.
    //            lnk = shell?.CreateShortcut(fullPath);

    //            if (string.IsNullOrEmpty(lnk?.TargetPath))
    //                return null;

    //            return new Win32Shortcut(lnk);
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            if (lnk != null) Marshal.ReleaseComObject(lnk);
    //            if (shell != null) Marshal.ReleaseComObject(shell);
    //        }
    //    }

    //    public System.Diagnostics.ProcessStartInfo ToStartInfo()
    //    {
    //        var startInfo = new System.Diagnostics.ProcessStartInfo
    //        {
    //            FileName = TargetPath,
    //            Arguments = Arguments
    //        };
    //        switch (WindowStyle)
    //        {
    //            case ActiveSize.Nomal:
    //                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
    //                break;
    //            case ActiveSize.Maximized:
    //                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
    //                break;
    //            case ActiveSize.Minimized:
    //                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
    //                break;
    //        }
    //        return startInfo;
    //    }
    //}
}

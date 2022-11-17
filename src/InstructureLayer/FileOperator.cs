using BlackSugar.Model;
using BlackSugar.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Repository
{
    public interface IFileOperator
    {
        void Copy(List<string> targets, string toFolder, IntPtr handle);

        void Move(List<string> targets, string toFolder, IntPtr handle);

        void Delete(List<string> targets, IntPtr handle);

        void Rename(string target, string name, IntPtr handle);

        void CreateFolder(string path);

        void OpenExplorer(IFileData file);

        bool ExecuteOrMove(ref IFileData file);

        void Execute(string application, string arguments);
    }

    public class FileOperator : IFileOperator
    {
        public void Copy(List<string> targets, string toFolder, IntPtr handle)
            => FileUtil.Copy(targets, toFolder, handle);    

        public void Move(List<string> targets, string toFolder, IntPtr handle)
            => FileUtil.Move(targets, toFolder, handle);

        public void Delete(List<string> targets, IntPtr handle)
            => FileUtil.Delete(targets, handle);

        public void Rename(string target, string name, IntPtr handle) 
            => FileUtil.Rename(new List<string>() { target }, name, handle);

        public void CreateFolder(string path) 
            => Directory.CreateDirectory(path);

        public void OpenExplorer(IFileData file)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "EXPLORER.EXE";
            process.StartInfo.Arguments = file.FullName;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        public bool ExecuteOrMove(ref IFileData file)
        {
            var lnk = Win32Shortcut.Get(file.FullName);
            var result = lnk == null ? file : FileUtil.Create(lnk.TargetPath);
            if (result.IsFile)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = lnk?.ToStartInfo() ?? new System.Diagnostics.ProcessStartInfo(file.FullName);
                process.StartInfo.UseShellExecute = true;
                process.Start();
                return false;
            }
            if (result.IsDirectory)
            {
                file = result;
                return true;
            }

            return false;
        }

        public void Execute(string application, string arguments)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = application;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }
}

using BlackSugar.Model;
using BlackSugar.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BlackSugar.Repository
{
    public interface IFileOperator
    {
        void Copy(List<string> targets, string toFolder, IntPtr handle);

        void Move(List<string> targets, string toFolder, IntPtr handle);

        void Delete(List<string> targets, IntPtr handle);

        void Rename(string target, string name, IntPtr handle);

        void CreateFolder(string path);

        void OpenExplorer(IFileData file, bool select);

        bool ExecuteOrMove(ref IFileData file);

        void Execute(string application, string? arguments, string? workingDirectory = null);

        void OpenDownload();

        void OpenTrash();

        void OpenCmd(IFileData? file);
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

        public void OpenExplorer(IFileData file, bool select)
        {
            var process = new Process();
            process.StartInfo.FileName = "EXPLORER.EXE";
            process.StartInfo.Arguments = (select ? "/select, " : "") + file.FullName;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        private ProcessStartInfo CreateStartInfo(IFileData file)
            => new ProcessStartInfo()
            {
                FileName = file.FullName,
                WorkingDirectory = Path.GetDirectoryName(file.FullName),
                UseShellExecute = true,
            };

        public bool ExecuteOrMove(ref IFileData file)
        {
            var lnk = MSShellLink.Load(file.FullName);

            var result = lnk == null ? file : FileUtil.Create(lnk.Target);
            if (result.IsFile)
            {
                Process process = new Process();
                process.StartInfo = lnk?.ToStartInfo() ?? CreateStartInfo(file);
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

        public void Execute(string application, string? arguments, string? workingDirectory = null)
        {
            var process = new Process();
            process.StartInfo.FileName = application;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;

            process.Start();
        }

        public void Rename(IFileData file, string newname)
        {
            if (file.IsDirectory)
                RenameDirectory(file.FullName, newname);
            else
                File.Move(file.FullName, newname);
        }

        private void RenameDirectory(string sourceFilePath, string outputFilePath)
        {
            if ((string.Compare(sourceFilePath, outputFilePath, true) == 0))
            {
                var tempPath = GetSafeTempName(outputFilePath);

                Directory.Move(sourceFilePath, tempPath);
                Directory.Move(tempPath, outputFilePath);
            }
            else
            {
                Directory.Move(sourceFilePath, outputFilePath);
            }
        }

        private string GetSafeTempName(string outputFilePath)
        {
            outputFilePath += "_";
            while (File.Exists(outputFilePath) || Directory.Exists(outputFilePath))
            {
                outputFilePath += "_";
            }
            return outputFilePath;
        }

        public void OpenDownload() => Execute("EXPLORER.EXE", FileUtil.GetDownloadsFolderPath());

        public void OpenTrash() => Execute("EXPLORER.EXE", "/e,::{645FF040-5081-101B-9F08-00AA002F954E}");

        public void OpenCmd(IFileData? file)
        {
            var path = file?.FullName ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Execute("cmd.exe", null, path);
        }

    }
}

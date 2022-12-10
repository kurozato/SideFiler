using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppExtensions;

namespace BlackSugar.Views
{
    public interface IExConfiguration
    {
        string? ExecutionPath { get; }
        string BaseDirectory { get; }
        string GetFullPath(string fileName, bool addJsonExtension = true, string? addExtension = null);
        string GetFullPath(string subDirectory, string fileName, bool addJsonExtension = true, string? addExtension = null);

    }
    public class ExConfiguration : IExConfiguration
    {
        public ExConfiguration()
        {
            ExecutionPath = Environment.ProcessPath;
            BaseDirectory = AppContext.BaseDirectory;
        }

        public string? ExecutionPath { get; }
        public string BaseDirectory { get; }


        public string GetFullPath(string fileName, bool addJsonExtension = true, string? addExtension = null) 
            => Path.Combine(BaseDirectory, GetFileWithExtenion(fileName, addJsonExtension, addExtension));

        public string GetFullPath(string subDirectory, string fileName, bool addJsonExtension = true, string? addExtension = null) 
            => Path.Combine(BaseDirectory, subDirectory, GetFileWithExtenion(fileName, addJsonExtension, addExtension));

        private string GetFileWithExtenion(string fileName, bool addJsonExtension = true, string? addExtension = null)
        {
            string result = fileName;
            if (addJsonExtension)
                result = result + ".json";
            else
            {
                if (addExtension?.StartsWith(".") == true)
                    result = result + addExtension;
                else
                    if (addExtension != null)
                     result = result + "." + addExtension;
            }
            
            return result;
        }
           
    }
}

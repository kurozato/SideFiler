using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSugar.Model;

namespace BlackSugar.Service.Model
{
    public class FileResultModel
    {
        public long? ID { get; set; }
        public string? Label { get; set; }
        public IFileData? File { get; set; }
        public IEnumerable<IFileData>? Results { get; set; }
    }

    public enum Effect
    {
        Undefined = -1,
        Copy = 1,
        Move = 2
    }
}

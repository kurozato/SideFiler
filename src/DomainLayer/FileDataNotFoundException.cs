using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Service
{
    [Serializable()]
    public class FileDataNotFoundException : Exception
    {
        private string? path;
        public string? Path => path;

        public static FileDataNotFoundException Create(string? path)
        {
            return new FileDataNotFoundException
            {
                path = path
            };
        }

        public string BuildMessage(string addMessage)
        {
            return "'" + Path +"' " + addMessage;
        }

        public FileDataNotFoundException()
        {
        }

        public FileDataNotFoundException(string? message) : base(message)
        {
        }

        public FileDataNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected FileDataNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

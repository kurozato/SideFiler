using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlackSugar.Service.Model
{
    public class BookmarkModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }
    }
}

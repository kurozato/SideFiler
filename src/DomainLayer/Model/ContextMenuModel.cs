using BlackSugar.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlackSugar.Service.Model
{
    public enum Multiple
    {
        Roop,
        Combine
    }

    public class ContextMenuModel
    {
        [JsonPropertyName("icon")]
        public string? IconPath { get; set; }
        [JsonPropertyName("content")]
        public string? Content { get; set; }
        [JsonPropertyName("app")]
        public string? App { get; set; }
        [JsonPropertyName("commandline")]
        public string? Commandline { get; set; }
        [JsonPropertyName("multiple")]
        public string? Multiple { get; set; }
        [JsonPropertyName("delimiter")]
        public string? Delimiter { get; set; }
        [JsonPropertyName("target")]
        public string? Target { get; set; }

        [JsonIgnore]
        public string? Result { get; set; }
 
        public string? GetArguments(string item)
        {
           return Commandline?.Replace("%1", item);
        }

        public string? GetArguments(string[] items)
        {
            if (Multiple.TryParse<Model.Multiple>() == Model.Multiple.Combine)
            {
                var sb = new StringBuilder();
                foreach (var item in items)
                {
                    sb.Append(Delimiter);
                    sb.Append('"');
                    sb.Append(item);
                    sb.Append('"');
                }
                return GetArguments(sb.ToString().Substring(Delimiter.Length));
            }

            return null;
        }
    }
}

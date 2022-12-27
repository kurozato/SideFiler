using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;


namespace BlackSugar.Repository
{
    public interface IJsonAdpter
    {
        TModel? Get<TModel>(string fileName, bool createInstance = true)
           where TModel : class, new();
        JsonNode? Get(string path);
        void Save(object content, string path);
        string ConvertFullPath(string fileName, bool addExtension = false, string extension = "json");
   
    }

    public class JsonAdpter : IJsonAdpter
    {
        public TModel? Get<TModel>(string path, bool createInstance = true)
            where TModel : class, new()
        {
            if (File.Exists(path) == false) return null;

            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };

            var model = JsonSerializer.Deserialize<TModel>(json, options);
            return model ?? (createInstance ? new TModel() : null);
        }

        public JsonNode? Get(string path)
        {
            var json = File.ReadAllText(path);
            return JsonNode.Parse(json);
        }

        public void Save(object content, string path)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(content, options);
            File.WriteAllText(path, json);
        }

        public string ConvertFullPath(string fileName, bool addExtension = false, string extension = "json")
        {
            if (addExtension)
                fileName = fileName + "." + extension;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }
           


        public async Task<TModel?> GetAsync<TModel>(string fileName, string extension = "json", bool createInstance = true)
            where TModel : class, new()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName + "." + extension);
            var stream = File.OpenRead(path);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };
            var model = await JsonSerializer.DeserializeAsync<TModel>(stream, options);

            return model ?? (createInstance ? new TModel() : null);
        }

        public Task SaveAsync(object content, string fileName, string extension = "json")
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName + "." + extension);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(content, options);
            return File.WriteAllTextAsync(path, json);
        }
    }
}

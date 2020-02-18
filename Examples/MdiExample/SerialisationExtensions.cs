using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MdiExample
{
    public static class SerialisationExtensions
    {
        public static Task<T> GetObjectFromJsonFile<T>(this string filename) where T : class
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (!File.Exists(filename))
                throw new FileNotFoundException("File not found", filename);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            
            var result = new TaskCompletionSource<T>();
            Task.Run(() =>
            {
                try
                {
                    var json = File.ReadAllText(filename);
                    T readResult = JsonConvert.DeserializeObject<T>(json, settings);
                    result.SetResult(readResult);
                }
                catch
                {
                    result.SetResult(null);
                }
            }).ConfigureAwait(false);

            return result.Task;
        }

        public static Task<bool> SaveObjectToJsonFile<T>(this T obj, string filename) where T : class
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var result = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(obj, settings);
                    File.WriteAllText(filename, json);
                    result.SetResult(true);
                }
                catch
                {
                    result.SetResult(false);
                }
            }).ConfigureAwait(false);

            return result.Task;
        }
    }
}

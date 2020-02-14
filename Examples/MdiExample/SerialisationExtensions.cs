using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MdiExample
{
    public static class SerialisationExtensions
    {
        public static Task<T> GetObjectFromJsonFile<T>(string filename) where T : class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException($"ReadJsonFile filename is empty");
            var result = new TaskCompletionSource<T>();
            Task.Run(() =>
            {
                try
                {
                    var json = File.ReadAllText(filename);
                    T readResult = JsonConvert.DeserializeObject<T>(json, settings);
                    //await Task.Delay(3000).ConfigureAwait(false);
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
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException($"ReadJsonFile filename is empty");
            var result = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(obj, settings);
                    File.WriteAllText(filename, json);
                    //await Task.Delay(3000).ConfigureAwait(false);
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

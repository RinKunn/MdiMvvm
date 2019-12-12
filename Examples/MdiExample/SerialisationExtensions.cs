using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdiExample
{
    public static class SerialisationExtensions
    {
        public static Task<T> GetObjectFromJsonFile<T>(string filename) where T : class
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException($"ReadJsonFile filename is empty");
            var result = new TaskCompletionSource<T>();
            Task.Run(async () =>
            {
                try
                {
                    var json = File.ReadAllText(filename);
                    T readResult = JsonConvert.DeserializeObject<T>(json);
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
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException($"ReadJsonFile filename is empty");
            var result = new TaskCompletionSource<bool>();
            Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(obj);
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

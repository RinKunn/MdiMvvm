using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public static class SerialisationExtensions
    {
        public static async Task<T> GetObjectFromJsonFileAsync<T>(this string filename, JsonSerializerSettings settings = null)
            where T : class
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (!File.Exists(filename))
                throw new FileNotFoundException("File not found", filename);

            var result = await Task.Run(() =>
            {
                try
                {
                    var json = File.ReadAllText(filename);
                    T readResult = JsonConvert.DeserializeObject<T>(json, settings);
                    return readResult;
                }
                catch
                {
                    return null;
                }
            }).ConfigureAwait(false);
            return result;
        }

        public static async Task<bool> SaveObjectToJsonFileAsync<T>(this T obj, string filename, JsonSerializerSettings settings = null) where T : class
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            bool result = await Task.Run(() =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(obj, settings);
                    File.WriteAllText(filename, json);
                    return true;
                }
                catch
                {
                    return false;
                }
            }).ConfigureAwait(false);

            return result;
        }
    }
}

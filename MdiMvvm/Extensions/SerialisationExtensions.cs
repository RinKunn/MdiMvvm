using System;
using System.IO;
using System.Threading.Tasks;
using MdiMvvm.Exceptions;
using MdiMvvm.ViewModels;
using Newtonsoft.Json;

namespace MdiMvvm.Extensions
{
    [Obsolete("Don't use this", true)]
    public static class SerialisationExtensions
    {
        public static Task<T> GetObjectFromJsonFile<T>(string filename) where T : class
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException($"ReadJsonFile filename is empty");
            var result = new TaskCompletionSource<T>();
            Task.Run(() =>
            {
                try
                {
                    var json = File.ReadAllText(filename);
                    T readResult = JsonConvert.DeserializeObject<T>(json);
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
            Task.Run(() =>
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

        public static async Task<bool> SaveContainerStateToFileAsync<TContainer>(this TContainer container, string filename) where TContainer : MdiContainerViewModelBase
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));

            bool success = false;
            try
            {
                success = await container.SaveObjectToJsonFile(filename).ConfigureAwait(false);
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public static async Task<TContainer> LoadContainerStateFromFileAsync<TContainer>(string filename) where TContainer : MdiContainerViewModelBase
        {
            TContainer loadedContainer = null;
            try
            {
                loadedContainer = await GetObjectFromJsonFile<TContainer>(filename).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new MdiContainerLoadingException(e.Message);
            }

            if (loadedContainer == null)
                throw new MdiContainerLoadingException($"Cannot load container from file: {filename}");
            return loadedContainer;
        }
    }
}

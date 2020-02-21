using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public interface IStoreSettings
    {
        string StoreFileName { get; }
        JsonSerializerSettings JsonSerializerSettings { get; }
    }

    public class DefaultStoreSettings : IStoreSettings
    {

        public DefaultStoreSettings()
        {
            string filename = Assembly.GetEntryAssembly()?.GetName()?.Name ?? Assembly.GetExecutingAssembly()?.GetName()?.Name;
            settingsFileName =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GalaxyBond", $"{filename}.json");
            settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

            string path = Path.GetDirectoryName(settingsFileName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        private string settingsFileName;
        private JsonSerializerSettings settings;

        public string StoreFileName => settingsFileName;
        public JsonSerializerSettings JsonSerializerSettings => settings;
    }
    
}

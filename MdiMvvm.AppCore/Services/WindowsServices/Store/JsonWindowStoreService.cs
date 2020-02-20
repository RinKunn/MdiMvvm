using System;
using System.IO;
using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public class JsonWindowStoreService : IWindowStoreService
    {
        private readonly string settingsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GalaxyBond", "winsettings.json");
        private readonly IWindowsManagerService _windowsManager;

        public JsonWindowStoreService(IWindowsManagerService windowsManager)
        {
            _windowsManager = windowsManager ?? throw new ArgumentNullException(nameof(windowsManager));
            string path = Path.GetDirectoryName(settingsFileName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public async Task<bool> KeepAsync(string saveFileName = null)
        {
            string filename = saveFileName ?? settingsFileName;

            bool success = false;
            ResumeStoreContext context = new ResumeStoreContext("admin");
            foreach (IStorable<ContainersStoreContext> container in _windowsManager.Containers)
            {
                var containerContext = new ContainersStoreContext();
                await containerContext.LoadContextFromEntity(container);
                context.ContainerContextCollection.Add(containerContext);
            }

            try
            {
                success = await context.SaveObjectToJsonFileAsync(filename).ConfigureAwait(false);
            }
            catch
            {
                success = false;
            }
            Console.WriteLine($"succes: {success}");
            return success;
        }
    }
}

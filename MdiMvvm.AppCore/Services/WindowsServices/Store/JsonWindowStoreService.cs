using System;
using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public class JsonWindowStoreService : IWindowStoreService
    {
        private readonly IWindowsManagerService _windowsManager;
        private readonly IStoreSettings _storeSettings;

        public JsonWindowStoreService(IWindowsManagerService windowsManager, IStoreSettings storeSettings)
        {
            _windowsManager = windowsManager ?? throw new ArgumentNullException(nameof(windowsManager));
            _storeSettings = storeSettings ?? throw new ArgumentNullException(nameof(storeSettings));
        }

        public async Task<bool> KeepAsync(string saveFileName = null)
        {
            string filename = saveFileName ?? _storeSettings.StoreFileName;

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
                success = await context.SaveObjectToJsonFileAsync(filename, _storeSettings.JsonSerializerSettings).ConfigureAwait(false);
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

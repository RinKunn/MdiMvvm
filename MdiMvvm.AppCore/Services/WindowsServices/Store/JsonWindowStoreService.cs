using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;
using MdiMvvm.Interfaces;

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
            bool success = true;
            string filename = saveFileName ?? _storeSettings.StoreFileName;

            var context = InitContext(_windowsManager.Containers);

            try
            {
                success = await context.SaveObjectToJsonFileAsync(filename, _storeSettings.JsonSerializerSettings).ConfigureAwait(false);
            }
            catch
            {
                success = false;
            }
            return success;
        }


        public bool Keep(string saveFileName = null)
        {
            bool success = true;
            string filename = saveFileName ?? _storeSettings.StoreFileName;

            var context = InitContext(_windowsManager.Containers);

            try
            {
                success = context.SaveObjectToJsonFile(filename, _storeSettings.JsonSerializerSettings);
            }
            catch
            {
                success = false;
            }
            return success;
        }

        private ResumeStoreContext InitContext(IEnumerable<IMdiContainerViewModel> containers)
        {
            ResumeStoreContext context = new ResumeStoreContext("admin");
            foreach (var container in containers)
            {
                var containerContext = (container as IStorable<ContainersStoreContext>).InitStoreContext();

                var storableWindows = container.WindowsCollection
                    .Where(w => w is IStorable<WindowsStoreContext>)
                    .Select(w => (IStorable<WindowsStoreContext>)w);

                foreach (var window in storableWindows)
                {
                    var windowsStoreContext = window.InitStoreContext();
                    containerContext.WindowsContextCollection.Add(windowsStoreContext);
                }

                context.ContainerContextCollection.Add(containerContext);
            }
            return context;
        }
    }
}

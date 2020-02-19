using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MdiExample.Services.WindowsServices.Factory;
using MdiExample.Services.WindowsServices.Store.Extensions;
using MdiExample.Services.WindowsServices.WindowsManager;
using MdiMvvm.Interfaces;

namespace MdiExample.Services.WindowsServices.Store
{
    public class JsonWindowLoaderService : IWindowLoaderService
    {
        private readonly string settingsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GalaxyBond", "winsettings.json");
        private readonly IWindowsManagerService _windowsManager;
        private readonly IWindowsFactory _windowsFactory;

        public JsonWindowLoaderService(IWindowsManagerService windowsManager, IWindowsFactory windowsFactory)
        {
            _windowsManager = windowsManager ?? throw new ArgumentNullException(nameof(windowsManager));
            _windowsFactory = windowsFactory ?? throw new ArgumentNullException(nameof(windowsFactory));
        }

        public async Task<bool> LoadAsync(string loadFileName = null)
        {
            bool success = false;
            string filename = loadFileName ?? settingsFileName;
            ResumeStoreContext resumeContext = null;

            if (!File.Exists(filename))
            {
                success = false;
            }
            else
            {
                try
                {
                    resumeContext = await filename.GetObjectFromJsonFileAsync<ResumeStoreContext>();
                    success = true;
                }
                catch
                {
                    success = false;
                }
            }

            if (!success)
                resumeContext = InitDefaultStoreContext();

            List<IMdiContainerViewModel> loadedContainers = new List<IMdiContainerViewModel>();
            foreach (ContainersStoreContext containerContext in resumeContext.ContainerContextCollection)
            {
                var container = (IMdiContainerViewModel)_windowsFactory.CreateContainer(containerContext.ViewModelType);
                await (container as IStorable<ContainersStoreContext>).OnLoading(containerContext);

                foreach (WindowsStoreContext windowContext in containerContext.WindowsContextCollection)
                {
                    var window = (IMdiWindowViewModel)_windowsFactory.CreateWindow(windowContext.ViewModelType);
                    window.Container = container;
                    container.WindowsCollection.Add(window);
                    await (window as IStorable<WindowsStoreContext>).OnLoading(windowContext);
                }
                loadedContainers.Add(container);
            }
            _windowsManager.LoadContainers(loadedContainers);
            return success;
        }

        private ResumeStoreContext InitDefaultStoreContext()
        {
            ResumeStoreContext context = new ResumeStoreContext("admin");
            for (int i = 1; i <= 3; i++)
            {
                ContainersStoreContext containersStoreContext = new ContainersStoreContext()
                {
                    Guid = Guid.NewGuid(),
                    ViewModelType = typeof(MdiContainerViewModel),
                    IsSelected = i == 1 ? true : false,
                    Title = $"DefaultTab {i}"
                };
                context.ContainerContextCollection.Add(containersStoreContext);
            }
            return context;
        }
    }
}

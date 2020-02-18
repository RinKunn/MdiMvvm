using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MdiExample.Services.WindowsServices.Factory;
using MdiExample.Services.WindowsServices.WindowsManager;
using MdiMvvm.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        public async Task<bool> Load(string loadFileName = null)
        {
            bool success = false;
            string filename = loadFileName ?? settingsFileName;
            ResumeStoreContext resumeContext = null;

            if (!File.Exists(settingsFileName))
            {
                success = false;
            }
            else
            {
                try
                {
                    resumeContext = await filename.GetObjectFromJsonFile<ResumeStoreContext>().ConfigureAwait(false);
                    await Task.Delay(3000);
                    success = true;
                }
                catch
                {
                    success = false;
                }
            }

            if (!success)
                resumeContext = InitDefaultStoreContext();

            ObservableCollection<IMdiContainerViewModel> loadedContainers = new ObservableCollection<IMdiContainerViewModel>();

            foreach (ContainersStoreContext containerContext in resumeContext.ContainerContextCollection)
            {
                var container = (IMdiContainerViewModel)_windowsFactory.CreateContainer(containerContext.ViewModelType);
                //_windowsManager.AppendContainer(container);
                loadedContainers.Add(container);
                await (container as IStorable<ContainersStoreContext>).OnLoading(containerContext);
                
                foreach (WindowsStoreContext windowContext in containerContext.WindowsContextCollection)
                {
                    var window = (IMdiWindowViewModel)_windowsFactory.CreateWindow(windowContext.ViewModelType);
                    //_windowsManager.AppendWindow(window, containerContext.Guid);
                    window.Container = container;
                    container.WindowsCollection.Add(window);
                    await (window as IStorable<WindowsStoreContext>).OnLoading(windowContext);
                }
            }
            //var activeContainer = resumeContext.ContainerContextCollection.FirstOrDefault(c => c.IsSelected);
            //Guid activeContainerGuid = activeContainer == null
            //    ? _windowsManager.Containers.First().Guid
            //    : activeContainer.Guid;
            //_windowsManager.ActivateContainer(activeContainerGuid);
            _windowsManager.Containers = loadedContainers;
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
                    Title = $"Tab {i}"
                };
                context.ContainerContextCollection.Add(containersStoreContext);
            }
            return context;
        }
    }
}

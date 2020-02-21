using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.Factory;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;
using MdiMvvm.AppCore.ViewModelsBase;
using MdiMvvm.AppCore.Extensions;
using MdiMvvm.Interfaces;


namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public class JsonWindowLoaderService : IWindowLoaderService
    {
        private readonly IWindowsManagerService _windowsManager;
        private readonly IWindowsFactory _windowsFactory;
        private readonly IStoreSettings _storeSettings;

        public JsonWindowLoaderService(IWindowsManagerService windowsManager, 
            IWindowsFactory windowsFactory,
            IStoreSettings storeSettings)
        {
            _windowsManager = windowsManager ?? throw new ArgumentNullException(nameof(windowsManager));
            _windowsFactory = windowsFactory ?? throw new ArgumentNullException(nameof(windowsFactory));
            _storeSettings = storeSettings ?? throw new ArgumentNullException(nameof(storeSettings));
        }

        public async Task<bool> LoadAsync(string loadFileName = null)
        {
            bool success = false;
            string filename = loadFileName ?? _storeSettings.StoreFileName;
            ResumeStoreContext resumeContext = null;

            if (!File.Exists(filename))
            {
                success = false;
            }
            else
            {
                try
                {
                    resumeContext = await filename.GetObjectFromJsonFileAsync<ResumeStoreContext>(_storeSettings.JsonSerializerSettings).ConfigureAwait(false);
                    success = true;
                }
                catch
                {
                    success = false;
                }
            }

            if (!success || resumeContext.ContainerContextCollection.Count == 0)
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
            Type continerType = Assembly.GetEntryAssembly().FindDerivedTypes<MdiContainerViewModelBase>().First();
            if (continerType == null) throw new ArgumentNullException("Didn't find type inheriting MdiContainerViewModelBase");
            for (int i = 1; i <= 3; i++)
            {
                ContainersStoreContext containersStoreContext = new ContainersStoreContext()
                {
                    Guid = Guid.NewGuid(),
                    ViewModelType = continerType,
                    IsSelected = i == 1 ? true : false,
                    Title = $"DefaultTab {i}"
                };
                context.ContainerContextCollection.Add(containersStoreContext);
            }
            return context;
        }
    }
}

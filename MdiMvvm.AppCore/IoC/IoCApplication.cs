using System.Windows;
using CommonServiceLocator;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;
using MdiMvvm.AppCore.Services.WindowsServices.Factory;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.Services.WindowsServices.Store;
using Unity;
using System.Threading.Tasks;
using System;

namespace MdiMvvm.AppCore
{
    public abstract class IoCApplication : Application
    {
        private IUnityContainer _container;
        public IUnityContainer Container => _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeInternal();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveWindows();
            base.OnExit(e);
        }


        private void InitializeInternal()
        {
            Initialize();
            LoadWindowsAsync().ConfigureAwait(false);
            StartShell();
        }

        public virtual void Initialize()
        {
            _container = new UnityContainer();
            RegisterRequiredTypes(_container);
            ConfigureServices(_container);

            ConfigureServiceLocator();

            var shell = CreateShell();
            if (shell != null)
            {
                InitializeShell(shell);
            }
        }

        protected virtual void RegisterRequiredTypes(IUnityContainer containerRegistry)
        {
            containerRegistry.RegisterInstance(_container);
            containerRegistry.RegisterSingleton<IServiceLocator, UnityServiceLocatorAdapter>();
            containerRegistry.RegisterSingleton<IWindowsManagerService, WindowsManagerService>();
            containerRegistry.RegisterSingleton<IStoreSettings, DefaultStoreSettings>();
            containerRegistry.RegisterType<INavigationService, NavigationService>();
            containerRegistry.RegisterType<IWindowsFactory, WindowsFactory>();
            containerRegistry.RegisterType<IWindowStoreService, JsonWindowStoreService>();
            containerRegistry.RegisterType<IWindowLoaderService, JsonWindowLoaderService>();
        }

        protected virtual void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => _container.Resolve<IServiceLocator>());
        }

        protected virtual void InitializeShell(Window shell)
        {
            MainWindow = shell;
        }

        protected virtual void StartShell()
        {
            MainWindow?.Show();
        }


        protected abstract void ConfigureServices(IUnityContainer containerRegistry);

        protected abstract Window CreateShell();


        public async Task LoadWindowsAsync()
        {
            var loader = Container.Resolve<IWindowLoaderService>();
            await loader.LoadAsync();
        }

        private void SaveWindows()
        {
            var keeper = Container.Resolve<IWindowStoreService>();
            var res = keeper.KeepAsync().Result;
        }
    }
}

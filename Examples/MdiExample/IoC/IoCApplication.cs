using System.Windows;
using CommonServiceLocator;
using Unity;

namespace MdiExample.IoC
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

        void InitializeInternal()
        {
            Initialize();
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
    }
}

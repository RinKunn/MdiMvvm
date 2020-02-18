using System.Windows;
using GalaSoft.MvvmLight.Threading;
using MdiExample.IoC;
using MdiExample.Services;
using MdiMvvm.ViewModels;
using Unity;
using MdiMvvm.Interfaces;
using MdiExample.Services.WindowsServices.WindowsManager;
using MdiExample.Services.WindowsServices.Navigation;
using MdiExample.Services.WindowsServices.Factory;
using MdiExample.Services.WindowsServices.Store;

namespace MdiExample
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : IoCApplication
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        public App() : base()
        {

        }

        protected override void ConfigureServices(IUnityContainer containerRegistry)
        {
            containerRegistry.RegisterSingleton<IWindowsManagerService, WindowsManagerService>();
            containerRegistry.RegisterType<INavigationService, NavigationService>();
            containerRegistry.RegisterType<IWindowsFactory, WindowsFactory>();
            containerRegistry.RegisterType<IWindowStoreService, JsonWindowStoreService>();
            containerRegistry.RegisterType<IWindowLoaderService, JsonWindowLoaderService>();

            containerRegistry.RegisterType<IMdiContainerViewModel, MdiContainerViewModel>();
            containerRegistry.RegisterType<IMdiWindowViewModel, Window1ViewModel>("Window1ViewModel");
            containerRegistry.RegisterType<IMdiWindowViewModel, Window2ViewModel>("Window2ViewModel");
        }

        protected override Window CreateShell()
        {
            MainWindow window = new MainWindow();
            window.DataContext = this.Container.Resolve<MainWindowViewModel>();
            window.Loaded += async (o, e) => await ((MainWindowViewModel)window.DataContext).LoadSettings();
            window.Closing += async (o, e) => await ((MainWindowViewModel)window.DataContext).SaveSettings();
            return window;
        }
    }
}

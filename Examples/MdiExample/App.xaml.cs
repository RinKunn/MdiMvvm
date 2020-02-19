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
using System.Threading.Tasks;
using MdiExample.View;
using MdiExample.ViewModel;
using System.Threading;

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
            window.Loaded += async (o, e) => await ((MainWindowViewModel)((Window)o).DataContext).LoadSettings();
            return window;
        }

        //protected override void StartShell()
        //{
        //    var splashScreenContext = new SplashScreenViewModel();
        //    var splashScreen = new SplashScreenView
        //    {
        //        DataContext = splashScreenContext
        //    };
        //    this.MainWindow = splashScreen;
        //    splashScreen.Show();

        //    Task.Factory.StartNew(() =>
        //    {
        //        splashScreen.Dispatcher.Invoke(() => { splashScreenContext.Progress = 10; splashScreenContext.Status = "Загрузка окон..."; });
        //        var store = Container.Resolve<IWindowStoreService>();
        //        store.Keep().Wait();
        //        splashScreen.Dispatcher.Invoke(() => { splashScreenContext.Progress = 70; splashScreenContext.Status = "Соединение с умом..."; });
        //        Thread.Sleep(500);
        //        splashScreen.Dispatcher.Invoke(() => { splashScreenContext.Progress = 100; splashScreenContext.Status = "Соединение успешно!"; });

        //        this.Dispatcher.Invoke(() =>
        //        {
        //            var mainWindow = new MainWindow();
        //            mainWindow.DataContext = this.Container.Resolve<MainWindowViewModel>();
        //            this.MainWindow = mainWindow;
        //            mainWindow.Show();
        //            splashScreen.Close();
        //        });
        //    });
        //}
    }
}

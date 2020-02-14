using System.Windows;
using GalaSoft.MvvmLight.Threading;
using MdiExample.IoC;
using Unity;

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
            
        }

        protected override Window CreateShell()
        {
            MainWindow window = new MainWindow();
            window.DataContext = this.Container.Resolve<MainWindowViewModel>();
            window.Loaded += (o, e) => ((MainWindowViewModel)window.DataContext).LoadSettings().GetAwaiter().GetResult();
            window.Closing += (o, e) => ((MainWindowViewModel)window.DataContext).SaveSettings().GetAwaiter().GetResult();
            
            return window;
        }
    }
}

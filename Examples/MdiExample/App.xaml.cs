using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace MdiExample
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}

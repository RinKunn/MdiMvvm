using System.Windows;

namespace MdiExample
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var context = new MainWindowViewModel();
            DataContext = context;
            this.Loaded += (o, e) => context.LoadSettings().GetAwaiter().GetResult();
            this.Closing += (o, e) => context.SaveSettings().GetAwaiter().GetResult();
        }
    }
}

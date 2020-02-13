using System;
using MdiMvvm.ViewModels;
using GalaSoft.MvvmLight.Command;

namespace MdiExample
{

    public class Window1ViewModel : MdiWindowViewModelBase
    {
        public Window1ViewModel() : base()
        {
            Random r = new Random();
            Title = $"Window {r.Next(1, 1000)}";
        }

        private RelayCommand _openWin2Command;
        public RelayCommand OpenWin2Command => _openWin2Command ?? (_openWin2Command = new RelayCommand(OpenWind2));

        private void OpenWind2()
        {
            Window2ViewModel wind = new Window2ViewModel();
            this.Container.AddMdiWindow(wind);
        }
    }
}

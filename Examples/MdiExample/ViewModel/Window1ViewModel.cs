using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiExample
{

    public class Window1ViewModel : MdiWindowViewModelBase, INavigateAware
    {
        private readonly INavigationService _navigation;

        public Window1ViewModel(INavigationService navigation) : base()
        {
            Random r = new Random();
            Title = $"Window {r.Next(1, 1000)}";
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }

        private RelayCommand _openWin2Command;
        private RelayCommand _closeCommand;
        
        
        public RelayCommand OpenWin2Command => _openWin2Command ?? (_openWin2Command = new RelayCommand(OpenWind2));
        public RelayCommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(() => this.Container.RemoveMdiWindow(this)));

        private void OpenWind2()
        {
            var context = new ViewModelContext();
            context.AddValue("Title", "hello from Window1ViewModel");

            _navigation.NavigateTo<Window2ViewModel>(new NavigateParameters(context));
        }

        public void NavigatedTo(ViewModelContext context)
        {
            Title = context.GetValue<string>("Title");
        }

        protected override Task OnWindowLoading(ViewModelContext context)
        {
            return Task.CompletedTask;
        }

        protected override Task OnWindowKeepeng(ViewModelContext context)
        {
            return Task.CompletedTask;
        }
    }
}

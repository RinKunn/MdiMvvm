using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MdiExample.Helper;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiExample
{

    public class Window1ViewModel : MdiWindowViewModelBase
    {
        private readonly INavigationService _navigation;

        public Window1ViewModel(INavigationService navigation) : base()
        {
            Random r = new Random();
            Title = $"Window {r.Next(1, 1000)}";
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }

        private AsyncCommand _openWin2Command;
        private RelayCommand _closeCommand;
        
        
        public AsyncCommand OpenWin2Command => _openWin2Command ?? (_openWin2Command = new AsyncCommand(OpenWind2));
        public RelayCommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(() => Close()));

        private async Task OpenWind2()
        {
            var context = new ViewModelContext();
            context.AddValue("Title", "hello from Window1ViewModel");

            try
            {
                await _navigation.NavigateTo<Window2ViewModel>(new NavigateParameters(context));
            }
            catch(Exception ex)
            {
                Title = "Error occur on Wind 2";
                
            }
        }
        
        public override void NavigatedTo(ViewModelContext context)
        {
            Title = context.GetValue<string>("Title");
        }

        protected override void OnLoadingState(ViewModelContext context)
        {
            
        }

        protected override void OnSavingState(ViewModelContext context)
        {
            
        }
    }
}

using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.ViewModelsBase;
using MdiMvvm.AppCore.Services.WindowsServices;

namespace MdiExample
{
    public class Window2ViewModel : MdiWindowViewModelBase
    {
        private readonly INavigationService _navigation;
        private string _text;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }
        public Window2ViewModel(INavigationService navigation) : base()
        {
            _navigation = navigation;
            Random r = new Random();
            Title = $"Window {r.Next(1, 1000)}";
        }

        protected override async Task OnIniting()
        {
            Random r = new Random();
            await Task.Delay(r.Next(1, 10)*1000);
            if(string.IsNullOrEmpty(Text)) Text = "Default text";
        }

        public override void NavigatedTo(ViewModelContext context)
        {
            Title = context.GetValue<string>("Title");
        }
        
        private RelayCommand _openWin3Command;
        public RelayCommand OpenWin3Command =>
            _openWin3Command ??
            (_openWin3Command = new RelayCommand(() =>
            {
                ViewModelContext context = new ViewModelContext();
                context.AddValue("Title", "Hello from win 2");
                _navigation.NavigateTo<Window3ViewModelCallBack>(new NavigateParameters(context), CallBack);
            }));

        private void CallBack(NavigationResult result)
        {
            Text = result.Context.GetValue<string>("Text");
        }

        protected override void OnLoadingState(ViewModelContext context)
        {
            Text = context.GetValue<string>("Text");
        }
        protected override void OnSavingState(ViewModelContext context)
        {
            context.AddValue("Text", Text);
        }
    }
}

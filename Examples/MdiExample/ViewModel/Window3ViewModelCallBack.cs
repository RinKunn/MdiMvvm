using System;
using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.ViewModelsBase;
using GalaSoft.MvvmLight.CommandWpf;

namespace MdiExample
{
    public class Window3ViewModelCallBack : MdiWindowNotStorableViewModelBase
    {
        private string _callBackText;
        public string CallBackText
        {
            get => _callBackText;
            set => Set(ref _callBackText, value);
        }

        public Window3ViewModelCallBack() : base()
        {
            Random r = new Random();
            Title = $"Window {r.Next(1, 1000)}";
        }

        public override void NavigatedTo(ViewModelContext context)
        {
            Title = context.GetValue<string>("Title");
        }


        public RelayCommand _sendCallBack;
        public RelayCommand SendCallBack =>
            _sendCallBack ??
            (_sendCallBack = new RelayCommand(() =>
            {
                RaiseCallBack(new NavigationResult("Text", CallBackText, true));
                Close();
            }));
    }
}

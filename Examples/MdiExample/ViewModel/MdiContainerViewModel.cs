using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.ViewModelsBase;
using MdiMvvm.Interfaces;

namespace MdiExample
{
    public class MdiContainerViewModel : MdiContainerViewModelBase
    {
        private readonly INavigationService _navigation;
        public int WindowsCount => this.WindowsCollection.Count;

        public MdiContainerViewModel(INavigationService navigation) : base()
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            this.WindowsCollection.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(WindowsCount));
        }

        private RelayCommand _addCommand;
        private RelayCommand _hideAllCommand;
        private RelayCommand _closeAllCommand;

        public RelayCommand AddCommand =>
            _addCommand ?? (_addCommand = new RelayCommand(() => OpenWindow<Window1ViewModel>("AddCommand")));

        public RelayCommand HideAllCommand =>
            _hideAllCommand ?? (_hideAllCommand = new RelayCommand(() =>
            {
                foreach (var win in WindowsCollection)
                    win.WindowState = System.Windows.WindowState.Minimized;
            }));

        public RelayCommand CloseAllCommand =>
            _closeAllCommand ?? (_closeAllCommand = new RelayCommand(() =>
            {
                for (int i = WindowsCollection.Count - 1; i >= 0; i--)
                    WindowsCollection[i].Close();
            }));


        private void OpenWindow<TWindow>(string contextStr) where TWindow : class, IMdiWindowViewModel, INavigateAware
        {
            var context = new ViewModelContext();   
            context.AddValue("Title", $"Hello from {contextStr}");
            _navigation.NavigateTo<TWindow>(new NavigateParameters(context, containerGuid: this.Guid));
        }

        protected override void OnLoadingContainerState(ViewModelContext context)
        {
           
        }

        protected override void OnSavingContainerState(ViewModelContext context)
        {
           
        }
    }
}

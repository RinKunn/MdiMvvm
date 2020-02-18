using System.Collections.Specialized;
using MdiMvvm.Interfaces;
using System;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using CommonServiceLocator;
using MdiExample.Services.WindowsServices.Navigation;
using MdiExample.ViewModel.Base;
using System.Threading.Tasks;

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
        private RelayCommand _addCommandModal;

        public RelayCommand AddCommand =>
            _addCommand ?? (_addCommand = new RelayCommand(() => OpenWindow<Window1ViewModel>("AddCommand")));

        public RelayCommand AddCommandModal =>
            _addCommandModal ?? (_addCommandModal = new RelayCommand(() => OpenWindow<Window1ViewModel>("AddCommandModal")));


        private void OpenWindow<TWindow>(string contextStr) where TWindow : class, IMdiWindowViewModel, INavigateAware
        {
            var context = new ViewModelContext();
            context.AddValue("Title", $"Hello from {contextStr}");
            _navigation.NavigateTo<TWindow>(new NavigateParameters(context, containerGuid: this.Guid));
        }

        public override Task OnContainerLoading(ViewModelContext context)
        {
            return Task.CompletedTask;
        }

        public override Task OnContainerKeeping(ViewModelContext context)
        {
            return Task.CompletedTask;
        }
    }
}

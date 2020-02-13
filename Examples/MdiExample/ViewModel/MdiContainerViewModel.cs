using GalaSoft.MvvmLight.CommandWpf;
using MdiMvvm.ViewModels;
using Newtonsoft.Json;

namespace MdiExample
{
    public class MdiContainerViewModel : MdiContainerViewModelBase
    {
        public int WindowsCount => this.WindowsCollection.Count;
        protected MdiContainerViewModel() { }

        public MdiContainerViewModel(string title) : base()
        {
            Title = title;
            this.WindowsCollection.CollectionChanged += (o, e) => RaisePropertyChanged(() => WindowsCount);
        }

        private RelayCommand _addCommand;
        private RelayCommand _addCommandModal;

        [JsonIgnore]
        public RelayCommand AddCommand =>
            _addCommand ??
            (_addCommand = new RelayCommand(() => this.AddMdiWindow(new Window1ViewModel() { IsModal = false, Title = "Default title aga" })));

        [JsonIgnore]
        public RelayCommand AddCommandModal =>
            _addCommandModal ??
            (_addCommandModal = new RelayCommand(() => this.AddMdiWindow(new Window1ViewModel() { IsModal = true, Title = "Default modal title aga" })));


    }
}

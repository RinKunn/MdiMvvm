using System.Collections.Specialized;
using GalaSoft.MvvmLight.CommandWpf;
using MdiMvvm.ViewModels;
using Newtonsoft.Json;

namespace MdiExample
{
    public class MdiContainerViewModel : MdiContainerViewModelBase
    {
        [JsonIgnore]
        public int WindowsCount => this.WindowsCollection.Count;

        protected MdiContainerViewModel() : base()
        {
            this.WindowsCollection.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(WindowsCount));
        }

        public MdiContainerViewModel(string title) : this()
        {
            Title = title;
            
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

        protected override void WindowsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.WindowsCollection_CollectionChanged(sender, e);

        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MdiMvvm.Interfaces;

namespace MdiMvvm.ViewModels
{
    [Obsolete("Don't use this", true)]
    public class MdiContainerCollectionViewModelBase : ViewModelBase
    {

        private ObservableCollection<IMdiContainerViewModel> _mdiContainers;

        public ObservableCollection<IMdiContainerViewModel> MdiContainers
        {
            get => _mdiContainers;
        }

        public MdiContainerCollectionViewModelBase()
        {
            _mdiContainers = new ObservableCollection<IMdiContainerViewModel>();
            _mdiContainers.CollectionChanged += _mdiContainers_CollectionChanged;
        }

        private void _mdiContainers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {

            }
        }
    }
}

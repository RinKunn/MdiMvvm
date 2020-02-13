using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using MdiMvvm.Exceptions;

namespace MdiMvvm.ViewModels
{
    public abstract class MdiContainerViewModelBase : ViewModelBase
    {
        #region Members
        private Guid _guid;
        private string _title;
        private bool _isSelected;
        private bool _isBusy;
        private ObservableCollection<MdiWindowViewModelBase> _windowsCollection;
        #endregion

        #region Properties
        /// <summary>
        /// GUID of Container
        /// </summary>
        public Guid Guid => _guid;

        /// <summary>
        /// Title of container
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(() => Title, ref _title, value);
        }

        /// <summary>
        /// Is Container selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(() => IsSelected, ref _isSelected, value);
        }

        /// <summary>
        /// Is container busy doing some process
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(() => IsBusy, ref _isBusy, value);
        }

        /// <summary>
        /// <see cref="MdiWindowViewModelBase" />'s collection
        /// </summary>
        public ObservableCollection<MdiWindowViewModelBase> WindowsCollection
        {
            get => _windowsCollection;
            set
            {
                if(_windowsCollection != null) 
                    _windowsCollection.CollectionChanged -= WindowsCollection_CollectionChanged;
                Set(() => WindowsCollection, ref _windowsCollection, value);
                if(value != null)
                    _windowsCollection.CollectionChanged += WindowsCollection_CollectionChanged;
            }
        }

        private void WindowsCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (MdiWindowViewModelBase win in e.NewItems)
                    win.Container = this;
            }
        }
        #endregion

        public MdiContainerViewModelBase()
        {
            _guid = Guid.NewGuid();
            WindowsCollection = new ObservableCollection<MdiWindowViewModelBase>();
        }

        /// <summary>
        /// Add mdi window to container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="MdiWindowViewModelBase"/></typeparam>
        /// <param name="window">Adding window</param>
        public void AddMdiWindow<TWindow>(TWindow window) where TWindow : MdiWindowViewModelBase
        {
            if (window == null) return;
            if (_windowsCollection.FirstOrDefault(w => w.Guid == window.Guid) != null)
                throw new MdiWindowAlreadyExistsException(window.Guid, this.Title);

            // TODO: validate Title for equal windows title

            _windowsCollection.Add(window);
        }

        /// <summary>
        /// Remove mdi window from container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="MdiWindowViewModelBase"/></typeparam>
        /// <param name="window">Removing window</param>
        public void RemoveMdiWindow<TWindow>(TWindow window) where TWindow : MdiWindowViewModelBase
        {
            if (window == null) return;
            if (_windowsCollection.FirstOrDefault(w => w.Guid == window.Guid) == null)
                throw new MdiWindowNotFoundedException(window.Guid, this.Title);
            _windowsCollection.Remove(window);
        }

        /// <summary>
        /// Remove mdi window from container
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveMdiWindowByGuid(Guid guid)
        {
            if (guid == null || string.IsNullOrEmpty(guid.ToString())) return;
            var window = _windowsCollection.FirstOrDefault(w => w.Guid == guid);
            if (window == null)
                throw new MdiWindowNotFoundedException(guid, this.Title);
            _windowsCollection.Remove(window);
        }
    }
}

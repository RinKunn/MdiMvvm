using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using MdiMvvm.AppCore.Services.WindowsServices.Store;
using MdiMvvm.Exceptions;
using MdiMvvm.Interfaces;

namespace MdiMvvm.AppCore.ViewModelsBase
{
    public abstract class MdiContainerViewModelBase : ViewModelBase, IMdiContainerViewModel, IStorable<ContainersStoreContext>, IBusy
    {
        #region Members
        private Guid _guid;
        private string _title;
        private bool _isSelected;
        private bool _isBusy;
        private bool _isInited;
        private ObservableCollection<IMdiWindowViewModel> _windowsCollection;
        #endregion

        #region Properties
        /// <summary>
        /// GUID of Container
        /// </summary>
        public Guid Guid
        {
            get => _guid;
            set => Set(ref _guid, value);
        }

        /// <summary>
        /// Title of container
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        /// <summary>
        /// Is Container selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        /// <summary>
        /// Is container busy doing some process
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public bool IsInited
        {
            get => _isInited;
            set => Set(ref _isInited, value);
        }

        /// <summary>
        /// <see cref="IMdiWindowViewModel" />'s collection
        /// </summary>
        public ObservableCollection<IMdiWindowViewModel> WindowsCollection
        {
            get => _windowsCollection;
            set
            {
                if (_windowsCollection != null)
                {
                    _windowsCollection.CollectionChanged -= WindowsCollectionChanged;
                }
                Set(ref _windowsCollection, value);
                if (value != null)
                {
                    _windowsCollection.CollectionChanged += WindowsCollectionChanged;
                    foreach (IMdiWindowViewModel win in value)
                        win.Container = this;
                }
            }
        }

        #endregion

        public MdiContainerViewModelBase()
        {
            _guid = Guid.NewGuid();
            WindowsCollection = new ObservableCollection<IMdiWindowViewModel>();
            IsInited = false;
        }

        /// <summary>
        /// Add mdi window to container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="IMdiWindowViewModel"/></typeparam>
        /// <param name="window">Adding window</param>
        public void AddMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel
        {
            if (window == null) return;
            if (_windowsCollection.FirstOrDefault(w => w.Guid == window.Guid) != null)
                throw new MdiWindowAlreadyExistsException(window.Guid, Title);

            // TODO: validate Title for equal windows title

            _windowsCollection.Add(window);
        }

        /// <summary>
        /// Remove mdi window from container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="IMdiWindowViewModel"/></typeparam>
        /// <param name="window">Removing window</param>
        public void RemoveMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel
        {
            if (window == null) return;
            if (_windowsCollection.FirstOrDefault(w => w.Guid == window.Guid) == null)
                throw new MdiWindowNotFoundedException(window.Guid, Title);
            _windowsCollection.Remove(window);
        }

        protected virtual void WindowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (MdiWindowNotStorableViewModelBase win in e.NewItems)
                    win.Container = this;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (MdiWindowNotStorableViewModelBase win in e.OldItems)
                    win.Container = null;
            }
        }

        public void LoadFromStoreContext(ContainersStoreContext context)
        {
            Guid = context.Guid;
            IsSelected = context.IsSelected;
            Title = context.Title;
            OnLoadingContainerState(context.ViewModelContext);
        }
        public ContainersStoreContext InitStoreContext()
        {
            ContainersStoreContext context = new ContainersStoreContext();
            context.Guid = Guid;
            context.IsSelected = IsSelected;
            context.Title = Title;
            context.ViewModelType = GetType();
            OnSavingContainerState(context.ViewModelContext);
            return context;
        }

        protected abstract void OnLoadingContainerState(ViewModelContext context);
        protected abstract void OnSavingContainerState(ViewModelContext context);

        public void Init()
        {
            if (WindowsCollection.Count > 0)
            {
                foreach (var window in WindowsCollection)
                {
                    window.InitAsync();
                }
            }
            IsInited = true;
        }

    }
}

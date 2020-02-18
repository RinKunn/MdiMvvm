using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using MdiExample.Services.WindowsServices.Store;
using MdiMvvm.Exceptions;
using MdiMvvm.Interfaces;

namespace MdiExample.ViewModel.Base
{
    public abstract class MdiContainerViewModelBase : ViewModelBase, IMdiContainerViewModel, IStorable<ContainersStoreContext>, IBusy
    {
        #region Members
        private Guid _guid;
        private string _title;
        private bool _isSelected;
        private bool _isBusy;
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

        /// <summary>
        /// <see cref="IMdiWindowViewModel" />'s collection
        /// </summary>
        public ObservableCollection<IMdiWindowViewModel> WindowsCollection
        {
            get => _windowsCollection;
            set
            {
                if(_windowsCollection != null)
                {
                    _windowsCollection.CollectionChanged -= WindowsCollectionChanged;
                }
                Set(ref _windowsCollection, value);
                if(value != null)
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
                throw new MdiWindowAlreadyExistsException(window.Guid, this.Title);

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
                throw new MdiWindowNotFoundedException(window.Guid, this.Title);
            _windowsCollection.Remove(window);
        }

        protected virtual void WindowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (MdiWindowViewModelBase win in e.NewItems)
                    win.Container = this;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (MdiWindowViewModelBase win in e.OldItems)
                    win.Container = null;
            }
        }

        public async Task<ContainersStoreContext> OnLoading(ContainersStoreContext context)
        {
            IsBusy = true;

            this.Guid = context.Guid;
            this.IsSelected = context.IsSelected;
            this.Title = context.Title;

            await OnContainerLoading(context.ViewModelContext);

            DispatcherHelper.CheckBeginInvokeOnUI(() => { IsBusy = false; });
            return context;
        }

        public async Task<ContainersStoreContext> OnKeeping(ContainersStoreContext context)
        {
            IsBusy = true;

            context.Guid = this.Guid;
            context.IsSelected = this.IsSelected;
            context.Title = this.Title;
            context.ViewModelType = this.GetType();

            var collection = WindowsCollection.Where(w => w is IStorable<WindowsStoreContext>).Select(w => (IStorable<WindowsStoreContext>)w);
            foreach (var wind in collection)
            {
                var windowsStoreContext = new WindowsStoreContext();
                await windowsStoreContext.LoadContextFromEntity(wind);
                context.WindowsContextCollection.Add(windowsStoreContext);
            }

            await OnContainerKeeping(context.ViewModelContext);

            DispatcherHelper.CheckBeginInvokeOnUI(() => { IsBusy = false; });

            return context;
        }

        public abstract Task OnContainerLoading(ViewModelContext context);
        public abstract Task OnContainerKeeping(ViewModelContext context);
    }
}

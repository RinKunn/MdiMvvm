using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MdiMvvm.Interfaces;

namespace MdiMvvm.AppCore.Services.WindowsServices.WindowsManager
{
    public class WindowsManagerService : ViewModelBase, IWindowsManagerService
    {
        private IMdiContainerViewModel _activeContainer;
        private ObservableCollection<IMdiContainerViewModel> _containers;
        private ReadOnlyObservableCollection<IMdiContainerViewModel> _containersReadOnly;

        public IMdiContainerViewModel ActiveContainer
        {
            get => _activeContainer;
            private set
            {
                if (ReferenceEquals(_activeContainer, value) || _activeContainer.Guid == value.Guid) return;

                var oldContainer = _activeContainer;
                if (_activeContainer != null) _activeContainer.IsSelected = false;
                Set(ref _activeContainer, value);
                if (_activeContainer != null) _activeContainer.IsSelected = true;
                ActiveContainerChanged?.Invoke(new ActiveContainerChangedArgs(oldContainer, value));
                //Console.WriteLine($"_activeContainer = {_activeContainer.IsBusy}, IsInited = {_activeContainer.IsInited}");
                if (!_activeContainer.IsInited) _activeContainer.Init();
                //Console.WriteLine($"\t {_activeContainer.IsBusy}, IsInited = {_activeContainer.IsInited}");
            }
        }
        public ReadOnlyObservableCollection<IMdiContainerViewModel> Containers => _containersReadOnly;

        public event ContainersCollectionChangedHandler ContainerCollectionChanged;
        public event ActiveContainerChangedHandler ActiveContainerChanged;

        public WindowsManagerService()
        {
            _containers = new ObservableCollection<IMdiContainerViewModel>();
            _containersReadOnly = new ReadOnlyObservableCollection<IMdiContainerViewModel>(_containers);
        }

        #region COllection behaviour

        public void LoadContainers(IEnumerable<IMdiContainerViewModel> collection)
        {
            if (ReferenceEquals(_containers, collection)) return;

            _containers = collection.ToObservableCollection();
            _containersReadOnly = new ReadOnlyObservableCollection<IMdiContainerViewModel>(_containers);
            ContainerCollectionChanged?.Invoke(new ContainersCollectionChangedArgs());

            ActiveContainer = _containers?.FirstOrDefault(c => c.IsSelected);
        }
        #endregion

        #region Mdi-windows behaviour
         
        public TViewModel AppendWindowToContainer<TViewModel>(TViewModel viewModel, Guid containerGuid, bool withIniting = true)
            where TViewModel : IMdiWindowViewModel
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            IMdiContainerViewModel mdiContainer = _containers.FirstOrDefault(c => c.Guid == containerGuid);
            if (mdiContainer == null) throw new ArgumentNullException(nameof(mdiContainer));
            ActiveContainer = mdiContainer;
            mdiContainer.AddMdiWindow(viewModel);
            viewModel.InitAsync();
            return viewModel;
        }

        public TViewModel AppendWindow<TViewModel>(TViewModel viewModel)
            where TViewModel : IMdiWindowViewModel
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            ActiveContainer.AddMdiWindow(viewModel);
            viewModel.InitAsync().ConfigureAwait(false);
            return viewModel;
        }

        public object AppendWindowToContainer(object viewModel, Guid containerGuid)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (!(viewModel is IMdiWindowViewModel mdiWindow))
                throw new NotImplementedException($"Object with type '{viewModel.GetType()}' not implement '{typeof(IMdiWindowViewModel).Name}'");

            IMdiContainerViewModel mdiContainer;
            mdiContainer = containerGuid == Guid.Empty ?
                ActiveContainer :
                _containers.FirstOrDefault(c => c.Guid == containerGuid);

            if (mdiContainer == null) throw new ArgumentNullException(nameof(mdiContainer));
            mdiContainer.AddMdiWindow(mdiWindow);
            mdiWindow.InitAsync().ConfigureAwait(false);
            return mdiWindow;
        }

        public TViewModel FindWindow<TViewModel>(Guid windowGuid) where TViewModel : class, IMdiWindowViewModel
        {
            //TODO create internal dictionary with windows for fast searching
            TViewModel window = null;
            foreach (var cont in _containers)
            {
                var findedWin = cont.WindowsCollection.FirstOrDefault(w => w.Guid == windowGuid);
                if (findedWin != null && findedWin is TViewModel res)
                {
                    window = res;
                    break;
                }
            }
            return window;
        }

        public void ActivateWindow<TViewModel>(TViewModel window) where TViewModel : class, IMdiWindowViewModel
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            ActivateContainer(window.Container);
            window.IsSelected = true;
        }
        #endregion

        #region Mdi-containers behaviour

        public TContainerViewModel AppendContainer<TContainerViewModel>(TContainerViewModel viewModel)
            where TContainerViewModel : IMdiContainerViewModel
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            _containers.Add(viewModel);
            if (viewModel.IsSelected) ActiveContainer = viewModel;
            return viewModel;
        }

        public object AppendContainer(object viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (!(viewModel is IMdiContainerViewModel mdiContainer))
                throw new NotImplementedException($"Object with type '{viewModel.GetType()}' not implement '{typeof(IMdiContainerViewModel).Name}'");
            _containers.Add(mdiContainer);
            if (mdiContainer.IsSelected) ActiveContainer = mdiContainer;
            return mdiContainer;
        }

        public void ActivateContainer(Guid guid)
        {
            if (guid == Guid.Empty) throw new ArgumentNullException(nameof(guid));
            var newContainer = _containers.FirstOrDefault(c => c.Guid == guid);
            if (newContainer == null) throw new ArgumentNullException(nameof(newContainer));
            ActiveContainer = newContainer;
        }

        public void ActivateContainer<TContainerViewModel>(TContainerViewModel containerViewModel)
            where TContainerViewModel : IMdiContainerViewModel
        {
            if (containerViewModel == null) throw new ArgumentNullException(nameof(containerViewModel));
            ActiveContainer = containerViewModel;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MdiMvvm.Exceptions;
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
                if (value == null || ReferenceEquals(_activeContainer, value) || 
                    (_activeContainer != null && _activeContainer.Guid == value.Guid)) return;

                var oldContainer = _activeContainer;
                if (_activeContainer != null) _activeContainer.IsSelected = false;
                Set(ref _activeContainer, value);
                if (_activeContainer != null) _activeContainer.IsSelected = true;
                ActiveContainerChanged?.Invoke(new ActiveContainerChangedArgs(oldContainer, value));
                if (!_activeContainer.IsInited) _activeContainer.Init();
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

        #region Collection behaviour

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
         
        public TViewModel AppendWindowWithoutInit<TViewModel>(TViewModel viewModel, Guid containerGuid = default, bool activateWindow = true)
            where TViewModel : IMdiWindowViewModel
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var mdiContainer = containerGuid == default
                ? ActiveContainer
                : _containers.FirstOrDefault(c => c.Guid == containerGuid);

            if (mdiContainer == null) throw new ArgumentNullException(nameof(mdiContainer));
            
            if (mdiContainer.WindowsCollection.FirstOrDefault(w => w.Guid == viewModel.Guid) != null)
                throw new MdiWindowAlreadyExistsException(viewModel.Guid, viewModel.Title);

            mdiContainer.WindowsCollection.Add(viewModel);
            if (viewModel.Container == null) viewModel.Container = mdiContainer;

            if (activateWindow)
            {
                ActivateWindow<TViewModel>(viewModel);
            }
            return viewModel;
        }

        public async Task<TViewModel> AppendWindowAsync<TViewModel>(TViewModel viewModel, Guid containerGuid = default, bool activateWindow = true)
           where TViewModel : IMdiWindowViewModel
        {
            var addedViewModel = AppendWindowWithoutInit(viewModel, containerGuid, activateWindow);
            await addedViewModel.InitAsync();
            return addedViewModel;
        }

        public object AppendWindowWithoutInit(object viewModel, Guid containerGuid = default, bool activateWindow = true)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (!(viewModel is IMdiWindowViewModel mdiWindow))
                throw new NotImplementedException($"Object with type '{viewModel.GetType()}' not implement '{typeof(IMdiWindowViewModel).Name}'");
            return AppendWindowWithoutInit<IMdiWindowViewModel>(mdiWindow, containerGuid, activateWindow);
        }

        public TViewModel FindWindow<TViewModel>(Guid windowGuid) where TViewModel : IMdiWindowViewModel
        {
            //TODO create internal dictionary with windows for fast searching
            TViewModel window = default;
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

        public void ActivateWindow<TViewModel>(TViewModel window) where TViewModel : IMdiWindowViewModel
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

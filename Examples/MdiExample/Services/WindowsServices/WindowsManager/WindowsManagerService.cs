using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using MdiMvvm.Interfaces;

namespace MdiExample.Services.WindowsServices.WindowsManager
{
    public class WindowsManagerService : ViewModelBase, IWindowsManagerService
    {
        private IMdiContainerViewModel _activeContainer;
        public IMdiContainerViewModel ActiveContainer
        {
            get => _activeContainer;
            private set
            {
                if (_activeContainer != null && _activeContainer == value) return;

                if (_activeContainer != null) _activeContainer.IsSelected = false;
                Set(ref _activeContainer, value);
                if (_activeContainer != null) _activeContainer.IsSelected = true;
            }
        }

        public event EventHandler ContainerCollectionChanged;
 
        private ObservableCollection<IMdiContainerViewModel> _containers;
        public ObservableCollection<IMdiContainerViewModel> Containers
        {
            get => _containers;
            set
            {
                Set(ref _containers, value);
                ActiveContainer = _containers?.FirstOrDefault(c => c.IsSelected);
                ContainerCollectionChanged?.Invoke(this, new EventArgs());
            }
        }

        public WindowsManagerService()
        {
            _containers = new ObservableCollection<IMdiContainerViewModel>();
        }

        public TViewModel AppendWindow<TViewModel>(TViewModel viewModel, Guid containerGuid) 
            where TViewModel : IMdiWindowViewModel
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            IMdiContainerViewModel mdiContainer = _containers.FirstOrDefault(c => c.Guid == containerGuid);
            if (mdiContainer == null) throw new ArgumentNullException(nameof(mdiContainer));
            ActiveContainer = mdiContainer;
            mdiContainer.AddMdiWindow(viewModel);
            return viewModel;
        }

        public TViewModel AppendWindow<TViewModel>(TViewModel viewModel)
            where TViewModel : IMdiWindowViewModel
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            ActiveContainer.AddMdiWindow(viewModel);
            return viewModel;
        }

        public object AppendWindow(object viewModel, Guid containerGuid)
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
            return mdiWindow;
        }

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

        //=====================================
        //TODO create internal dictionoary of windows for fast searching

        public TViewModel FindWindow<TViewModel>(Guid windowGuid) where TViewModel : class, IMdiWindowViewModel
        {
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

        //=====================================

        public void ActivateContainer(Guid guid)
        {
            if (guid == Guid.Empty) throw new ArgumentNullException(nameof(guid));
            var newContainer = _containers.FirstOrDefault(c => c.Guid == guid);
            if(newContainer == null) throw new ArgumentNullException(nameof(newContainer));
            ActiveContainer = newContainer;
        }

        public void ActivateContainer<TContainerViewModel>(TContainerViewModel containerViewModel)
            where TContainerViewModel : IMdiContainerViewModel
        {
            if (containerViewModel == null) throw new ArgumentNullException(nameof(containerViewModel));
            ActiveContainer = containerViewModel;
        }

        public void ActivateWindow<TViewModel>(TViewModel window) where TViewModel : class, IMdiWindowViewModel
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            ActivateContainer(window.Container);
            window.IsSelected = true;
        }
    }
}

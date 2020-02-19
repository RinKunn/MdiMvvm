using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MdiExample.Services.WindowsServices.WindowsManager;
using MdiExample.Services.WindowsServices.Store;
using MdiExample.ViewModel.Base;
using MdiMvvm.Interfaces;

namespace MdiExample
{
    public class MainWindowViewModel : ViewModelBase, IBusy
    {
        private readonly IWindowsManagerService _manager;
        private readonly IWindowStoreService _storeService;
        private readonly IWindowLoaderService _loaderService;

        private IMdiContainerViewModel _selectedContainer;
        private ReadOnlyObservableCollection<IMdiContainerViewModel> _containers;
        private bool _busy;


        public IMdiContainerViewModel SelectedContainer
        {
            get => _selectedContainer;
            set
            {
                Set(ref _selectedContainer, value);
                if (value != null)
                    _manager.ActivateContainer(value);
            }
        }
        public ReadOnlyObservableCollection<IMdiContainerViewModel> Containers
        {
            get => _containers;
            private set => Set(ref _containers, value);
        }
        public bool IsBusy
        {
            get => _busy;
            set => Set(ref _busy, value);
        }

        public MainWindowViewModel(
            IWindowsManagerService manager, 
            IWindowStoreService storeService,
            IWindowLoaderService loaderService)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _loaderService = loaderService ?? throw new ArgumentNullException(nameof(loaderService));

            Containers = _manager.Containers;
            SelectedContainer = _manager.ActiveContainer;

            IsBusy = false;
            _manager.ContainerCollectionChanged += (args) => Containers = _manager.Containers;
            _manager.ActiveContainerChanged += (args) => SelectedContainer = args.NewContainer;
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new RelayCommand(async () =>
            {
                string fname = null;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "mygalaxyview.gbvw";
                saveFileDialog.Filter = "Custom type files|*.gbvw|All files|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fname = saveFileDialog.FileName;
                    await SaveSettings(fname);
                }
                SaveCommand.RaiseCanExecuteChanged();
            }, () => !_busy));


        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand =>
            _loadCommand ?? (_loadCommand = new RelayCommand(async () =>
            {
                string fname = null;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = "gbvw";
                openFileDialog.Filter = "Custom type files|*.gbvw|All files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fname = openFileDialog.FileName;
                    await LoadSettings(fname);
                }
                LoadCommand.RaiseCanExecuteChanged();
            }, () => !_busy));

        public async Task SaveSettings(string saveFilename = null)
        {
            IsBusy = true;
            try
            {
                await _storeService.KeepAsync(saveFilename);
            }
            catch { }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => IsBusy = false);
            }
        }

        public async Task LoadSettings(string loadingFilename = null)
        {
            IsBusy = true;
            try 
            {
                await _loaderService.LoadAsync(loadingFilename);
            }
            catch { }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => IsBusy = false);
            }
        }
    }
}

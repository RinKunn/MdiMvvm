using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MdiExample
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string settingsFileName = "./example.json";
        private MdiContainerViewModel _selectedContainer;
        private ObservableCollection<MdiContainerViewModel> _containers;
        private bool _busy;
        
        [JsonIgnore]
        public bool IsBusy
        {
            get => _busy;
            set => Set(ref _busy, value);
        }

        public ObservableCollection<MdiContainerViewModel> Containers
        {
            get => _containers;
            set => Set(ref _containers, value);
        }
        [JsonIgnore]
        public MdiContainerViewModel SelectedContainer
        {
            get => _selectedContainer;
            set
            {
                if(_selectedContainer != null) _selectedContainer.Selectedd = false;
                Set(ref _selectedContainer, value);
                if (_selectedContainer != null) _selectedContainer.Selectedd = true;
            }
        }


        public MainWindowViewModel()
        {   
            IsBusy = false;
        }

        public async void Init() => await LoadSettings();


        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;


        [JsonIgnore]
        public RelayCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new RelayCommand(async () =>
            {
                await SaveSettings();
                SaveCommand.RaiseCanExecuteChanged();
            }, () => !_busy));


        [JsonIgnore]
        public RelayCommand LoadCommand =>
            _loadCommand ?? (_loadCommand = new RelayCommand(async () =>
            {
                await LoadSettings();
                LoadCommand.RaiseCanExecuteChanged();
            }, () => !_busy));




        public async Task<bool> SaveSettings()
        {
            bool success = false;
            try
            {
                IsBusy = true;
                success = await this.SaveObjectToJsonFile(settingsFileName).ConfigureAwait(false);
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsBusy = false;
                });
            }
            return success;
        }

        private async Task LoadSettings()
        {
            MainWindowViewModel success = null;
            try
            {
                IsBusy = true;
                success = await SerialisationExtensions.GetObjectFromJsonFile<MainWindowViewModel>(settingsFileName).ConfigureAwait(false);
            }
            // ensure that no matter what, the busy state is cleared even if there were errors
            finally
            {
                // make sure we're on the UI thread...
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsBusy = false;
                });
            }

            if (success != null)
            {
                this.Containers = success.Containers;
                this.SelectedContainer = Containers.FirstOrDefault(c => c.Selectedd == true);
                if (SelectedContainer == null && Containers.Count > 0)
                    SelectedContainer = Containers[0];
            }
            else
            {
                Containers = new ObservableCollection<MdiContainerViewModel>();
                Containers.Add(new MdiContainerViewModel("item 1"));
                Containers.Add(new MdiContainerViewModel("item 2"));
                Containers.Add(new MdiContainerViewModel("item 3"));

                SelectedContainer = Containers.First();
            }
        }

    }
}

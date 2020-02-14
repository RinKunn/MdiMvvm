using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;

namespace MdiExample
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string settingsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GalaxyBond", "winsettings.json");

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
                if (_selectedContainer != null) _selectedContainer.IsSelected = false;
                Set(ref _selectedContainer, value);
                if (_selectedContainer != null) _selectedContainer.IsSelected = true;
            }
        }


        public MainWindowViewModel()
        {
            IsBusy = false;
            string path = Path.GetDirectoryName(settingsFileName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public async void Init() => await LoadSettings();

        public string Text { get; set; }
        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;


        [JsonIgnore]
        public RelayCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new RelayCommand(async () =>
            {
                string fname = null;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.DefaultExt = "gbvw";
                saveFileDialog.FileName = "mygalaxyview.gbvw";
                saveFileDialog.Filter = "Custom type files|*.gbvw|All files|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fname = saveFileDialog.FileName;
                    await SaveSettings(fname);
                }
                SaveCommand.RaiseCanExecuteChanged();
            }, () => !_busy));


        [JsonIgnore]
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


        public async Task<bool> SaveSettings(string saveFilename = null)
        {
            string filename = saveFilename ?? settingsFileName;
            bool success = false;
            try
            {
                IsBusy = true;
                success = await this.SaveObjectToJsonFile(filename).ConfigureAwait(false);
            }
            catch
            {
                success = false;
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

        public async Task LoadSettings(string saveFilename = null)
        {
            string filename = saveFilename ?? settingsFileName;

            MainWindowViewModel success = null;
            try
            {
                IsBusy = true;
                success = await SerialisationExtensions.GetObjectFromJsonFile<MainWindowViewModel>(filename).ConfigureAwait(false);
            }
            catch(JsonSerializationException exc)
            {
                throw new Exception($"ddd: {exc.InnerException}");
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
                this.SelectedContainer = success.Containers.FirstOrDefault(c => c.IsSelected == true);
                //if (SelectedContainer == null)
                //    SelectedContainer = Containers[0];
            }
            else
            {
                InitDefault();
            }
        }

        private void InitDefault()
        {
            Containers = new ObservableCollection<MdiContainerViewModel>
                {
                    new MdiContainerViewModel("item 1"),
                    new MdiContainerViewModel("item 2"),
                    new MdiContainerViewModel("item 3")
                };

            SelectedContainer = Containers.First();
        }

    }
}

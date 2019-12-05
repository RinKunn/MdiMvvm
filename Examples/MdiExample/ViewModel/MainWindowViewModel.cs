using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MdiExample
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MdiContainerViewModel> _containers;
        public ObservableCollection<MdiContainerViewModel> Containers
        {
            get => _containers;
            set
            {
                _containers = value;
                OnPropertyChanged(nameof(Containers));
            }
        }

        public ObservableCollection<string> Streams = new ObservableCollection<string>();

        private MdiContainerViewModel _selectedContainer;
        public MdiContainerViewModel SelectedContainer
        {
            get => _selectedContainer;
            set
            {
                //if (_selectedContainer != null &&_selectedContainer.ViewModelCollection.Count > 0)
                //    Console.WriteLine($"Pos: {_selectedContainer.ViewModelCollection[0].Left}");
                _selectedContainer = value;
                OnPropertyChanged(nameof(SelectedContainer));

                //if (_selectedContainer.ViewModelCollection.Count > 0)
                //    Console.WriteLine($"New Pos: {_selectedContainer.ViewModelCollection.FirstOrDefault()?.Left}");
            }
        }

        public MainWindowViewModel()
        {
            Containers = new ObservableCollection<MdiContainerViewModel>();
            Containers.Add(new MdiContainerViewModel("item 1"));
            Containers.Add(new MdiContainerViewModel("item 2"));
            Containers.Add(new MdiContainerViewModel("item 3"));

            SelectedContainer = Containers.First();
        }
        

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand =>
            _saveCommand ??
            (_saveCommand = new RelayCommand(() =>
            {
                BinaryFormatter formatter = new BinaryFormatter();
                string filename = Path.Combine(Directory.GetCurrentDirectory(), "setting.sbws");
                using (FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        formatter.Serialize(file, this);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }
            }));


        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand =>
            _loadCommand ??
            (_loadCommand = new RelayCommand(() =>
            {
                BinaryFormatter formatter = new BinaryFormatter();
                string filename = Path.Combine(Directory.GetCurrentDirectory(), "setting.sbws");
                using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    Console.WriteLine($"Selected pos: {SelectedContainer?.ViewModelCollection.FirstOrDefault().PositionLeft}");
                    Console.WriteLine($"Selected pos: {SelectedContainer?.ViewModelCollection.FirstOrDefault().PositionTop}");
                    
                }
            }));


        private RelayCommand _getInfo;
        public RelayCommand GetInfo =>
            _getInfo ??
            (_getInfo = new RelayCommand(() =>
            {
                foreach(var v in _containers)
                {
                    foreach(var w in v.ViewModelCollection)
                    {
                        Console.WriteLine($"{v.Title} -- {w?.Text} {w?.State}");
                    }
                }
            }));

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}

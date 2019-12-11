using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private MdiContainerViewModel _selectedContainer;
        public MdiContainerViewModel SelectedContainer
        {
            get => _selectedContainer;
            set
            {
                _selectedContainer = value;
                foreach(var item in _selectedContainer.ViewModelCollection)
                {
                    Console.WriteLine($"item: {item.CurrentLeft}x{item.CurrentTop}");
                }
                OnPropertyChanged(nameof(SelectedContainer));
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
                
            }));


        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand =>
            _loadCommand ??
            (_loadCommand = new RelayCommand(() =>
            {
                
            }));


        private RelayCommand _getInfo;
        public RelayCommand GetInfo =>
            _getInfo ??
            (_getInfo = new RelayCommand(() =>
            {
               
            }));

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }


    public class ButtonViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name //надо же хоть что-то сбиндить =)
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

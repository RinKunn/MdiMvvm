using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;

namespace MdiExample
{
    public class MdiContainerViewModel : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private bool _selected;
        public bool Selectedd
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selectedd));
            }
        }

        public ObservableCollection<Window1ViewModel> ViewModelCollection { get; private set; }

        protected MdiContainerViewModel()
        { }

        public MdiContainerViewModel(string title)
        {
            Title = title;
            ViewModelCollection = new ObservableCollection<Window1ViewModel>();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }



        private RelayCommand _addCommand;
        private RelayCommand _addCommandModal;
        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;


        [JsonIgnore]
        public RelayCommand AddCommand =>
            _addCommand ??
            (_addCommand = new RelayCommand(() =>
            {
                ViewModelCollection.Add(new Window1ViewModel() { IsModal = false, Title = "window_DefaultTitleDefaultTitleDefaultTitle" + Title + (ViewModelCollection.Count + 1) });
            }));
        
        [JsonIgnore]
        public RelayCommand AddCommandModal =>
            _addCommandModal ??
            (_addCommandModal = new RelayCommand(() =>
            {

                ViewModelCollection.Add(new Window1ViewModel() { IsModal = true, Title = "window_" + Title });

            }));

        [JsonIgnore]
        public RelayCommand SaveCommand =>
            _saveCommand ??
            (_saveCommand = new RelayCommand(() =>
            {
                this.Save();
            }));

        [JsonIgnore]
        public RelayCommand LoadCommand =>
            _loadCommand ??
            (_loadCommand = new RelayCommand(() =>
            {
                this.Load();
            }));


        private void Save()
        {
            
        }
        private void Load()
        {
           
        }
    }
}

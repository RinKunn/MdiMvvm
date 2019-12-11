using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight.CommandWpf;

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

        public ObservableCollection<Window1ViewModel> ViewModelCollection { get; }


        public MdiContainerViewModel(string title)
        {
            Title = title;
            ViewModelCollection = new ObservableCollection<Window1ViewModel>();
        }



        private int count = 1;
        private RelayCommand _addCommand;
        public RelayCommand AddCommand =>
            _addCommand ??
            (_addCommand = new RelayCommand(() =>
            {
                ViewModelCollection.Add(new Window1ViewModel() { IsModal = false, Title = "window_" + Title + count++ });
            }));
        private RelayCommand _addCommandModal;


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public RelayCommand AddCommandModal =>
            _addCommandModal ??
            (_addCommandModal = new RelayCommand(() =>
            {

                ViewModelCollection.Add(new Window1ViewModel() { IsModal = true, Title = "window_" + Title });

            }));

    }
}

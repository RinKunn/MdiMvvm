using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace MdiExample
{

    public class Window1ViewModel : INotifyPropertyChanged,  ISerializable
    {
        private bool _isModal;
        public bool IsModal
        {
            get => _isModal;
            set
            {
                _isModal = value;
                OnPropertyChanged(nameof(IsModal));
            }
        }
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        private double _heigh;
        public double Height
        {
            get => _heigh;
            set
            {
                _heigh = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private double _left;
        public double PositionLeft
        {
            get => _left;
            set
            {
                _left = value;
                OnPropertyChanged(nameof(PositionLeft));
            }
        }
        private double _top;
        public double PositionTop
        {
            get => _top;
            set
            {
                _top = value;
                OnPropertyChanged(nameof(PositionTop));
            }
        }
        private double _isSelected;
        public double IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        private WindowState _state;
        public WindowState State
        {
            get => _state;
            set
            {
                _state = value;
                Console.WriteLine($"State cahnged ----- {value.ToString()}");
                OnPropertyChanged(nameof(State));
            }
        }


        public Window1ViewModel(SerializationInfo info, StreamingContext context)
        {
            Text = (string)info.GetValue(nameof(Text), typeof(string));
            PositionTop = (double)info.GetValue(nameof(PositionTop), typeof(double));
            PositionLeft = (double)info.GetValue(nameof(PositionLeft), typeof(double));
            Height = (double)info.GetValue(nameof(Height), typeof(double));
            Width = (double)info.GetValue(nameof(Width), typeof(double));
        }

        public Window1ViewModel()
        {
            Random r = new Random();
            Text = "Window1ViewModel_" + r.Next(1, 1000);
            PositionTop = -1;
            PositionLeft = -1;
            Console.WriteLine($"Content '{Text}' Constructor: {IsSelected}");
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            Console.WriteLine($"'{Text}' prop changed '{name}' to {typeof(Window1ViewModel).GetProperty(name).GetValue(this, null).ToString()}");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Width), Width, typeof(double));
            info.AddValue(nameof(Height), Height, typeof(double));
            info.AddValue(nameof(Text), Text, typeof(string));
        }
    }
}

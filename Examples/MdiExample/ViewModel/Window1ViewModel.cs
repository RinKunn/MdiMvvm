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
using MdiMvvm.ViewModels;

namespace MdiExample
{

    public class Window1ViewModel : MdiWindowViewModelBase
    {
        private double _isSelected;
        public double IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
       
        public Window1ViewModel()
        {
            Random r = new Random();
            Title = "Window1ViewModel_" + r.Next(1, 1000);
            Console.WriteLine($"Content '{Title}' Constructor: {IsSelected}");
        }

    }
}

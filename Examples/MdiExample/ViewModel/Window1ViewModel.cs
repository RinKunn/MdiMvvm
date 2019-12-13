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
        public Window1ViewModel()
        {
            Random r = new Random();
            Title = "DefaultTitle" + r.Next(1, 1000);
            //Console.WriteLine($"Content '{Title}' Constructor: {IsSelected}");
        }
    }
}

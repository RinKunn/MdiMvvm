using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;

namespace MdiMvvm.ViewModels
{
    public abstract class MdiWindowViewModelBase : ViewModelBase
    {
        private double MinimizedLeft;
        private double MinimizedTop;

        private double PreviousLeft;
        private double PreviousTop;
        private double PreviousWidth;
        private double PreviousHeight;

        private double CurrentLeft;
        private double CurrentTop;
        private double CurrentWidth;
        private double CurrentHeight;

        private WindowState PreviousState;
        private WindowState WindowState;
    }
}

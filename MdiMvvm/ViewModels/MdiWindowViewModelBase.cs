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
        #region Members
        private double _previousLeft;
        private double _previousTop;
        private double _previousWidth;
        private double _previousHeight;
        private WindowState _previousState;

        private double _currentLeft;
        private double _currentTop;
        private double _currentWidth;
        private double _currentHeight;
        private WindowState _windowState; 
        #endregion

        #region Props
        public double PreviousLeft
        {
            get => _previousLeft;
            set
            {
                _previousLeft = value;
                RaisePropertyChanged(() => PreviousLeft);
            }
        }
        public double PreviousTop
        {
            get => _previousTop;
            set
            {
                _previousTop = value;
                RaisePropertyChanged(() => PreviousTop);
            }
        }
        public double PreviousWidth
        {
            get => _previousWidth;
            set
            {
                _previousWidth = value;
                RaisePropertyChanged(() => PreviousWidth);
            }
        }
        public double PreviousHeight
        {
            get => _previousHeight;
            set
            {
                _previousHeight = value;
                RaisePropertyChanged(() => PreviousHeight);
            }
        }
        public WindowState PreviousState
        {
            get => _previousState;
            set
            {
                _previousState = value;
                RaisePropertyChanged(() => PreviousState);
            }
        }
        public double CurrentLeft
        {
            get => _currentLeft;
            set
            {
                _currentLeft = value;
                RaisePropertyChanged(() => CurrentLeft);
            }
        }
        public double CurrentTop
        {
            get => _currentTop;
            set
            {
                _currentTop = value;
                RaisePropertyChanged(() => CurrentTop);
            }
        }
        public double CurrentWidth
        {
            get => _currentWidth;
            set
            {
                _currentWidth = value;
                RaisePropertyChanged(() => CurrentWidth);
            }
        }
        public double CurrentHeight
        {
            get => _currentHeight;
            set
            {
                _currentHeight = value;
                RaisePropertyChanged(() => CurrentHeight);
            }
        }
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                RaisePropertyChanged(() => WindowState);
            }
        } 
        #endregion

        public MdiWindowViewModelBase()
        {

        }

        public virtual string Retuddrn()
        {
            return null;
        }
    }
}

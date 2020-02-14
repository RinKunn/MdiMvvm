using System;
using System.Windows;
using Newtonsoft.Json;

namespace MdiMvvm.ViewModels
{
    public abstract class MdiWindowViewModelBase : ViewModelBase
    {
        #region Members

        private Guid _uid;
        private string _title;
        private bool _isModal;
        private bool _isSelected;
        private double _previousLeft;
        private double _previousTop;
        private double _previousWidth;
        private double _previousHeight;
        private WindowState _previousState;

        private double _currentLeft; // bind Canvas.Left
        private double _currentTop; // bind Canvas.Top
        private double _currentWidth;
        private double _currentHeight;
        private WindowState _windowState;
        
        [JsonIgnore]
        public MdiContainerViewModelBase Container;
        #endregion

        #region Props
        /// <summary>
        /// GUID of window
        /// </summary>
        [JsonIgnore]
        public Guid Guid => _uid;

        /// <summary>
        /// Title
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        /// <summary>
        /// Is window modal
        /// </summary>
        public bool IsModal
        {
            get => _isModal;
            set => Set(ref _isModal, value);
        }

        /// <summary>
        /// Is window selected at <see cref="MdiContainer"/>
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        /// <summary>
        /// Previous position's left
        /// </summary>
        public double PreviousLeft
        {
            get => _previousLeft;
            set => Set(ref _previousLeft, value);
        }

        /// <summary>
        /// Previous position's top
        /// </summary>
        public double PreviousTop
        {
            get => _previousTop;
            set => Set(ref _previousTop, value);
        }

        /// <summary>
        /// Previous position's width
        /// </summary>
        public double PreviousWidth
        {
            get => _previousWidth;
            set => Set(ref _previousWidth, value);
        }

        /// <summary>
        /// Previous position's height
        /// </summary>
        public double PreviousHeight
        {
            get => _previousHeight;
            set => Set(ref _previousHeight, value);
        }

        /// <summary>
        /// Previous position's window state
        /// </summary>
        public WindowState PreviousState
        {
            get => _previousState;
            set => Set(ref _previousState, value);
        }

        /// <summary>
        /// Current position's left
        /// </summary>
        public double CurrentLeft
        {
            get => _currentLeft;
            set => Set(ref _currentLeft, value);
        }

        /// <summary>
        /// Current position's top
        /// </summary>
        public double CurrentTop
        {
            get => _currentTop;
            set => Set(ref _currentTop, value);
        }

        /// <summary>
        /// Current position's width
        /// </summary>
        public double CurrentWidth
        {
            get => _currentWidth;
            set => Set(ref _currentWidth, value);
        }

        /// <summary>
        /// Current position's height
        /// </summary>
        public double CurrentHeight
        {
            get => _currentHeight;
            set => Set(ref _currentHeight, value);
        }

        /// <summary>
        /// Current position's window state
        /// </summary>
        public WindowState WindowState
        {
            get => _windowState;
            set => Set(ref _windowState, value);   
        }

        #endregion

        public MdiWindowViewModelBase()
        {
            _uid = Guid.NewGuid();
        }
    }
}

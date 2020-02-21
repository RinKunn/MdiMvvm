using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using MdiMvvm.AppCore.Services.WindowsServices.Store;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.Interfaces;

namespace MdiMvvm.AppCore.ViewModelsBase
{
    public abstract class MdiWindowViewModelBase : ViewModelBase, IMdiWindowViewModel, IStorable<WindowsStoreContext>, IBusy, INavigateAware
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
        private double _currentLeft;
        private double _currentTop;
        private double _currentWidth;
        private double _currentHeight;
        private WindowState _windowState;
        private IMdiContainerViewModel _container;
        private bool _isBusy;
        #endregion

        #region Props
        /// <summary>
        /// GUID of window
        /// </summary>
        public Guid Guid
        {
            get => _uid;
            set => Set(ref _uid, value);
        }

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

        /// <summary>
        /// Контейнер
        /// </summary>
        public IMdiContainerViewModel Container
        {
            get => _container;
            set => Set(ref _container, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }
        #endregion

        public MdiWindowViewModelBase()
        {
            _uid = Guid.NewGuid();
        }

        protected void Close()
        {
            Container?.RemoveMdiWindow(this);
        }

        public async Task<WindowsStoreContext> OnLoading(WindowsStoreContext context)
        {
            IsBusy = true;

            Guid = context.Guid;
            Title = context.Title;

            PreviousHeight = context.PreviousHeight;
            PreviousLeft = context.PreviousLeft;
            PreviousTop = context.PreviousTop;
            PreviousWidth = context.PreviousWidth;
            PreviousState = context.PreviousState;

            CurrentHeight = context.CurrentHeight;
            CurrentLeft = context.CurrentLeft;
            CurrentTop = context.CurrentTop;
            CurrentWidth = context.CurrentWidth;
            WindowState = context.WindowState;

            IsModal = context.IsModal;
            IsSelected = context.IsSelected;

            await OnWindowLoading(context.ViewModelContext);

            DispatcherHelper.CheckBeginInvokeOnUI(() => { IsBusy = false; });

            return context;
        }

        public async Task<WindowsStoreContext> OnKeeping(WindowsStoreContext context)
        {
            IsBusy = true;

            context.Guid = Guid;
            context.Title = Title;
            context.ViewModelType = GetType();

            context.PreviousHeight = PreviousHeight;
            context.PreviousLeft = PreviousLeft;
            context.PreviousTop = PreviousTop;
            context.PreviousWidth = PreviousWidth;
            context.PreviousState = PreviousState;

            context.CurrentHeight = CurrentHeight;
            context.CurrentLeft = CurrentLeft;
            context.CurrentTop = CurrentTop;
            context.CurrentWidth = CurrentWidth;
            context.WindowState = WindowState;

            context.IsModal = IsModal;
            context.IsSelected = IsSelected;
            context.ContainerGuid = Container.Guid;

            await OnWindowKeepeng(context.ViewModelContext);

            DispatcherHelper.CheckBeginInvokeOnUI(() => { IsBusy = false; });

            return context;
        }

        protected abstract Task OnWindowLoading(ViewModelContext context);
        protected abstract Task OnWindowKeepeng(ViewModelContext context);

        public abstract void NavigatedTo(ViewModelContext context);
    }
}

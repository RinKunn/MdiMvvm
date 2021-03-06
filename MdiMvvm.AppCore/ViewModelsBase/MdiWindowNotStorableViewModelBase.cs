﻿using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using MdiMvvm.AppCore.Services.WindowsServices.Store;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.Interfaces;
using System.Threading;
using System.Diagnostics;

namespace MdiMvvm.AppCore.ViewModelsBase
{
    public abstract class MdiWindowNotStorableViewModelBase : ViewModelBase, IMdiWindowViewModel, IBusy, INavigateAware
    {

        #region Members
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        CancellationToken token = new CancellationToken();
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
        private bool _isInited;
        private string _notificationMessage;

        public event EventHandler Closing;
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

        public bool IsInited
        {
            get => _isInited;
            private set => Set(ref _isInited, value);
        }

        public Action<NavigationResult> CallBackAction { get; set; }

        public bool IsSuccess => NotificationMessage != null && NotificationMessage.ToLower().Contains("успешно");

        public string NotificationMessage
        {
            get => _notificationMessage;
            protected set
            {
                _notificationMessage = value;
                RaisePropertyChanged(nameof(IsSuccess));
                RaisePropertyChanged(nameof(NotificationMessage));
            }
        }
        #endregion

        public MdiWindowNotStorableViewModelBase()
        {
            _uid = Guid.NewGuid();
            IsInited = false;
            token = cancelTokenSource.Token;
        }

        public void Close()
        {
            cancelTokenSource.Cancel();
            Closing?.Invoke(this, null);
            cancelTokenSource.Dispose();
        }

        public abstract void NavigatedTo(ViewModelContext context);

        public void RaiseCallBack(NavigationResult result)
        {
            CallBackAction?.Invoke(result);
        }

        public async Task InitAsync()
        {
            IsBusy = true;
            await OnIniting(token);
            IsBusy = false;
            IsInited = true;
        }

        protected virtual Task OnIniting(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}

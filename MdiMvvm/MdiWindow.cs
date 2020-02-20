using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MdiMvvm.Events;
using MdiMvvm.Extensions;
using MdiMvvm.WindowControls;

namespace MdiMvvm
{
    [TemplatePart(Name = "PART_Border", Type = typeof(Border))]
    [TemplatePart(Name = "PART_BorderGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Header", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ButtonBar", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_ButtonBar_CloseButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_ButtonBar_MaximizeButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_ButtonBar_MinimizeButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_BorderContent", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_MoverThumb", Type = typeof(MoveThumb))]
    [TemplatePart(Name = "PART_ResizerThumb", Type = typeof(ResizeThumb))]
    [DebuggerDisplay("{Title}")]

    public sealed class MdiWindow : ContentControl
    {
        internal static bool UseSnapshots = false;

        private const int WindowOffsetDiff = 25;

        private WindowButton _closeButton;
        private WindowButton _maximizeButton;
        private WindowButton _minimizeButton;

        private AdornerLayer _myAdornerLayer;
        private Adorner _myAdorner;

        internal MdiContainer Container { get; private set; }
        public ImageSource ImageSource { get; set; }
        public Image Tumblr { get; private set; }

        public MdiWindow()
        {
            _myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
        }

        static MdiWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MdiWindow), new FrameworkPropertyMetadata(typeof(MdiWindow)));
        }


        public void Initialize(MdiContainer container)
        {
            Container = container;
            Container.SizeChanged += Container_SizeChanged;
            this.Loaded += MdiWindow_Loaded;
            if (string.IsNullOrEmpty(Uid)) this.Uid = Guid.NewGuid().ToString();
        }

        [Obsolete]
        public void InitPositionn()
        {
            if (this.WindowState == WindowState.Maximized || (Width != 0 && Height != 0)) return;

            var actualContainerHeight = Container.ActualHeight;
            var actualContainerWidth = Container.ActualWidth;
            UpdateLayout();
            InvalidateMeasure();
            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            PreviousWidth = ActualWidth;
            PreviousHeight = ActualHeight;

            var left = Math.Max(0, (actualContainerWidth - actualWidth) / 2);
            var top = Math.Max(0, (actualContainerHeight - actualHeight) / 2);

            SetValue(Canvas.LeftProperty, left);
            SetValue(Canvas.TopProperty, top);

            PreviousLeft = left;
            PreviousTop = top;

        }

        public void InitPosition()
        {
            if (this.WindowState == WindowState.Maximized || (Width != 0 && Height != 0)) return;

            var actualContainerHeight = Container.ActualHeight;
            var actualContainerWidth = Container.ActualWidth;

            UpdateLayout();
            InvalidateMeasure();
            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            PreviousWidth = ActualWidth;
            PreviousHeight = ActualHeight;

            double left = Math.Max(0, Container.WindowsOffset);
            double top = Math.Max(0, Container.WindowsOffset);

            SetValue(Canvas.LeftProperty, left);
            SetValue(Canvas.TopProperty, top);


            PreviousLeft = left;
            PreviousTop = top;

            Container.WindowsOffset += WindowOffsetDiff;

            if (Container.WindowsOffset + actualHeight > actualContainerHeight || Container.WindowsOffset + actualWidth > actualContainerWidth)
                Container.WindowsOffset = 5;
        }



        #region Overrides

        public override void OnApplyTemplate()
        {
            _closeButton = GetTemplateChild("PART_ButtonBar_CloseButton") as WindowButton;
            if (_closeButton != null)
            {
                _closeButton.Click += CloseWindow;
            }

            _maximizeButton = GetTemplateChild("PART_ButtonBar_MaximizeButton") as WindowButton;
            if (_maximizeButton != null)
            {
                _maximizeButton.Click += ToggleMaximizeWindow;
            }

            _minimizeButton = GetTemplateChild("PART_ButtonBar_MinimizeButton") as WindowButton;
            if (_minimizeButton != null)
            {
                _minimizeButton.Click += ToggleMinimizeWindow;
            }

            Tumblr = GetTemplateChild("PART_Tumblr") as Image;
        }

        #endregion

        #region RoutedEvents

        #region Registration
        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(
            "Closing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MdiWindow));

        public static readonly RoutedEvent FocusChangedEvent = EventManager.RegisterRoutedEvent(
           "FocusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MdiWindow));

        public static readonly RoutedEvent WindowStateChangedEvent = EventManager.RegisterRoutedEvent(
           "WindowStateChanged", RoutingStrategy.Bubble, typeof(WindowStateChangedRoutedEventHandler), typeof(MdiWindow));
        #endregion

        #region Properties
        public event WindowStateChangedRoutedEventHandler WindowStateChanged
        {
            add { AddHandler(WindowStateChangedEvent, value); }
            remove { RemoveHandler(WindowStateChangedEvent, value); }
        }

        public event RoutedEventHandler Closing
        {
            add { AddHandler(ClosingEvent, value); }
            remove { RemoveHandler(ClosingEvent, value); }
        }

        public event RoutedEventHandler FocusChanged
        {
            add { AddHandler(FocusChangedEvent, value); }
            remove { RemoveHandler(FocusChangedEvent, value); }
        }
        #endregion

        public delegate void WindowStateChangedRoutedEventHandler(object sender, WindowStateChangedEventArgs e);
        #endregion

        #region DependencyProperties

        #region Registration

        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register("Title", typeof(string), typeof(MdiWindow), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.Register("WindowState", typeof(WindowState), typeof(MdiWindow), new UIPropertyMetadata(WindowState.Normal, IsWindowStateChangedCallBack));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MdiWindow), new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register("IsModal", typeof(bool?), typeof(MdiWindow), new UIPropertyMetadata(false, IsModalChangedCallback));

        public static readonly DependencyProperty IsCloseButtonEnabledProperty =
            DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(MdiWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.NotDataBindable));

        public static readonly DependencyProperty HasDropShadowProperty =
            DependencyProperty.Register("HasDropShadow", typeof(bool), typeof(MdiWindow), new UIPropertyMetadata(true));

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(MdiWindow), new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty IsResizableProperty =
            DependencyProperty.Register("IsResizable", typeof(bool), typeof(MdiWindow), new UIPropertyMetadata(IsResizableChangedCallback));

        public static readonly DependencyProperty PreviousLeftProperty =
            DependencyProperty.Register("PreviousLeft", typeof(double), typeof(MdiWindow), new FrameworkPropertyMetadata(0D));

        public static readonly DependencyProperty PreviousTopProperty =
            DependencyProperty.Register("PreviousTop", typeof(double), typeof(MdiWindow), new FrameworkPropertyMetadata(0D));

        public static readonly DependencyProperty PreviousWidthProperty =
            DependencyProperty.Register("PreviousWidth", typeof(double), typeof(MdiWindow), new FrameworkPropertyMetadata(100D));

        public static readonly DependencyProperty PreviousHeightProperty =
            DependencyProperty.Register("PreviousHeight", typeof(double), typeof(MdiWindow), new FrameworkPropertyMetadata(100D));

        public static readonly DependencyProperty PreviousWindowStateProperty =
            DependencyProperty.Register("PreviousWindowState", typeof(WindowState), typeof(MdiWindow), new FrameworkPropertyMetadata(WindowState.Normal));


        //public static readonly DependencyProperty ScreenshotProperty =
        //    DependencyProperty.Register("Screenshot", typeof(byte[]), typeof(MdiWindow), new FrameworkPropertyMetadata(WindowState.Normal));
        #endregion

        #region Properties
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public WindowState WindowState
        {
            get { return (WindowState)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }

        public double PreviousLeft
        {
            get { return (double)GetValue(PreviousLeftProperty); }
            set { SetValue(PreviousLeftProperty, value); }
        }
        public double PreviousTop
        {
            get { return (double)GetValue(PreviousTopProperty); }
            set { SetValue(PreviousTopProperty, value); }
        }
        public double PreviousWidth
        {
            get { return (double)GetValue(PreviousWidthProperty); }
            set { SetValue(PreviousWidthProperty, value); }
        }
        public double PreviousHeight
        {
            get { return (double)GetValue(PreviousHeightProperty); }
            set { SetValue(PreviousHeightProperty, value); }
        }
        public WindowState PreviousWindowState
        {
            get { return (WindowState)GetValue(PreviousWindowStateProperty); }
            set { SetValue(PreviousWindowStateProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public bool HasDropShadow
        {
            get { return (bool)GetValue(HasDropShadowProperty); }
            set { SetValue(HasDropShadowProperty, value); }
        }
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }
        public bool IsResizable
        {
            get { return (bool)GetValue(IsResizableProperty); }
            set
            {
                if (!value)
                {
                    Height = double.NaN;
                    Width = double.NaN;
                }
                SetValue(IsResizableProperty, value);
            }
        }
        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCloseButtonEnabled
        {
            get { return (bool)GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, value); }
        }
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set
            {
                if (value)
                {
                    if (_myAdornerLayer == null)
                        _myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (_myAdorner == null)
                    {
                        _myAdorner = new HollowRectangleAdorner(this);
                    }
                    _myAdornerLayer.Add(_myAdorner);
                }
                else
                {
                    _myAdornerLayer?.Remove(_myAdorner);
                }
                if (Container != null)
                    Container.IsModal = value;

                SetValue(IsModalProperty, value);
            }
        }

        //public byte[] Screenshot
        //{
        //    get { return (byte[])GetValue(ScreenshotProperty); }
        //    set { SetValue(ScreenshotProperty, value); }
        //}
        #endregion

        #region Callbacks
        private static void IsWindowStateChangedCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as MdiWindow;
            if (window != null)
            {
                //window._logger.Trace($"IsWindowStateChangedCallBack: {e.OldValue} to {e.NewValue}");
                window.PreviousWindowState = (WindowState)e.OldValue;

                var args = new WindowStateChangedEventArgs(WindowStateChangedEvent, (WindowState)e.OldValue, (WindowState)e.NewValue);
                window.RaiseEvent(args);
            }
        }

        private static void IsModalChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ((MdiWindow)d).IsModal = (bool)e.NewValue;
        }

        private static void IsResizableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool)e.NewValue))
            {
                ((MdiWindow)d).Height = double.NaN;
                ((MdiWindow)d).Width = double.NaN;
            }

            if (e.NewValue == null) return;
            ((MdiWindow)d).IsResizable = (bool)e.NewValue;
        }

        private static void IsSelectedChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as MdiWindow;
            if (window != null)
            {
                if ((bool)e.NewValue == (bool)e.OldValue) return;

                if (((bool)e.NewValue) == true)
                {
                    Panel.SetZIndex(window, 2);
                    window.RaiseEvent(new RoutedEventArgs(FocusChangedEvent, window.DataContext));
                    if (!window.IsFocused) window.Focus();
                }
                else
                {
                    Panel.SetZIndex(window, 0);
                    window.RaiseEvent(new RoutedEventArgs(FocusChangedEvent, window.DataContext));
                }
            }
        }
        #endregion

        #endregion

        #region Container Events Handlers

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                //Width += e.NewSize.Width - e.PreviousSize.Width;
                Width = e.NewSize.Width - 2;
                Height = e.NewSize.Height - 2;
                //Height += e.NewSize.Height - e.PreviousSize.Height;
                this.RemoveWindowLock();
            }
        }

        #endregion

        #region MdiWindow Event Handlers

        private void MdiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MdiWindow window = sender as MdiWindow;
            var content = VisualTreeExtension.FindContent(window);
            if (content != null)
            {
                window.MinHeight = content.MinHeight + 34;
                window.MinWidth = content.MinWidth + 10;

                window.Height = Math.Max(content.ActualHeight + 34, ActualHeight);
                window.Width = Math.Max(content.ActualWidth + 10, ActualWidth);
                if(window.PreviousHeight == 0) window.PreviousHeight = window.Height;
                if (window.PreviousWidth == 0) window.PreviousWidth = window.Width;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
            Panel.SetZIndex(this, 2);
            RaiseEvent(new RoutedEventArgs(FocusChangedEvent, DataContext));
            Focus();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            FrameworkElement parent = VisualTreeExtension.FindMdiWindow(e.NewFocus as FrameworkElement);
            if ((e.NewFocus is MdiWindow && !Equals(e.NewFocus, this)) || (parent != null && !Equals(parent, this)))
            {
                IsSelected = false;
                Panel.SetZIndex(this, 0);
                var newWindow = (e.NewFocus is MdiWindow) ? (e.NewFocus as MdiWindow) : (parent as MdiWindow);
                Container.SetValue(MdiContainer.SelectedItemProperty, newWindow.DataContext);
                newWindow.IsSelected = true;
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            IsSelected = true;
            Panel.SetZIndex(this, 2);
            RaiseEvent(new RoutedEventArgs(FocusChangedEvent, DataContext));
        }

        private void ToggleMaximizeWindow(object sender, RoutedEventArgs e)
        {
            Focus();
            this.ToggleMaximize();
        }

        private void ToggleMinimizeWindow(object sender, RoutedEventArgs e)
        {
            Focus();
            this.ToggleMinimize();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var canCloseBinding = BindingOperations.GetBindingExpression(this, CanCloseProperty);
            if (canCloseBinding != null)
            {
                canCloseBinding.UpdateTarget();
            }

            if (CanClose)
            {
                RaiseEvent(new RoutedEventArgs(ClosingEvent));
            }
        }

        #endregion

        public void DoFocus(MouseButtonEventArgs mouseButtonEventArgs)
        {
            OnMouseLeftButtonDown(mouseButtonEventArgs);
        }

    }
}

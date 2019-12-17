using MdiMvvm.Events;
using MdiMvvm.Extensions;
using MdiMvvm.ValueObjects;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using NLog;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace MdiMvvm
{
    [TemplatePart(Name = "PART_ContainerScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_ContainerMinWin_ListBox", Type = typeof(ListBox))]
    public sealed class MdiContainer : Selector
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private ScrollViewer ContainerScrollViewer;
        private ListBox ContainerMinWinListox;
        private Canvas ContainerCanvas;
        private MdiWindow _maximizedWindow;
        private IList _internalItemSource;

        internal int WindowsOffset = 5;

        private MdiWindow MaximizedWindow
        {
            get => _maximizedWindow;
            set
            {
                if (object.ReferenceEquals(value, _maximizedWindow)) return;
                var buf = _maximizedWindow;
                _maximizedWindow = value;
                if (value == null)
                {
                    EnableContainerScroll();
                    ContainerMinWinListox.Visibility = Visibility.Visible;
                    InvalidateSize(buf);
                }
                else if(buf == null)
                {
                    EnableContainerScroll(false);
                    ContainerMinWinListox.Visibility = Visibility.Collapsed;
                    _maximizedWindow.IsSelected = true;
                    _maximizedWindow.Width = ActualWidth-2;
                    _maximizedWindow.Height = ActualHeight-2;
                    _maximizedWindow.DoFocus(null);
                }
            }
        }
        private MinimizedWindowCollection MinimizedWindows;
        
        static MdiContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MdiContainer), new FrameworkPropertyMetadata(typeof(MdiContainer)));
        }

        public MdiContainer() : base()
        {
            this.Loaded += MdiContainer_Loaded;
            this.SelectionChanged += MdiContainer_SelectionChanged;
            this.SizeChanged += MdiContainer_SizeChanged;
            ((INotifyCollectionChanged)Items).CollectionChanged += MdiContainer_CollectionChanged;

            string snapsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gb_mdi");
            if (!Directory.Exists(snapsPath)) Directory.CreateDirectory(snapsPath);
        }

        private void MdiContainer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            //_logger.Trace($"MdiContainer_CollectionChanged: {e.Action}");
            if (e.Action == NotifyCollectionChangedAction.Add && MaximizedWindow != null) MaximizedWindow.Normalize();
        }


        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContainerScrollViewer = GetTemplateChild("PART_ContainerScrollViewer") as ScrollViewer;
            ContainerMinWinListox = GetTemplateChild("PART_ContainerMinWin_ListBox") as ListBox;
            MinimizedWindows = new MinimizedWindowCollection(ContainerMinWinListox);
            ContainerMinWinListox.ItemsSource = MinimizedWindows;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MdiWindow();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is MdiWindow window)
            {
                //_logger.Trace($"PrepareContainerForItemOverride: new MdiWindow added '{window.Title}' : {window.WindowState}");
                window.FocusChanged += OnMdiWindowFocusChanged;
                window.WindowStateChanged += OnMdiWindowStateChanged;
                window.Closing += OnMdiWindowClosing;
                window.Unloaded += OnMdiWindow_Unloaded;

                window.Initialize(this);
                window.InitPosition();

                window.Focus();
            }
            base.PrepareContainerForItemOverride(element, item);
        }
        
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            _logger.Trace($"OnItemsSourceChanged: oldSouce = {(oldValue == null ? "null" : $"{(oldValue as IList).Count}")}, newSouce = {(newValue == null ? "null" : $"{(newValue as IList).Count}")}");
            base.OnItemsSourceChanged(oldValue, newValue);
            WindowsOffset = 5;
            if (newValue != null && newValue is IList)
            {
                _internalItemSource = newValue as IList;
                if (oldValue != null) RenderSourceItems();
                else InvalidateSize();
            }
        }

        

        #endregion

        #region Dependencies

        #region Properties
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }
        #endregion

        #region Registers

        public static readonly DependencyProperty IsModalProperty =
                DependencyProperty.Register("IsModal", typeof(bool?), typeof(MdiContainer), new UIPropertyMetadata(IsModalChangedCallback));

        #endregion

        #region Callbacks
        private static void IsModalChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ((MdiContainer)d).IsModal = (bool)e.NewValue;
        }
        #endregion 

        #endregion

        #region MdiContainer Events Handles

        private void MdiContainer_Loaded(object sender, RoutedEventArgs e)
        {
            ContainerCanvas = VisualTreeExtension.FindItemPresenterChild<Canvas>(this);
            RenderSourceItems();
        }
        private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateSize();
        }
        private void MdiContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]) is MdiWindow selectedWindow)
                    selectedWindow.IsSelected = true;
            }
            if (e.RemovedItems.Count > 0)
            {
                if (ItemContainerGenerator.ContainerFromItem(e.RemovedItems[0]) is MdiWindow unSelectedWindow)
                    unSelectedWindow.IsSelected = false;
            }
        }

        #endregion

        #region MdiWindow Event Handles

        private void OnMdiWindowStateChanged(object sender, WindowStateChangedEventArgs e)
        {
            if (!(sender is MdiWindow window)) throw new NullReferenceException($"Sender in OnMdiWindowStateChanged is not {typeof(MdiWindow).Name} ");

            _logger.Trace($"OnMdiWindowStateChanged: Window '{window.Title}' changed state from '{e.OldValue}' to '{e.NewValue}'");
            if (e.NewValue == WindowState.Minimized)
            {
                MinimizedWindows.Add(window);
            }
            else if (e.OldValue == WindowState.Minimized)
            {
                MinimizedWindows.Remove(window);
            }

            if (e.NewValue == WindowState.Maximized)
            {
                MaximizedWindow = window;
            }
            else if (e.OldValue == WindowState.Maximized)
            {
                MaximizedWindow = null;
            }
        }

        private void OnMdiWindowClosing(object sender, RoutedEventArgs e)
        {
            var window = sender as MdiWindow;
            _logger.Trace($"OnMdiWindowClosing: Window '{window.Title}'");
            if (window.WindowState == WindowState.Maximized)
                MaximizedWindow = null;
            if (window?.DataContext != null)
            {
                _internalItemSource?.Remove(window.DataContext);
                if (Items.Count > 0)
                {
                    SelectedItem = Items[Items.Count - 1];
                    if (ItemContainerGenerator.ContainerFromItem(SelectedItem) is MdiWindow windowNew) windowNew.IsSelected = true;
                }

                // clear
                window.FocusChanged -= OnMdiWindowFocusChanged;
                window.Closing -= OnMdiWindowClosing;
                window.WindowStateChanged -= OnMdiWindowStateChanged;
                window.DataContext = null;
                InvalidateSize();
            }
        }

        private void OnMdiWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is MdiWindow window)
            {
                _logger.Trace($"OnMdiWindow_Unloaded: Window '{window.Title}': {window.WindowState}");
                if (window.WindowState == WindowState.Maximized) MaximizedWindow = null;
                window.FocusChanged -= OnMdiWindowFocusChanged;
                window.Closing -= OnMdiWindowClosing;
                window.WindowStateChanged -= OnMdiWindowStateChanged;
                window.DataContext = null;
            }
        }

        private void OnMdiWindowFocusChanged(object sender, RoutedEventArgs e)
        {
            if (((MdiWindow)sender).IsFocused)
            {
                foreach (var item in Items)
                {
                    if (item != e.OriginalSource)
                    {
                        if (ItemContainerGenerator.ContainerFromItem(item) is MdiWindow window)
                        {
                            window.IsSelected = false;
                            Panel.SetZIndex(window, 0);
                        }
                    }
                }
                SelectedItem = e.OriginalSource;
                ((MdiWindow)ItemContainerGenerator.ContainerFromItem(SelectedItem)).IsSelected = true;
            }
        }

        private void OnMinimizedWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MdiWindow window = (MdiWindow)(sender as ListBoxItem).DataContext;
            if (window != null)
            {
                window.Normalize();
            }
        }

        #endregion

        private void RenderSourceItems()
        {
            MinimizedWindows?.Clear();
            if (Items != null && Items.Count > 0)
            {
                MdiWindow maxwin = null;
                foreach (var item in Items)
                {
                    MdiWindow window = ItemContainerGenerator.ContainerFromItem(item) as MdiWindow;
                    if (window.WindowState == WindowState.Minimized)
                    {
                        MinimizedWindows.Add(window);
                        var snap = window.LoadSnapshot();
                        if (snap != null) window.ImageSource = snap;
                    }
                    else if (window.WindowState == WindowState.Maximized)
                    {
                        maxwin = window;
                    }
                }

                _logger.Trace($"OnItemsSourceChanged: Max Window = {(maxwin == null ? "null" : $"{maxwin.Title}")}");
                _logger.Trace($"OnItemsSourceChanged: Min Window = {MinimizedWindows.Count}");
                MaximizedWindow = maxwin;
            }
            InvalidateSize();
            //for (int i = 0; i < MinimizedWindows.Count; i++)
            //{
            //    var item = MinimizedWindows[i];
            //    var snap = item.LoadSnapshot();
            //    if (snap != null) item.ImageSource = snap;
            //}
        }

        internal void InvalidateSize(MdiWindow currWindow = null)
        {
            const int windowMargin = 5;
            if (ContainerCanvas == null || _maximizedWindow != null) return;

            Point largestPoint = new Point(this.ActualWidth - 5, this.ActualHeight - 5);
            
            if(currWindow != null)
            {
                double winRight = Canvas.GetLeft(currWindow) + currWindow.Width + windowMargin;
                double winBottom = Canvas.GetTop(currWindow) + currWindow.Height + windowMargin;
                largestPoint.X = largestPoint.X > winRight ? largestPoint.X : winRight;
                largestPoint.Y = largestPoint.Y > winBottom ? largestPoint.Y : winBottom;
            }
            else
            {
                if (Items.Count > 0)
                {
                    foreach (var item in Items)
                    {
                        if (!(ItemContainerGenerator.ContainerFromItem(item) is MdiWindow window)) return;
                        if (window.WindowState == WindowState.Minimized) continue;
                        
                        Point farPosition = new Point(Canvas.GetLeft(window) + window.Width+ windowMargin, Canvas.GetTop(window) + window.Height+ windowMargin);

                        if (farPosition.X > largestPoint.X)
                            largestPoint.X = farPosition.X;

                        if (farPosition.Y > largestPoint.Y)
                            largestPoint.Y = farPosition.Y;
                    }
                }
            }
            //Console.WriteLine($"InvalidateSize: container:[{ContainerCanvas.Width}, {ContainerCanvas.Height}] bifsize: [{largestPoint.X}, {largestPoint.Y}]");
            if (ContainerCanvas.Width != largestPoint.X) ContainerCanvas.Width = largestPoint.X;
            if (ContainerCanvas.Height != largestPoint.Y) ContainerCanvas.Height = largestPoint.Y;
        }

        private void EnableContainerScroll(bool enable = true)
        {
            ContainerScrollViewer.VerticalScrollBarVisibility = enable ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
            ContainerScrollViewer.HorizontalScrollBarVisibility = enable ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        }

    }
}

using MdiMvvm.Events;
using MdiMvvm.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace MdiMvvm
{
    [TemplatePart(Name = "PART_ContainerScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_ContainerMinWin_ListBox", Type = typeof(ListBox))]
    public sealed class MdiContainer : Selector
    {
        private ScrollViewer ContainerScrollViewer;
        private ListBox ContainerMinWinListox;
        private IList _internalItemSource;
        private Canvas ContainerCanvas;
        private MdiWindow _maximizedWindow;

        internal ObservableCollection<MdiWindow> _minimizedWindowsCollection;
        
        static MdiContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MdiContainer), new FrameworkPropertyMetadata(typeof(MdiContainer)));
            
        }

        public MdiContainer() : base()
        {
            _minimizedWindowsCollection = new ObservableCollection<MdiWindow>();
            var r = this.ItemsSource;
            this.Loaded += MdiContainer_Loaded;
            this.SelectionChanged += MdiContainer_SelectionChanged;
            this.SizeChanged += MdiContainer_SizeChanged;
        }


        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContainerScrollViewer = GetTemplateChild("PART_ContainerScrollViewer") as ScrollViewer;
            ContainerMinWinListox = GetTemplateChild("PART_ContainerMinWin_ListBox") as ListBox;
            ContainerMinWinListox.ItemsSource = _minimizedWindowsCollection;
        }

        
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MdiWindow();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var window = element as MdiWindow;
            if (window != null)
            {
                window.FocusChanged += OnMdiWindowFocusChanged;
                window.WindowStateChanged += OnMdiWindowStateChanged;
                window.Closing += OnMdiWindowClosing;

                window.Initialize(this);

                window.InitPosition();

                if (_maximizedWindow != null)
                    _maximizedWindow.Normalize();

                window.Focus();
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue != null && newValue is IList)
            {
                _internalItemSource = newValue as IList;
                InvalidateSize();
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

        #region Events handlers

        private void MdiContainer_Loaded(object sender, RoutedEventArgs e)
        {

            ContainerCanvas = VisualTreeExtension.FindItemPresenterChild<Canvas>(this);
            InvalidateSize();
            Console.WriteLine($"MdiContainer_Loaded:  ");
            counter++;
            if (counter == 1) ContainerCanvas.Background = Brushes.Gray;
            else if (counter == 2) ContainerCanvas.Background = Brushes.Red;
            else ContainerCanvas.Background = Brushes.Blue;
        }
        private static int counter = 0;
        private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateSize();
        }

        private void MdiContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedWindow = ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]) as MdiWindow;
                if (selectedWindow != null)
                    selectedWindow.IsSelected = true;
            }
            if (e.RemovedItems.Count > 0)
            {
                var unSelectedWindow = ItemContainerGenerator.ContainerFromItem(e.RemovedItems[0]) as MdiWindow;
                if (unSelectedWindow != null)
                    unSelectedWindow.IsSelected = false;
            }
        }

        #endregion

        #region MdiWindow Event Handles

        private void OnMdiWindowStateChanged(object sender, WindowStateChangedEventArgs e)
        {
            MdiWindow window = sender as MdiWindow;
            if (window == null) throw new NullReferenceException($"Sender in OnMdiWindowStateChanged is not {typeof(MdiWindow).Name} ");

            if (e.NewValue == WindowState.Minimized)
            {
                _minimizedWindowsCollection.Add(window);
                ListBoxItem lbi = (ListBoxItem)ContainerMinWinListox.ItemContainerGenerator.ContainerFromItem(window);
                lbi.MouseDoubleClick += MinimizedWindow_MouseDoubleClick;
            }
            else if (e.OldValue == WindowState.Minimized)
            {
                ListBoxItem lbi = (ListBoxItem)ContainerMinWinListox.ItemContainerGenerator.ContainerFromItem(window);
                lbi.MouseDoubleClick -= MinimizedWindow_MouseDoubleClick;
                _minimizedWindowsCollection.Remove(window);
            }

            if (e.NewValue == WindowState.Maximized)
            {
                EnableContainerScroll(false);
                ContainerMinWinListox.Visibility = Visibility.Collapsed;
                _maximizedWindow = window;
            }
            else if (e.OldValue == WindowState.Maximized)
            {
                ContainerMinWinListox.Visibility = Visibility.Visible;
                EnableContainerScroll();
                _maximizedWindow = null;
            }
        }

        private void OnMdiWindowClosing(object sender, RoutedEventArgs e)
        {
            var window = sender as MdiWindow;
            if (window?.DataContext != null)
            {
                if(window.WindowState == WindowState.Maximized)
                {
                    ContainerMinWinListox.Visibility = Visibility.Visible;
                    EnableContainerScroll();
                    _maximizedWindow = null;
                }

                _internalItemSource?.Remove(window.DataContext);
                if (Items.Count > 0)
                {
                    SelectedItem = Items[Items.Count - 1];
                    var windowNew = ItemContainerGenerator.ContainerFromItem(SelectedItem) as MdiWindow;
                    if (windowNew != null) windowNew.IsSelected = true;
                }

                // clear
                window.FocusChanged -= OnMdiWindowFocusChanged;
                window.Closing -= OnMdiWindowClosing;
                window.WindowStateChanged -= OnMdiWindowStateChanged;
                window.DataContext = null;
                InvalidateSize();
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
                        var window = ItemContainerGenerator.ContainerFromItem(item) as MdiWindow;
                        if (window != null)
                        {
                            window.IsSelected = false;
                            if (window.WindowState == WindowState.Maximized)
                                window.WindowState = window.PreviousWindowState;
                            Panel.SetZIndex(window, 0);
                        }
                    }
                }
                SelectedItem = e.OriginalSource;
                ((MdiWindow)ItemContainerGenerator.ContainerFromItem(SelectedItem)).IsSelected = true;
            }
        }

        private void MinimizedWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MdiWindow window = (MdiWindow)(sender as ListBoxItem).DataContext;
            if (window != null)
            {
                window.Normalize();
            }
        }
        #endregion

        internal void InvalidateSize(MdiWindow currWindow = null)
        {
            if (ContainerCanvas == null || _maximizedWindow != null) return;

            Point largestPoint = new Point(ActualWidth - 5, ActualHeight - 5);
            
            if(currWindow != null)
            {
                double winRight = Canvas.GetLeft(currWindow) + currWindow.ActualWidth;
                double winBottom = Canvas.GetTop(currWindow) + currWindow.ActualHeight;
                largestPoint.X = largestPoint.X > winRight ? largestPoint.X : winRight;
                largestPoint.Y = largestPoint.Y > winBottom ? largestPoint.Y : winBottom;
            }
            else
            {
                if (Items.Count > 0)
                {
                    foreach (var item in Items)
                    {
                        MdiWindow window = ItemContainerGenerator.ContainerFromItem(item) as MdiWindow;
                        if (window == null) return;

                        Point farPosition = new Point(Canvas.GetLeft(window), Canvas.GetTop(window));

                        if (window.WindowState == WindowState.Minimized) 
                            continue;
                        else
                        {
                            farPosition.X += window.ActualWidth;
                            farPosition.Y += window.ActualHeight;
                        }

                        if (farPosition.X > largestPoint.X)
                            largestPoint.X = farPosition.X;

                        if (farPosition.Y > largestPoint.Y)
                            largestPoint.Y = farPosition.Y;
                    }
                }
            }

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

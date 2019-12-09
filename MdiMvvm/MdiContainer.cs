using MdiMvvm.Events;
using MdiMvvm.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;

namespace MdiMvvm
{
    [TemplatePart(Name = "PART_ContainerScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_ContainerMinWin_ListBox", Type = typeof(ListBox))]
    public sealed class MdiContainer : Selector
    {
        internal ScrollViewer ContainerScrollViewer;
        internal ListBox ContainerMinWinListox;
        private IList _internalItemSource;
        internal Canvas ContainerCanvas;


        private MdiWindow _maximizedWindow;
        internal ObservableCollection<MdiWindow> _minimizedWindowsCollection;
        private readonly int _containerRowCapacity;


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

            _containerRowCapacity = (int)(SystemParameters.VirtualScreenWidth / MdiWindow.MINIMIZED_WINDOW_WIDTH);
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
        }


        private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateSize();

            int newContainerRowCapacity = (int)(e.NewSize.Width / MdiWindow.MINIMIZED_WINDOW_WIDTH);
            if (newContainerRowCapacity != _containerRowCapacity)
            {
                RearrangeMinimizedWindows();
            }
            RearrangeMinimizedWindows();
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

        #region Minimized windows methods

        private void AddMinimizedWindow(MdiWindow window)
        {
            _minimizedWindowsCollection.Add(window);
            //RearrangeMinimizedWindows(_minimizedWindowsCollection.Count - 1);
        }

        private void RemoveMinimizedWindow(MdiWindow window)
        {
            _minimizedWindowsCollection.Remove(window);
            //int index = _minimizedWindowsCollection.IndexOf(window);
            //_minimizedWindowsCollection.Remove(window);
            //// handle situation when removed item was last in collection
            //if (index < _minimizedWindowsCollection.Count)
            //    RearrangeMinimizedWindows(index);
        }

        /// <summary>
        /// Render Collection of minimized windows
        /// </summary>
        /// <param name="startIndex"></param>
        internal void RearrangeMinimizedWindows(int startIndex = 0)
        {
            //_containerRowCapacity = (int)(ActualWidth / MdiWindow.MINIMIZED_WINDOW_WIDTH);

            for (int i = startIndex; i < _minimizedWindowsCollection.Count; i++)
            {
                int newWindowRowPlacemnt = (i / _containerRowCapacity) + 1;
                int newWindowColumnPlacemnt = i % _containerRowCapacity;
                Canvas.SetTop(_minimizedWindowsCollection[i], ActualHeight - newWindowRowPlacemnt * MdiWindow.MINIMIZED_WINDOW_HEIGHT);
                Canvas.SetLeft(_minimizedWindowsCollection[i], newWindowColumnPlacemnt * MdiWindow.MINIMIZED_WINDOW_WIDTH);
            }
        }

        #endregion

        #region MdiWindow Event Handles

        private void OnMdiWindowStateChanged(object sender, WindowStateChangedEventArgs e)
        {
            MdiWindow window = sender as MdiWindow;
            if (e.NewValue == WindowState.Minimized)
            {
                AddMinimizedWindow(window);
            }
            else if (e.OldValue == WindowState.Minimized)
            {
                RemoveMinimizedWindow(window);
            }

            if (e.NewValue == WindowState.Maximized)
            {
                EnableContainerScroll(false);
                _maximizedWindow = window;
            }
            else if (e.OldValue == WindowState.Maximized)
            {
                EnableContainerScroll();
                _maximizedWindow = null;
            }
        }

        private void OnMdiWindowClosing(object sender, RoutedEventArgs e)
        {
            //TODO: если состояние Minimized и index = не последний, то перерисовать минимум окна или при добавлении
            var window = sender as MdiWindow;
            if (window?.DataContext != null)
            {
                if(window.WindowState == WindowState.Minimized) RemoveMinimizedWindow(window);
                if(window.WindowState == WindowState.Maximized) EnableContainerScroll();

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

                        Point farPosition = new Point(Canvas.GetLeft(window), Canvas.GetTop(window));

                        if (window.WindowState == WindowState.Minimized)
                        {
                            farPosition.X += MdiWindow.MINIMIZED_WINDOW_WIDTH;
                            farPosition.Y += MdiWindow.MINIMIZED_WINDOW_HEIGHT;
                        }
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

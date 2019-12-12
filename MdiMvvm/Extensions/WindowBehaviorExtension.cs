using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MdiMvvm.Extensions
{
    internal static class WindowBehaviorExtension
    {
        //private static int ANIMATED_MILLISECONDS_DURATION = 0;

        private static Logger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Save current <see cref="WindowState.Normal"/> state of <see cref="MdiWindow"/>
        /// </summary>
        /// <param name="window"></param>
        private static void SavePreviousPosition(this MdiWindow window)
        {
            window.PreviousLeft = Canvas.GetLeft(window);
            window.PreviousTop = Canvas.GetTop(window);
            window.PreviousWidth = window.ActualWidth;
            window.PreviousHeight = window.ActualHeight;
        }

        /// <summary>
        /// Load previous <see cref="WindowState.Normal"/> state of <see cref="MdiWindow"/>
        /// </summary>
        /// <param name="window"></param>
        private static void LoadPreviousPosition(this MdiWindow window)
        {
            Canvas.SetLeft(window, window.PreviousLeft);
            Canvas.SetTop(window, window.PreviousTop);
            AnimateResize(window, window.PreviousWidth, window.PreviousHeight, false);
        }


        /// <summary>
        /// Set WindowState of <see cref="MdiWindow"/> Maximized state
        /// </summary>
        /// <param name="window"></param>
        public static void Maximize(this MdiWindow window)
        {
            _logger.Trace($"Maximize: '{window.Title} go to Maximized from {window.WindowState}");
            if (window.IsResizable)
            {
                window.SavePreviousPosition();

                Canvas.SetTop(window, 0.0);
                Canvas.SetLeft(window, 0.0);
                AnimateResize(window, window.Container.ActualWidth - 4, window.Container.ActualHeight - 4, true);
                Panel.SetZIndex(window, 10);
                
                window.PreviousWindowState = window.WindowState;
                window.WindowState = WindowState.Maximized;
            }
        }


        /// <summary>
        /// Set WindowState of <see cref="MdiWindow"/> Normal state
        /// </summary>
        /// <param name="window"></param>
        public static void Normalize(this MdiWindow window)
        {
            _logger.Trace($"Normalize: '{window.Title} go to Normalize from {window.WindowState}");
            if (window.WindowState == WindowState.Maximized) window.LoadPreviousPosition();

            Panel.SetZIndex(window, 0);
            window.PreviousWindowState = window.WindowState;
            window.WindowState = WindowState.Normal;

            
        }

        /// <summary>
        /// Set WindowState of <see cref="MdiWindow"/> Minimized state
        /// </summary>
        /// <param name="window"></param>
        public static void Minimize(this MdiWindow window)
        {
            window.ImageSource = window.CreateSnapshot();

            window.PreviousWindowState = window.WindowState;
            window.WindowState = WindowState.Minimized;
            //Panel.SetZIndex(window, 0);
        }

        internal static void AnimateResize(MdiWindow window, double newWidth, double newHeight, bool lockWindow)
        {
            window.LayoutTransform = new ScaleTransform();

            window.Height = newHeight;
            window.Width = newWidth;
            //var widthAnimation = new DoubleAnimation(window.ActualWidth, newWidth, new Duration(TimeSpan.FromMilliseconds(ANIMATED_MILLISECONDS_DURATION)));
            //var heightAnimation = new DoubleAnimation(window.ActualHeight, newHeight, new Duration(TimeSpan.FromMilliseconds(ANIMATED_MILLISECONDS_DURATION)));

            //if (lockWindow == false)
            //{
            //    widthAnimation.Completed += (s, e) => window.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(window.Width, newWidth, new Duration(TimeSpan.FromMilliseconds(ANIMATED_MILLISECONDS_DURATION))));
            //    heightAnimation.Completed += (s, e) => window.BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation(window.Height, newHeight, new Duration(TimeSpan.FromMilliseconds(ANIMATED_MILLISECONDS_DURATION))));
            //}

            //window.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation, HandoffBehavior.Compose);
            //window.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation, HandoffBehavior.Compose);
        }



        public static void ToggleMaximize(this MdiWindow window)
        {         
            if (window.WindowState == WindowState.Maximized)
            {
                window.Normalize();
            }
            else
            {
                window.Maximize();
            }
        }      

        public static void ToggleMinimize(this MdiWindow window)
        {
            if (window.WindowState != WindowState.Minimized)
            {
                window.Minimize();
            }
            else
            {            
                switch (window.PreviousWindowState)
                {
                    case WindowState.Maximized:
                        window.Maximize();
                        break;
                    case WindowState.Normal:
                        window.Normalize();
                        break;
                    default:
                        throw new NotSupportedException("Invalid WindowState");
                }
            }
        }

        public static void RemoveWindowLock(this MdiWindow window)
        {
            window.BeginAnimation(FrameworkElement.WidthProperty, null);
            window.BeginAnimation(FrameworkElement.HeightProperty, null);
        }
    }
}
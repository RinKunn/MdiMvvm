using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using MdiMvvm.Extensions;

namespace MdiMvvm.WindowControls
{
    public sealed class MoveThumb : Thumb
    {
        static MoveThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveThumb), new FrameworkPropertyMetadata(typeof(MoveThumb)));
        }

        public MoveThumb()
        {
            DragDelta += OnMoveThumbDragDelta;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var window = VisualTreeExtension.FindMdiWindow(this);         

            if (window != null)
            {
                window.DoFocus(e);         
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            var window = VisualTreeExtension.FindMdiWindow(this);         

            if (window != null && window.Container != null)
            {
                switch (window.WindowState)
                { 
                    case WindowState.Maximized:
                        window.Normalize();
                        break;
                    case WindowState.Normal:
                        window.Maximize();
                        break;
                    case WindowState.Minimized:
                        window.Normalize();
                        break;
                    default:
                    throw new InvalidOperationException("Unsupported WindowsState mode");
                }
            }

            e.Handled = true;
        }

        private void OnMoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var window = VisualTreeExtension.FindMdiWindow(this);
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    window.Normalize();
                }

                if (window.WindowState != WindowState.Minimized)
                {
                    window.PreviousLeft = Canvas.GetLeft(window);
                    window.PreviousTop = Canvas.GetTop(window);

                    var candidateLeft =  window.PreviousLeft + e.HorizontalChange;
                    var candidateTop = window.PreviousTop + e.VerticalChange;

                    double newLeft = Math.Min(Math.Max(0, candidateLeft), window.Container.ActualWidth - 25);
                    double newTop = Math.Min(Math.Max(0, candidateTop), window.Container.ActualHeight - 25);

                    Canvas.SetLeft(window, window.PreviousLeft + e.HorizontalChange);
                    Canvas.SetTop(window, window.PreviousTop + e.VerticalChange);
                }
                window.Container.InvalidateSize();
            }


        }
    }
}

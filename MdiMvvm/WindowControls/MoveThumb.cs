using System;
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
                if (window.WindowState == WindowState.Maximized) return;

                Canvas.SetLeft(window, Math.Max(0, Canvas.GetLeft(window) + e.HorizontalChange));
                Canvas.SetTop(window, Math.Max(0, Canvas.GetTop(window) + e.VerticalChange));

                window.Container.InvalidateSize();
            }
        }
    }
}

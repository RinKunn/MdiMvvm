using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MdiMvvm.Extensions
{
    internal static class VisualTreeExtension
    {
        public static TParent FindSpecificParent<TParent>(FrameworkElement sender)
         where TParent : FrameworkElement
        {
            var current = sender;
            if (current == null) return null;
            var p = VisualTreeHelper.GetParent(current) as FrameworkElement;

            if (p != null && p.GetType() != typeof(TParent))
            {
                p = FindSpecificParent<TParent>(p);
            }

            if (p == null && current.Parent is Popup)
            {
                var grandpa = ((Popup)current.Parent).Parent as FrameworkElement;
                if (grandpa != null)
                {
                    p = FindSpecificParent<TParent>(grandpa);
                }

            }

            return p as TParent;
        }

        public static T FindItemPresenterChild<T>(Visual visual) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = VisualTreeHelper.GetChild(visual, i) as Visual;
                if (child != null)
                {
                    if (child is T && VisualTreeHelper.GetParent(child) is ItemsPresenter)
                    {
                        object temp = child;
                        return (T)temp;
                    }

                    T panel = FindItemPresenterChild<T>(child);
                    if (panel != null)
                    {
                        object temp = panel;
                        return (T)temp;
                    }
                }
            }
            return null;
        }

        public static ContentControl FindContent(Visual visual, int row = 0)
        {
            ContentControl content = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = VisualTreeHelper.GetChild(visual, i) as Visual;
                if (child != null)
                {
                    if (VisualTreeHelper.GetParent(child) is ContentPresenter && child is ContentControl)
                    {
                        content = (ContentControl)child;
                        break;
                    }
                    else
                        content = FindContent(child, row + 1);
                }
            }
            return content;
        }

        public static MdiWindow FindMdiWindow(FrameworkElement sender)
        {
            return FindSpecificParent<MdiWindow>(sender);
        }
    }
}

using MdiMvvm.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Specialized;

namespace MdiMvvm.ValueObjects
{
    internal class MinimizedWindowCollection : ObservableCollection<MdiWindow>
    {
        private readonly ItemsControl _itemsControl;

        public MinimizedWindowCollection(ItemsControl bindingItemsControl) : base()
        {
            _itemsControl = bindingItemsControl;
        }


        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                Console.WriteLine($"minimized added: {e.NewItems.Count}");
                foreach(MdiWindow item in e.NewItems)
                {
                    if (item.WindowState != WindowState.Minimized) continue;
                    base.OnCollectionChanged(e);
                    ContentControl lbi = (ContentControl)_itemsControl.ItemContainerGenerator.ContainerFromItem(item);
                    lbi.MouseDoubleClick += Lbi_MouseDoubleClick;
                }
                return;
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (MdiWindow item in e.OldItems)
                {
                    ContentControl lbi = (ContentControl)_itemsControl.ItemContainerGenerator.ContainerFromItem(item);
                    lbi.MouseDoubleClick -= Lbi_MouseDoubleClick;
                }
                base.OnCollectionChanged(e);
                return;
            }
            else if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (MdiWindow item in base.Items)
                {
                    ContentControl lbi = (ContentControl)_itemsControl.ItemContainerGenerator.ContainerFromItem(item);
                    lbi.MouseDoubleClick -= Lbi_MouseDoubleClick;
                }
                base.OnCollectionChanged(e);
                return;
            }
            base.OnCollectionChanged(e);
        }
        private void Lbi_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MdiWindow window = (MdiWindow)(sender as ContentControl).DataContext;
            if (window != null)
            {
                window.Normalize();
            }
        }
    }
}

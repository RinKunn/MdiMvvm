using System;
using System.Threading.Tasks;
using System.Windows;
using MdiExample.ViewModel.Base;

namespace MdiExample.Services.WindowsServices.Store
{
    public class WindowsStoreContext : IStoreContext
    {
        public Guid Guid { get; set; }
        public Type ViewModelType { get; set; }
        public string Title { get; set; }
        public bool IsModal { get; set; }
        public bool IsSelected { get; set; }
        public double PreviousLeft { get; set; }
        public double PreviousTop { get; set; }
        public double PreviousWidth { get; set; }
        public double PreviousHeight { get; set; }
        public WindowState PreviousState { get; set; }
        public double CurrentLeft { get; set; }
        public double CurrentTop { get; set; }
        public double CurrentWidth { get; set; }
        public double CurrentHeight { get; set; }
        public WindowState WindowState { get; set; }
        public Guid ContainerGuid { get; set; }
        public ViewModelContext ViewModelContext { get; set; }

        public WindowsStoreContext()
        {
            ViewModelContext = new ViewModelContext();
        }

        internal async Task LoadContextFromEntity(IStorable<WindowsStoreContext> storable)
        {
            await storable.OnKeeping(this);
        }
    }
}

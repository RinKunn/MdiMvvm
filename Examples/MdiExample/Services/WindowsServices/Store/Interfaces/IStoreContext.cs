using System;
using MdiExample.ViewModel.Base;

namespace MdiExample.Services.WindowsServices.Store
{
    public interface IStoreContext
    {
        Guid Guid { get; set; }
        Type ViewModelType { get; set; }
        ViewModelContext ViewModelContext { get; set; }
    }
}

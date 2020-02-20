using System;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public interface IStoreContext
    {
        Guid Guid { get; set; }
        Type ViewModelType { get; set; }
        ViewModelContext ViewModelContext { get; set; }
    }
}

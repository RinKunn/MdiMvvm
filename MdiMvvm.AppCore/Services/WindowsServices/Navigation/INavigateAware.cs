using System;
using System.Threading.Tasks;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Services.WindowsServices.Navigation
{
    public interface INavigateAware
    {
        void NavigatedTo(ViewModelContext context);

        void RaiseCallBack(NavigationResult result);

        Action<NavigationResult> CallBackAction { get; set; }
    }
}

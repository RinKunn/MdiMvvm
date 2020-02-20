using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Services.WindowsServices.Navigation
{
    public interface INavigateAware
    {
        void NavigatedTo(ViewModelContext context);
    }
}

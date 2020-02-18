using MdiExample.ViewModel.Base;

namespace MdiExample.Services.WindowsServices.Navigation
{
    public interface INavigateAware
    {
        void NavigatedTo(ViewModelContext context);
    }
}

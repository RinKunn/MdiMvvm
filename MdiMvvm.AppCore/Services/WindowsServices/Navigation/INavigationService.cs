using MdiMvvm.Interfaces;

namespace MdiMvvm.AppCore.Services.WindowsServices.Navigation
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigate to window.
        /// Create or navigate to exists window <see cref="INavigateAware"/>
        /// </summary>
        /// <typeparam name="TViewModel">Must implement <see cref="IMdiWindowViewModel"/> and <see cref="INavigateAware"/> </typeparam>
        /// <param name="navigateParameters">Navigate parameters with context</param>
        void NavigateTo<TViewModel>(NavigateParameters navigateParameters)
            where TViewModel : class, IMdiWindowViewModel, INavigateAware;
    }
}

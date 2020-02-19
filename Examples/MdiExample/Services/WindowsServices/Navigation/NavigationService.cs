using System;
using MdiExample.Services.WindowsServices.Factory;
using MdiExample.Services.WindowsServices.WindowsManager;
using MdiMvvm.Interfaces;

namespace MdiExample.Services.WindowsServices.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IWindowsManagerService _windowManager;
        private readonly IWindowsFactory _windowsFactory;

        public NavigationService(IWindowsManagerService windowsManager, IWindowsFactory windowsFactory)
        {
            _windowManager = windowsManager ?? throw new ArgumentNullException(nameof(windowsManager));
            _windowsFactory = windowsFactory ?? throw new ArgumentNullException(nameof(windowsFactory));
        }

        public void NavigateTo<TViewModel>(NavigateParameters navigateParameters)
            where TViewModel : class, IMdiWindowViewModel, INavigateAware
        {
            TViewModel viewModel;
            if (navigateParameters.GuidWindows != Guid.Empty)
            {
                viewModel = _windowManager.FindWindow<TViewModel>(navigateParameters.GuidWindows);
                if (viewModel == null)
                    throw new ArgumentNullException(nameof(viewModel));
            }
            else
            {
                viewModel = _windowsFactory.CreateWindow<TViewModel>();
                viewModel = navigateParameters.GuidContainer == Guid.Empty
                    ? _windowManager.AppendWindow(viewModel)
                    : _windowManager.AppendWindowToContainer(viewModel, navigateParameters.GuidContainer);
            }
            _windowManager.ActivateWindow(viewModel);
            viewModel.NavigatedTo(navigateParameters.Context);
        }
    }
}

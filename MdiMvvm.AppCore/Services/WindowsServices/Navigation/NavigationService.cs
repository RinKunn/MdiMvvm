using System;
using MdiMvvm.AppCore.Services.WindowsServices.Factory;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;
using MdiMvvm.AppCore.ViewModelsBase;
using MdiMvvm.Interfaces;

namespace MdiMvvm.AppCore.Services.WindowsServices.Navigation
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


        public void NavigateTo<TViewModel>(string key, object obj)
            where TViewModel : class, IMdiWindowViewModel, INavigateAware
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue(key, obj);
            NavigateParameters navigateParameters = new NavigateParameters(context);

            this.NavigateTo<TViewModel>(navigateParameters);
        }
    }
}

using System;
using System.Threading.Tasks;
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

        private TViewModel FindOrCreateViewModel<TViewModel>(NavigateParameters navigateParameters)
            where TViewModel : IMdiWindowViewModel, INavigateAware
        {
            TViewModel viewModel;
            if (navigateParameters != null && navigateParameters.GuidWindows != Guid.Empty)
            {
                viewModel = _windowManager.FindWindow<TViewModel>(navigateParameters.GuidWindows);
                if (viewModel == null)
                    throw new ArgumentNullException(nameof(viewModel));
            }
            else
            {
                viewModel = _windowsFactory.CreateWindow<TViewModel>();
                Guid containerGuid = navigateParameters == null ? Guid.Empty : navigateParameters.GuidContainer;
                viewModel = _windowManager.AppendWindowWithoutInit(viewModel, containerGuid, false);
            }
            return viewModel;
        }

        public async Task NavigateToAsync<TViewModel>(NavigateParameters navigateParameters)
            where TViewModel : IMdiWindowViewModel, INavigateAware
        {
            var viewModel = FindOrCreateViewModel<TViewModel>(navigateParameters);
            _windowManager.ActivateWindow(viewModel);
            viewModel.NavigatedTo(navigateParameters?.Context);
            await viewModel.InitAsync();
        }

        public async Task NavigateToAsync<TViewModel>(string key, object obj)
            where TViewModel : IMdiWindowViewModel, INavigateAware
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue(key, obj);
            NavigateParameters navigateParameters = new NavigateParameters(context);

            await NavigateToAsync<TViewModel>(navigateParameters);
        }

        public async Task NavigateToAsync<TViewModel>(NavigateParameters navigateParameters, Action<NavigationResult> navigationCallback)
            where TViewModel : IMdiWindowViewModel, INavigateAware
        {
            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            var viewModel = FindOrCreateViewModel<TViewModel>(navigateParameters);
            viewModel.CallBackAction = navigationCallback;
            _windowManager.ActivateWindow(viewModel);
            viewModel.NavigatedTo(navigateParameters?.Context);
            await viewModel.InitAsync();
        }
    }
}

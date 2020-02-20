using System;
using MdiMvvm.Interfaces;
using Unity;

namespace MdiMvvm.AppCore.Services.WindowsServices.Factory
{
    public class WindowsFactory : IWindowsFactory
    {
        private readonly IUnityContainer _container;
        public WindowsFactory(IUnityContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public TContainerViewModel CreateContainer<TContainerViewModel>() where TContainerViewModel : IMdiContainerViewModel
        {
            return (TContainerViewModel)_container.Resolve<IMdiContainerViewModel>();
        }

        public object CreateContainer(Type containerType)
        {
            return _container.Resolve<IMdiContainerViewModel>();
        }

        public TWindowViewModel CreateWindow<TWindowViewModel>() where TWindowViewModel : IMdiWindowViewModel
        {
            return (TWindowViewModel)_container.Resolve<IMdiWindowViewModel>(typeof(TWindowViewModel).Name);
        }

        public object CreateWindow(Type windowType)
        {
            return _container.Resolve<IMdiWindowViewModel>(windowType.Name);
        }
    }
}

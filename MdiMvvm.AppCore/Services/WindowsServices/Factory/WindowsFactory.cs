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
            return _container.Resolve<TContainerViewModel>();
        }

        public object CreateContainer(Type containerType)
        {
            return _container.Resolve(containerType);
        }

        public TWindowViewModel CreateWindow<TWindowViewModel>() where TWindowViewModel : IMdiWindowViewModel
        {
            return _container.Resolve<TWindowViewModel>();
        }

        public object CreateWindow(Type windowType)
        {
            return _container.Resolve(windowType);
        }
    }
}

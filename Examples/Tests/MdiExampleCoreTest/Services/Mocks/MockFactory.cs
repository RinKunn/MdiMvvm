using System;
using MdiMvvm.AppCore.Services.WindowsServices.Factory;
using MdiMvvm.Interfaces;

namespace MdiMvvm.AppCore.Tests.Services.Mocks
{
    public class WindowsFactoryMock : IWindowsFactory
    {
        public object CreateContainer(Type containerType)
        {
            return new MdiContainerMock();
        }

        public TContainerViewModel CreateContainer<TContainerViewModel>() where TContainerViewModel : IMdiContainerViewModel
        {
            var res = new MdiContainerMock();
            if (res is TContainerViewModel cont)
                return cont;
            return default;
        }

        public object CreateWindow(Type windowType)
        {
            return new MdiWindowMock();
        }

        public TWindowViewModel CreateWindow<TWindowViewModel>() where TWindowViewModel : IMdiWindowViewModel
        {
            var res = new MdiWindowMock();
            if (res is TWindowViewModel wind)
                return wind;
            return default;
        }
    }
}

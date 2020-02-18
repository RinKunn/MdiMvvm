using System;
using MdiMvvm.Interfaces;

namespace MdiExample.Services.WindowsServices.Factory
{
    public interface IWindowsFactory
    {
        object CreateWindow(Type windowType);
        TWindowViewModel CreateWindow<TWindowViewModel>() where TWindowViewModel : IMdiWindowViewModel;

        object CreateContainer(Type containerType);
        TContainerViewModel CreateContainer<TContainerViewModel>() where TContainerViewModel : IMdiContainerViewModel;
    }
}

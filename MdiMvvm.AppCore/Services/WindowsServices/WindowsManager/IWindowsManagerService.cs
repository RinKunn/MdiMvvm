using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MdiMvvm.Interfaces;

namespace MdiMvvm.AppCore.Services.WindowsServices.WindowsManager
{
    public interface IWindowsManagerService
    {
        /// <summary>
        /// On Mdi-Container collection changed
        /// </summary>
        event ContainersCollectionChangedHandler ContainerCollectionChanged;

        /// <summary>
        /// On active Mdi-Container changed
        /// </summary>
        event ActiveContainerChangedHandler ActiveContainerChanged;

        /// <summary>
        /// Active Mdi-Container
        /// </summary>
        IMdiContainerViewModel ActiveContainer { get; }

        /// <summary>
        /// Mdi-Containers
        /// </summary>
        ReadOnlyObservableCollection<IMdiContainerViewModel> Containers { get; }

        /// <summary>
        /// Load new Mdi-Container's collection
        /// </summary>
        /// <param name="collection">New Mdi-Container's collection</param>
        void LoadContainers(IEnumerable<IMdiContainerViewModel> collection);


        /// <summary>
        /// Append ViewModel of new Mdi-Window to Mdi-Container by <see cref="Guid"/>
        /// </summary>
        /// <typeparam name="TViewModel">Must implement <see cref="IMdiWindowViewModel"/> </typeparam>
        /// <param name="viewModel">ViewModel of adding Mdi-Window</param>
        /// <param name="containerGuid">Mdi-Container's <see cref="Guid"/> </param>
        /// <returns></returns>
        TViewModel AppendWindowToContainer<TViewModel>(TViewModel viewModel, Guid containerGuid) where TViewModel : IMdiWindowViewModel;

        /// <summary>
        /// Append ViewModel of new Mdi-Window to Active Mdi-Container
        /// </summary>
        /// <typeparam name="TViewModel">Must implement <see cref="IMdiWindowViewModel"/> </typeparam>
        /// <param name="viewModel">ViewModel of adding Mdi-Window</param>
        /// <returns></returns>
        TViewModel AppendWindow<TViewModel>(TViewModel viewModel) where TViewModel : IMdiWindowViewModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        object AppendWindowToContainer(object viewModel, Guid containerGuid);


        /// <summary>
        /// Append ViewModel of new Mdi-Container to collection
        /// </summary>
        /// <typeparam name="TContainerViewModel">Must implement <see cref="IMdiContainerViewModel"/> </typeparam>
        /// <param name="viewModel">ViewModel of adding Mdi-Container</param>
        /// <returns></returns>
        TContainerViewModel AppendContainer<TContainerViewModel>(TContainerViewModel viewModel) where TContainerViewModel : IMdiContainerViewModel;

        /// <summary>
        /// Append ViewModel of new Mdi-Container to collection
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        object AppendContainer(object viewModel);

        /// <summary>
        /// Find Mdi-Window by <see cref=Guid"/>
        /// </summary>
        /// <typeparam name="TViewModel">Must implement <see cref="IMdiWindowViewModel"/> </typeparam>
        /// <param name="windowGuid"><see cref="Guid"/> of desired Mdi-Window</param>
        /// <returns>Desired Mdi-Window if exists of <see cref="null"/> </returns>
        TViewModel FindWindow<TViewModel>(Guid windowGuid) where TViewModel : class, IMdiWindowViewModel;

        /// <summary>
        /// Activate Mdi-Container if it's not activated by <see cref="Guid"/>
        /// </summary>
        /// <param name="guid"><see cref="Guid"/> of desired Mdi_Container</param>
        void ActivateContainer(Guid guid);

        /// <summary>
        /// Activate Mdi-Container
        /// </summary>
        /// <typeparam name="TContainerViewModel">Must implement <see cref="IMdiContainerViewModel"/></typeparam>
        /// <param name="containerViewModel">Activiting Mdi-Container</param>
        void ActivateContainer<TContainerViewModel>(TContainerViewModel containerViewModel) where TContainerViewModel : IMdiContainerViewModel;

        /// <summary>
        /// Activate Mdi-Window
        /// </summary>
        /// <typeparam name="TViewModel">Must implement <see cref="IMdiWindowViewModel"/> </typeparam>
        /// <param name="windowGuid">Activiting Mdi-Window</param>
        void ActivateWindow<TViewModel>(TViewModel window) where TViewModel : class, IMdiWindowViewModel;

    }
}

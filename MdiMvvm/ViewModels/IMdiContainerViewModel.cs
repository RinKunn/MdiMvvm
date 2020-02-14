using System;
using System.Collections.ObjectModel;

namespace MdiMvvm.ViewModels
{
    public interface IMdiContainerViewModel
    {
        /// <summary>
        /// GUID of Container
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Title of container
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Is Container selected
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Is container busy doing some process
        /// </summary>
        bool IsBusy { get; set; }

        /// <summary>
        /// <see cref="IMdiWindowViewModel" />'s collection
        /// </summary>
        ObservableCollection<IMdiWindowViewModel> WindowsCollection { get; set; }


        /// <summary>
        /// Add mdi window to container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="IMdiWindowViewModel"/></typeparam>
        /// <param name="window">Adding window</param>
        void AddMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel;


        /// <summary>
        /// Remove mdi window from container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="MdiWindowViewModelBase"/></typeparam>
        /// <param name="window">Removing window</param>
        void RemoveMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel;


        /// <summary>
        /// Remove mdi window from container
        /// </summary>
        /// <param name="guid"></param>
        void RemoveMdiWindowByGuid(Guid guid);
        
    }
}

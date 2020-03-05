using System;
using System.Collections.ObjectModel;

namespace MdiMvvm.Interfaces
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

        bool IsInited { get; set; }

        bool IsScrollBarVisible { get; set; }

        /// <summary>
        /// <see cref="IMdiWindowViewModel" />'s collection
        /// </summary>
        ObservableCollection<IMdiWindowViewModel> WindowsCollection { get; set; }

        void AddMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel;
        void RemoveMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel;

        void Init();
    }
}

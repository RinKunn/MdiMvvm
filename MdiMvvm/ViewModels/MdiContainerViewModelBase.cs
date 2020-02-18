﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using MdiMvvm.Exceptions;
using MdiMvvm.Interfaces;

namespace MdiMvvm.ViewModels
{
    [Obsolete("Create custom ViewModelBase, that implement IMdiContainerViewModel", true)]
    public abstract class MdiContainerViewModelBase : ObservableObject, IMdiContainerViewModel
    {
        #region Members
        private Guid _guid;
        private string _title;
        private bool _isSelected;
        private bool _isBusy;
        private ObservableCollection<IMdiWindowViewModel> _windowsCollection;
        #endregion

        #region Properties
        /// <summary>
        /// GUID of Container
        /// </summary>
        public Guid Guid => _guid;

        /// <summary>
        /// Title of container
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        /// <summary>
        /// Is Container selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        /// <summary>
        /// Is container busy doing some process
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        /// <summary>
        /// <see cref="MdiWindowViewModelBase" />'s collection
        /// </summary>
        public ObservableCollection<IMdiWindowViewModel> WindowsCollection
        {
            get => _windowsCollection;
            set
            {
                if (_windowsCollection != null)
                {
                    _windowsCollection.CollectionChanged -= WindowsCollection_CollectionChanged;
                }
                Set(ref _windowsCollection, value);
                if (value != null)
                {
                    _windowsCollection.CollectionChanged += WindowsCollection_CollectionChanged;
                    foreach (IMdiWindowViewModel win in value)
                        win.Container = this;
                }

            }
        }

        protected virtual void WindowsCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (MdiWindowViewModelBase win in e.NewItems)
                    win.Container = this;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (MdiWindowViewModelBase win in e.OldItems)
                    win.Container = null;
            }
        }
        #endregion

        public MdiContainerViewModelBase()
        {
            _guid = Guid.NewGuid();
            WindowsCollection = new ObservableCollection<IMdiWindowViewModel>();
        }

        /// <summary>
        /// Add mdi window to container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="IMdiWindowViewModel"/></typeparam>
        /// <param name="window">Adding window</param>
        public void AddMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel
        {
            if (window == null) return;
            if (_windowsCollection.FirstOrDefault(w => w.Guid == window.Guid) != null)
                throw new MdiWindowAlreadyExistsException(window.Guid, this.Title);

            // TODO: validate Title for equal windows title

            _windowsCollection.Add(window);
        }

        /// <summary>
        /// Remove mdi window from container
        /// </summary>
        /// <typeparam name="TWindow">Type that derived from <see cref="IMdiWindowViewModel"/></typeparam>
        /// <param name="window">Removing window</param>
        public void RemoveMdiWindow<TWindow>(TWindow window) where TWindow : IMdiWindowViewModel
        {
            if (window == null) return;
            if (_windowsCollection.FirstOrDefault(w => w.Guid == window.Guid) == null)
                throw new MdiWindowNotFoundedException(window.Guid, this.Title);
            _windowsCollection.Remove(window);
        }

        public IMdiWindowViewModel GetWindow(Guid guid)
        {
            return _windowsCollection.FirstOrDefault(w => w.Guid == guid);
        }
    }
}

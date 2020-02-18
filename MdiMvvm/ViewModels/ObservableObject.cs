using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MdiMvvm.ViewModels
{
    [Obsolete("Only for internal usage. Now is obsolete", true)]
    public abstract class ViewModelBase : ObservableObject
    {

        public ViewModelBase()
        {

        }
    }

    [Obsolete("Only for internal usage. Now is obsolete", true)]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T field, T newValue = default, [CallerMemberName] string propertyName = null)
        {
            if (object.ReferenceEquals(field, newValue)) return false;
            if (object.Equals(field, newValue)) return false;

            field = newValue;

            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}

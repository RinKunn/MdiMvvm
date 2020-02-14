using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MdiMvvm.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
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

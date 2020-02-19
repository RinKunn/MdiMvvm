using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdiExample.Services.WindowsServices.WindowsManager
{
    public static class IEnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            ObservableCollection<T> observ = new ObservableCollection<T>();
            foreach (var item in collection)
                observ.Add(item);
            return observ;
        }
    }
}

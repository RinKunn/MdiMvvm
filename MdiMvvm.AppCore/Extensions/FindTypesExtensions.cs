using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MdiMvvm.AppCore.Extensions
{
    public static class FindTypesExtensions
    {
        public static IEnumerable<Type> FindDerivedTypes<TBaseType>(this Assembly assembly)
        {
            return assembly.GetTypes().Where(t => typeof(TBaseType).IsAssignableFrom(t));
        }
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body)
        {
            List<Exception> exceptions = null;
            foreach (var item in source)
            {
                try 
                {
                    await body(item);
                }
                catch (Exception exc)
                {
                    if (exceptions == null) 
                        exceptions = new List<Exception>();
                    exceptions.Add(exc);
                }
            }
            if (exceptions != null)
                throw new AggregateException(exceptions);
        }

    }
}

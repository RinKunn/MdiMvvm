using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MdiMvvm.AppCore.Extensions
{
    public static class FindTypesExtensions
    {
        public static IEnumerable<Type> FindDerivedTypes<TBaseType>(this Assembly assembly)
        {
            return assembly.GetTypes().Where(t => typeof(TBaseType).IsAssignableFrom(t));
        }
    }
}

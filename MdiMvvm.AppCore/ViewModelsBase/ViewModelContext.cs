using System;
using System.Collections.Generic;

namespace MdiMvvm.AppCore.ViewModelsBase
{
    public class ViewModelContext
    {
        public Dictionary<string, object> InternalParameter { get; set; }

        public ViewModelContext()
        {
            InternalParameter = new Dictionary<string, object>();
        }

        public void AddValue<T>(string key, T obj)
        {
            if (string.IsNullOrEmpty(key) || obj == null) return;
            InternalParameter.Add(key, obj);
        }

        public bool TryGetValue<T>(string key, out T obj)
        {
            obj = default;
            if (string.IsNullOrEmpty(key)) return false;
            if (!InternalParameter.ContainsKey(key)) return false;
            var value = InternalParameter[key];
            if (value is T result)
                obj = result;
            else
            {
                try
                {
                    obj = (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public T GetValue<T>(string key)
        {
            Console.WriteLine($"key = {InternalParameter.Count}");
            if (!TryGetValue(key, out T result)) return default;
            return result;
        }

        public bool ContainsKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            return InternalParameter.ContainsKey(key);
        }

        public void ClearValues()
        {
            InternalParameter?.Clear();
        }
    }
}

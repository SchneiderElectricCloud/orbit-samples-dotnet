using System;
using System.Collections.Generic;
using SampleOrbitEventListenerService.Converters;

namespace SampleOrbitEventListenerService.Extensions
{
    static class DictionaryExtensions
    {
        public static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            T result = default(T);
            object value;
            if (dictionary != null && dictionary.TryGetValue(key, out value))
            {
                result = (T)SmartTypeConverter.ConvertTo(typeof(T), value);
            }
            return result;
        }
    }
}
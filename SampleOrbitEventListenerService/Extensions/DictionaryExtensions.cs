using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using NLog;

namespace SampleOrbitEventListenerService.Extensions
{
    static class DictionaryExtensions
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            T result = default(T);
            object value;
            if (dictionary != null && dictionary.TryGetValue(key, out value))
            {
                try
                {
                    if (IsNotNull(value))
                    {
                        if (value.GetType() == typeof(T))
                        {
                            // If the type of 'value' is already type T, then there is
                            // no need for type coersion, just set value as the result
                            result = (T)value;
                        }
                        else
                        {
                            if (typeof(T) == typeof(String) || !IsNullOrWhitespace(value))
                            {
                                result = (T)Conversion.CTypeDynamic(value, typeof(T));
                            }
                        }
                    }
                }
                catch (InvalidCastException e)
                {
                    string errorMessage = string.Format(
                        "Cannot convert key '{0}' value '{1}' from {2} to {3}.",
                        key,
                        value,
                        (value == null) ? "null" : value.GetType().Name,
                        typeof(T).Name);
                    Log.Warn(errorMessage, e);
                }
            }
            return result;
        }

        static bool IsNullOrWhitespace(object value)
        {
            return value is string && string.IsNullOrWhiteSpace((string)value);
        }

        static bool IsNotNull(object value)
        {
            return value != null && value != DBNull.Value;
        }
    }
}
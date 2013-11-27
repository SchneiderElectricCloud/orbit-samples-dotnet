using System;

namespace SampleOrbitEventListenerService.Extensions
{
    static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static object GetDefaultValue(this Type type)
        {
            object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
            return defaultValue;
        }
    }
}
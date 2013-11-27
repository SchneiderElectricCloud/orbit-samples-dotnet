using System;
using System.ComponentModel;
using SampleOrbitEventListenerService.Extensions;

namespace SampleOrbitEventListenerService.Converters
{
    static class SmartTypeConverter
    {
        public static object ConvertTo(Type targetType, object value)
        {
            object coercedValue;
            if (value == null || value == DBNull.Value)
            {
                coercedValue = targetType.IsValueType 
                    ? targetType.GetDefaultValue() 
                    : null;
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(value.GetType()))
                {
                    try
                    {
                        coercedValue = converter.ConvertFrom(value);
                    }
                    catch (Exception e)
                    {
                        // We check CanConvertFrom, but this not a very thorough check
                        // and the ConvertFrom may throw an exception anyway!
                        coercedValue = targetType.GetDefaultValue();
                    }
                }
                else
                {
                    if (targetType.IsNullable())
                    {
                        // Convert.ChangeType doesn't handle nullable types correctly
                        Type underlyingType = Nullable.GetUnderlyingType(targetType);
                        coercedValue = Convert.ChangeType(value, underlyingType);
                    }
                    else
                    {
                        coercedValue = Convert.ChangeType(value, targetType);
                    }
                }
            }

            return coercedValue;
        }
    }
}
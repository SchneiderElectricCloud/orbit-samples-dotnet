using System;
using System.ComponentModel;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SampleOrbitEventListenerService.Extensions;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    [DebuggerDisplay("Name = {Name}")]
    public class Field
    {
        [JsonProperty("name", Order = -99)]
        public string Name { get; set; }

        [JsonProperty("type", Order = -98)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType Type { get; set; }

        [JsonProperty("alias", Order = -97)]
        public string Alias { get; set; }
       
        /// <summary>
        /// From 10.0 fields of type (String, Date, GlobalID, GUID and XML) have an additional length property
        /// </summary>
        [DefaultValue(0)]
        [JsonProperty("length", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Length { get; set; }

        [DefaultValue(false)]
        [JsonProperty("editable", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Editable { get; set; }

        [JsonProperty("nullable")]
        public bool Nullable { get; set; }

        public object ToCompatibleType(object value)
        {
            object convertedValue = value;
            if (value != null)
            {
                switch (Type)
                {
                    case FieldType.ObjectId:
                    case FieldType.Integer:
                        convertedValue = Convert.ToInt32(value);
                        break;
                    case FieldType.Single:
                        convertedValue = Convert.ToSingle(value);
                        break;
                    case FieldType.Double:
                        convertedValue = Convert.ToDouble(value);
                        break;
                    case FieldType.SmallInteger:
                        convertedValue = Convert.ToInt16(value);
                        break;
                    case FieldType.Date:
                        convertedValue = Convert.ToDateTime(value)
                            .ToLocalTime()
                            .AsUnixTimestampMillis();
                        break;
                    case FieldType.String:
                        convertedValue = Convert.ToString(value);
                        break;
                    case FieldType.GlobalId:
                    case FieldType.Guid:
                        convertedValue = new Guid(value.ToString()).ToString("B");
                        break;
                }
            }
            else if (!Nullable)
            {
                convertedValue = GetDefaultValue();
            }

            return convertedValue;
        }

        public object GetDefaultValue()
        {
            object defaultValue = null;
            if (!Nullable)
            {
                if (!Nullable)
                {
                    switch (Type)
                    {
                        case FieldType.SmallInteger:
                            defaultValue = default(short);
                            break;
                        case FieldType.ObjectId:
                        case FieldType.Integer:
                            defaultValue = default(int);
                            break;
                        case FieldType.Single:
                            defaultValue = default(float);
                            break;
                        case FieldType.Double:
                            defaultValue = default(double);
                            break;
                        case FieldType.String:
                            defaultValue = string.Empty;
                            break;
                        case FieldType.Date:
                            defaultValue = 0;
                            break;
                        case FieldType.GlobalId:
                        case FieldType.Guid:
                            defaultValue = Guid.Empty;
                            break;
                        default:
                            throw new Exception(
                                string.Format("Need to provide appropriate default value for field type {0}", Type));
                    }
                }
            }

            return defaultValue;
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    public enum FieldType
    {
        [EnumMember(Value = "esriFieldTypeSmallInteger")]
        SmallInteger = 0,

        [EnumMember(Value = "esriFieldTypeInteger")]
        Integer = 1,

        [EnumMember(Value = "esriFieldTypeSingle")]
        Single = 2,

        [EnumMember(Value = "esriFieldTypeDouble")]
        Double = 3,

        [EnumMember(Value = "esriFieldTypeString")]
        String = 4,

        [EnumMember(Value = "esriFieldTypeDate")]
        Date = 5,

        [EnumMember(Value = "esriFieldTypeOID")]
        ObjectId = 6,

        [EnumMember(Value = "esriFieldTypeGeometry")]
        Geometry = 7,

        [EnumMember(Value = "esriFieldTypeBlob")]
        Blob = 8,

        [EnumMember(Value = "esriFieldTypeRaster")]
        Raster = 9,

        [EnumMember(Value = "esriFieldTypeGUID")]
        Guid = 10,

        [EnumMember(Value = "esriFieldTypeGlobalID")]
        GlobalId = 11,

        [EnumMember(Value = "esriFieldTypeXML")]
        Xml = 12
    }
}
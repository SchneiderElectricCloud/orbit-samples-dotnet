using System;
using System.Runtime.Serialization;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    public enum LayerType
    {
        [EnumMember(Value = "Feature Layer")]
        FeatureLayer,

        [EnumMember(Value = "Table")]
        Table
    }
}
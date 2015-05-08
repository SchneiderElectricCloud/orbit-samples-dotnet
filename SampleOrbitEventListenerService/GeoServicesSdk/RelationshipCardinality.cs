using System;
using System.Runtime.Serialization;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    public enum RelationshipCardinality
    {
        [EnumMember(Value = "esriRelCardinalityOneToOne")]
        OneToOne,

        [EnumMember(Value = "esriRelCardinalityOneToMany")]
        OneToMany,

        [EnumMember(Value = "esriRelCardinalityManyToMany")]
        ManyToMany
    }
}
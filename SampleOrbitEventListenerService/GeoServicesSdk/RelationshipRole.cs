using System;
using System.Runtime.Serialization;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    public enum RelationshipRole
    {
        [EnumMember(Value = "esriRelRoleOrigin")]
        Origin,

        [EnumMember(Value = "esriRelRoleDestination")]
        Destination
    }
}
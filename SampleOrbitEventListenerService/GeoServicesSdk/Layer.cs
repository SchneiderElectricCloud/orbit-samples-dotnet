using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    [DebuggerDisplay("ID = {Id}, Name = {Name}")]
    public class Layer
    {
        public Layer()
        {
            Relationships = new Relationship[0];
        }

        [JsonProperty("currentVersion", Order = -99)]
        public double CurrentVersion { get; set; }

        [JsonProperty("id", Order = -98)]
        public int Id { get; set; }

        [JsonProperty("name", Order = -97)]
        public string Name { get; set; }

        [JsonProperty("type", Order = -96)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LayerType Type { get; protected set; }

        [JsonProperty("relationships")]
        public Relationship[] Relationships { get; set; }

        [JsonProperty("objectIdField")]
        public string ObjectIdField { get; set; }

        [JsonProperty("globalIdField")]
        public string GlobalIdField { get; set; }

        [JsonProperty("displayField")]
        public string DisplayField { get; set; }

        [JsonProperty("typeIdField")]
        public string TypeIdField { get; set; }

        [JsonProperty("fields")]
        public Field[] Fields { get; set; } 

        [JsonProperty("capabilities")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LayerCapabilities Capabilities { get; set; }
    }
}
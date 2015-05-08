using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    public class Relationship
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("relatedTableId")]
        public int RelatedTableId { get; set; }

        [JsonProperty("cardinality")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RelationshipCardinality Cardinality { get; set; }

        [JsonProperty("role")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RelationshipRole Role { get; set; }

        [JsonProperty("keyField")]
        public string KeyField { get; set; }

        [JsonProperty("composite")]
        public bool Composite { get; set; }

        [JsonProperty("relationshipTableId")]
        public int RelationshipTableId { get; set; }

        [JsonProperty("keyFieldInRelationshipTable")]
        public string KeyFieldInRelationshipTable { get; set; }
    }
}
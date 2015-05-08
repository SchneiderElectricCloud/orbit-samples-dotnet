using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    public class Feature
    {
        public Feature()
        {
            Attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        [JsonProperty("attributes")]
        public Dictionary<string, object> Attributes { get; set; }
    }
}
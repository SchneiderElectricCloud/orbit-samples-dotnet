using System;
using Newtonsoft.Json;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    public class Error
    {
        public Error()
        {
            Code = 0;
            Message = string.Empty;
            Details = new string[0];
        }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("details")]
        public string[] Details { get; set; }
    }
}
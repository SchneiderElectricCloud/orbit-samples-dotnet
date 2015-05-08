using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    class FeatureServer
    {
        readonly HttpClient _client;
        readonly Uri _featureServerUrl;

        /// <summary>
        /// Expecting Url formatted as http://[host]:[port]/arcgis/rest/services/[serviceName]/MapServer
        /// </summary>
        /// <param name="featureServerUrl"></param>
        public FeatureServer(Uri featureServerUrl)
        {
            if (featureServerUrl == null) throw new ArgumentNullException("featureServerUrl");

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _featureServerUrl = NormalizeUri(featureServerUrl);
        }

        public async Task<Layer> GetLayerAsync(int layerId)
        {
            Uri featureLayerUrl = new Uri(_featureServerUrl + "/" + layerId);

            string featureLayerContent = await _client.GetStringAsync(featureLayerUrl + "?f=json");
            JObject featureLayerJson = JObject.Parse(featureLayerContent);
            ThrowIfAgsErrorResponse(featureLayerJson, featureLayerUrl);

            Layer featureLayer = featureLayerJson.ToObject<Layer>();
            return featureLayer;
        }

        public async Task<JObject> AddFeature(int layerId, Dictionary<string, string> parameters)
        {
            parameters["f"] = "json";

            Uri addFeatureUrl = new Uri(_featureServerUrl + "/" + layerId + "/addFeatures");
            HttpContent formUrlContent = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await _client.PostAsync(addFeatureUrl, formUrlContent);

            string responseContent = await response.Content.ReadAsStringAsync();
            JObject responseContentJson = JObject.Parse(responseContent);
            ThrowIfAgsErrorResponse(responseContentJson, addFeatureUrl);

            // The response format of the addFeatures operation are documented here:
            // http://resources.arcgis.com/en/help/arcgis-rest-api/02r3/02r3000001np000000.htm
            return responseContentJson;
        }

        static Uri NormalizeUri(Uri uri)
        {
            string normalizedUri = (uri != null) ? uri.OriginalString : string.Empty;

            // Trim the ending forward-slash, if there is one
            if (!string.IsNullOrEmpty(normalizedUri) && normalizedUri.EndsWith("/"))
            {
                normalizedUri = normalizedUri.Remove(normalizedUri.Length - 1, 1);
            }

            return new Uri(normalizedUri);
        }

        static void ThrowIfAgsErrorResponse(JObject jo, Uri url)
        {
            // Here's an example from ArcGIS Server
            // {"error":{"code":400,"message":"Invalid URL","details":[]}}
            if (jo != null)
            {
                JToken error = jo["error"];
                if (error != null)
                {
                    int code = error["code"].Value<int>();
                    string message = string.Format("{0}\nURL={1}", error, url);

                    throw new HttpException(code, message);
                }
            }
        }
    }

}
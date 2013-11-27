using System;
using System.Net.Http;

namespace SampleOrbitEventListenerService.Extensions
{
    static class HttpClientExtensions
    {
        public static HttpClient WithOrbitAcceptHeaders(this HttpClient client)
        {
            if (client == null) throw new ArgumentNullException("client");

            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.schneider-electric.orbit+json; version=1");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        public static HttpResponseMessage ThrowOnError(this HttpResponseMessage response,
            string format, params object[] args)
        {
            if (response == null) throw new ArgumentNullException("response");

            return response.EnsureSuccessStatusCode();
            //if (!response.IsSuccessStatusCode)
            //{
            //    string message = string.Format("[{0}] ", response.StatusCode)
            //        + string.Format(format, args);
            //    throw new Exception(message);
            //}
            //return response;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SampleOrbitEventListenerService.Configuration;
using SampleOrbitEventListenerService.Extensions;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService
{
    /// <summary>
    /// Helper for calling into Orbit Task Services
    /// </summary>
    /// <remarks>
    /// These functions should/will be included in the Orbit SDK, but they
    /// haven't been yet...
    /// </remarks>
    public class TaskServicesApi
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        const string JsonMediaType = "application/json";

        void AddCorrelationIdHeader(HttpContent content)
        {
            Guid correlationId = Guid.NewGuid();
            content.Headers.Add("X-CorrelationId", correlationId.ToString("D"));
        }

        public async Task<TaskResource> CreateTask(TaskResource task)
        {
            HttpResponseMessage response;
            using (var client = CreateHttpClient().WithOrbitAcceptHeaders())
            {
                string json = JsonConvert.SerializeObject(task);
                HttpContent requestContent = new StringContent(json, Encoding.UTF8, JsonMediaType);
                //AddCorrelationIdHeader(requestContent);
                response = await client.PostAsync("api/Tasks", requestContent);
            }
            response.ThrowOnError("Error creating task");

            string responseContent = await response.Content.ReadAsStringAsync();
            var createdTask = DeserializeJson<TaskResource>(responseContent);

            Debug.Assert(createdTask != null, "Expecting valid task reference");

            // NOTE: tasks now contain a display name (as configured), but this isn't in production yet...
            //string displayName = createdTask.DisplayName ?? string.Empty;
            //Log.Info("Created task {0}: {1}", createdTask.ID, displayName);

            return createdTask;
        }

        public async Task<TaskResource> UpdateTask(TaskResource task)
        {
            HttpResponseMessage response;
            using (var client = CreateHttpClient().WithOrbitAcceptHeaders())
            {
                string json = JsonConvert.SerializeObject(task);
                HttpContent requestContent = new StringContent(json, Encoding.UTF8, JsonMediaType);
                //AddCorrelationIdHeader(requestContent);
                string uri = string.Format("api/Tasks/{0}", task.ID);
                response = await client.PutAsync(uri, requestContent);
            }
            response.ThrowOnError("Error updating task");

            string responseContent = await response.Content.ReadAsStringAsync();
            var updatedTask = DeserializeJson<TaskResource>(responseContent);

            Debug.Assert(updatedTask != null, "Expecting valid task reference");
            return updatedTask;
        }

        public async Task<TaskResource> GetTaskAsync(Guid taskId)
        {
            HttpResponseMessage response;
            using (var client = CreateHttpClient().WithOrbitAcceptHeaders())
            {
                string query = string.Format("api/Tasks/{0:D}", taskId);
                response = await client.GetAsync(query);
            }
            response.ThrowOnError("Error getting task with ID {0}", taskId);

            string json = await response.Content.ReadAsStringAsync();
            var task = DeserializeJson<TaskResource>(json);

            return task;
        }

        public async Task<TaskTypeResource> GetCurrentTaskTypeAsync(string ancestryId)
        {
            HttpResponseMessage response;
            using (var client = CreateHttpClient().WithOrbitAcceptHeaders())
            {
                string query = string.Format("api/TaskTypes?ancestorId={0}", ancestryId);
                response = await client.GetAsync(query);
            }
            response.ThrowOnError("Error getting current task type {0}", ancestryId);

            string json = await response.Content.ReadAsStringAsync();
            var taskTypes = DeserializeJson<List<TaskTypeResource>>(json);

            var taskType = taskTypes.FirstOrDefault();

            return taskType;
        }

        public async Task<InspectorResource> GetInspectorAsync(string upn)
        {
            HttpResponseMessage response;
            using (var client = CreateHttpClient().WithOrbitAcceptHeaders())
            {
                string query = string.Format("api/Inspectors?upn={0}", upn);
                response = await client.GetAsync(query);
            }
            response.ThrowOnError("Error getting inspectors with upn {0}", upn);

            string json = await response.Content.ReadAsStringAsync();
            var inspectors = DeserializeJson<List<InspectorResource>>(json);
            var inspector = inspectors.First();

            return inspector;
        }

        HttpClient CreateHttpClient()
        {
            var config = Config.Global;
            var client = new HttpClient { BaseAddress = config.TaskServicesUrl };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", config.ApiKey);
            return client;
        }

        static T DeserializeJson<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new JsonConverter[]
                {
                    new JsonCreationConverter<TaskTypePropertyResource>(new TaskTypePropertyResourceFactory())
                }
            };
            var result = JsonConvert.DeserializeObject<T>(json, settings);
            return result;
        }
    }
}
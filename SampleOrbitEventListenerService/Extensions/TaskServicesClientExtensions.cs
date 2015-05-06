using System;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using NLog;
using SE.Orbit.Services.Utilities;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.Extensions
{
    static class TaskServicesClientExtensions
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        static readonly IsolatedCache<TaskTypeResource> TaskTypeCache = new IsolatedCache<TaskTypeResource>("TaskType");
        static readonly IsolatedCache<InspectorResource> InspectorCache = new IsolatedCache<InspectorResource>("Inspector");

        public static async Task<TaskResource> GetTaskAsync(this TaskServicesClient client, Guid taskId)
        {
            TaskResource task;
            try
            {
                task = await client.Tasks.Get(taskId).GetContentAsync();
            }
            catch (DetailedHttpRequestException e)
            {
                switch (e.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        task = null;
                        break;
                    default:
                        throw;
                }
            }
            return task;
        }

        public static Task<TaskResource> CreateTaskAsync(this TaskServicesClient client, TaskResource task)
        {
            return client.Tasks.Create(task).GetContentAsync();
        }

        public static Task<TaskResource> UpdateTaskAsync(this TaskServicesClient client, TaskResource task)
        {
            return client.Tasks.Update(task).GetContentAsync();
        }

        public static async Task<TaskTypeResource> GetTaskTypeAsync(this TaskServicesClient client, string ancestralName)
        {
            TaskTypeResource taskType = TaskTypeCache.Get(ancestralName);
            if (taskType == null)
            {
                // If taskType is null, then that means we had a cache miss. 
                try
                {
                    taskType = await client.TaskTypes.Get(ancestralName).GetContentAsync();
                }
                catch (DetailedHttpRequestException e)
                {
                    switch (e.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            // The taskType is null which (most likely) means the task type does not
                            // exist in this environment/tenant. In this case, you can simply remove
                            // the non-applicable message handlers from your project. Because we use
                            // this project across tenants, we'll just introduce a 'missing' task type
                            // resource.
                            taskType = CreateMissingTaskTypeResource();
                            Log.Debug("TaskType {0} not found... will be ignored", ancestralName);
                            break;
                        default:
                            throw;
                    }
                }

                // The taskType is non-null, which means we successfully found the task
                // type definition from Orbit Task Services. Cache the value for re-use.
                TaskTypeCache.Set(ancestralName, taskType, CreateCacheItemPolicy());
            }

            // We create a 'missing' task type indicator when the task type doesn't exist
            // to distinguish between a cache miss, but we want to return 'null' to the caller
            if (taskType.ID == Guid.Empty)
            {
                taskType = null;
            }

            return taskType;
        }

        public static async Task<InspectorResource> GetInspectorAsync(this TaskServicesClient client, string upn)
        {
            InspectorResource inspector = InspectorCache.Get(upn);
            if (inspector == null) // cache miss
            {
                inspector = await client.Inspectors.Get(upn).GetContentAsync();

                InspectorCache.Set(upn, inspector, CreateCacheItemPolicy());
            }

            return inspector;
        }

        static TaskTypeResource CreateMissingTaskTypeResource()
        {
            // We can't simply write 'null' to the cache because that will always cause
            // a cache miss and cause an http call to Orbit services. To prevent this
            // scenario, we'll fabricate a fake task type resource that represents 'missing'
            var taskType = new TaskTypeResource
            {
                Name = "Missing",
                ID = Guid.Empty,
            };
            return taskType;
        }

        static CacheItemPolicy CreateCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
        }
    }

}
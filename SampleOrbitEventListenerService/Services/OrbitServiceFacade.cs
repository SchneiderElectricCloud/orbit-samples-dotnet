using System;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using NLog;
using SE.Orbit.Services.Utilities;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.Services
{
    public class OrbitServiceFacade
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly TaskServicesClient _client;
        readonly IsolatedCache<TaskTypeResource> _taskTypeCache;
        readonly IsolatedCache<InspectorResource> _inspectorCache; 

        public OrbitServiceFacade(TaskServicesClient client)
        {
            _client = client;
            _taskTypeCache = new IsolatedCache<TaskTypeResource>("TaskType");
            _inspectorCache = new IsolatedCache<InspectorResource>("Inspector");
        }

        public async Task<TaskResource> GetTaskAsync(Guid taskId)
        {
            TaskResource task;
            try
            {
                task = await _client.Tasks.GetAsync(taskId);
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

        public Task<TaskResource> CreateTaskAsync(TaskResource task)
        {
            return _client.Tasks.PostAsync(task);
        }

        public Task<TaskResource> UpdateTaskAsync(TaskResource task)
        {
            return _client.Tasks.PutAsync(task);
        }

        public async Task<TaskTypeResource> GetTaskTypeAsync(string ancestralName)
        {
            TaskTypeResource taskType = _taskTypeCache.Get(ancestralName);
            if (taskType == null)
            {
                // If taskType is null, then that means we had a cache miss. 
                try
                {
                    taskType = await _client.TaskTypes.GetAsync(ancestralName);
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
                _taskTypeCache.Set(ancestralName, taskType, CreateCacheItemPolicy());
            }

            // We create a 'missing' task type indicator when the task type doesn't exist
            // to distinguish between a cache miss, but we want to return 'null' to the caller
            if (taskType.ID == Guid.Empty)
            {
                taskType = null;
            }

            return taskType;
        }

        public async Task<InspectorResource> GetInspectorAsync(string upn)
        {
            InspectorResource inspector = _inspectorCache.Get(upn);
            if (inspector == null) // cache miss
            {
                inspector = await _client.Inspectors.GetAsync(upn);

                _inspectorCache.Set(upn, inspector, CreateCacheItemPolicy());
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
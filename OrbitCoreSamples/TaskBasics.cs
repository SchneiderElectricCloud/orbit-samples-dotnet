using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using SE.Orbit.TaskServices;
using SE.Orbit.TaskServices.Authentication;
using SE.Orbit.TaskServices.Operations;

namespace OrbitCoreSamples
{
    class TaskBasics
    {
        private readonly TaskServicesClient _client;


        public TaskBasics()
        {
            _client = new TaskServicesClient();
            _client.UseApiKey(new Guid(ConfigurationManager.AppSettings["ApiKey"]));
        }

        public async Task AddCompletedTaskToOrbit()
        {

            TaskTypeResource taskType = await _client.TaskTypes.Get("YOUR TASKTYPE NAME").GetContentAsync();

            TaskResource newTask = new TaskResource
            {
                CompletionDate = new DateTime(2014, 3, 23, 5, 0, 0),
                Status = Status.Completed,
                TaskTypeID = taskType.ID
            };
            newTask.Properties["PropertyName1"] = "PropertyValue1";
            newTask.Properties["PropertyName2"] = "PropertyValue2";

            await _client.Tasks.Create(newTask).GetServiceResponseAsync();

        }

        public async Task GetAllCompletedTasks()
        {
            var completedTasks = await _client.Tasks.GetAll(new GetAllTaskOptions()
            {
                Filter = TaskFilterType.Completed,
                PageSize = 500,
            }).GetContentAsync();

            foreach (var task in completedTasks)
            {
                // Update other systems as need with the task

            }

        }
    }
}

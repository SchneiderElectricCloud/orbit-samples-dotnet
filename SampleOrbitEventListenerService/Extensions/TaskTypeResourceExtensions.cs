using System;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.Extensions
{
    static class TaskTypeResourceExtensions
    {
        public static TaskResource ConstructTask(this TaskTypeResource taskType)
        {
            var task = new TaskResource();
            task.TaskTypeID = taskType.ID;
            task.IsActive = true;
            task.CreatePropertyStubs(taskType);
            return task;
        }
    }
}
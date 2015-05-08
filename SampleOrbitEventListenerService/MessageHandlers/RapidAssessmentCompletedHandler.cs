using System;
using System.Threading.Tasks;
using NLog;
using SampleOrbitEventListenerService.Configuration;
using SampleOrbitEventListenerService.Extensions;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class RapidAssessmentCompletedHandler : IAsyncMessageHandler<TaskCompleted>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly TaskServicesClient _orbit;

        const string SourceTaskTypeName = "RapidAssessment";
        const string TargetTaskTypeName = "DetailedAssessment";

        public RapidAssessmentCompletedHandler(TaskServicesClient orbit)
        {
            _orbit = orbit;
        }

        public async Task Handle(TaskCompleted message)
        {
            TaskTypeResource sourceTaskType = await _orbit.GetTaskTypeAsync(SourceTaskTypeName);
            TaskTypeResource targetTaskType = await _orbit.GetTaskTypeAsync(TargetTaskTypeName);

            if (sourceTaskType != null && targetTaskType != null)
            {
                Config config = Config.Global;
                TaskResource task = await _orbit.GetTaskAsync(message.TaskId);

                // For example: is this a 
                if (task.HasTaskType(sourceTaskType))
                {
                    Log.Debug("Processing completed {0} task: {1}", SourceTaskTypeName, message.TaskId);
                    if (ShouldCreateTargetTaskType(task))
                    {
                        Log.Debug("--> Task needs {0}", TargetTaskTypeName);
                        InspectorResource inspector = await _orbit.GetInspectorAsync(config.AssignToUpn);

                        TaskResource targetTask = targetTaskType.ConstructTask();
                        targetTask.Status = Status.New;
                        targetTask.AssignTo(inspector);
                        targetTask.CopyLocationFrom(task);
                        targetTask.CopyPropertiesFrom(task);

                        TaskResource createdTask = await _orbit.CreateTaskAsync(targetTask);
                        Log.Info("--> Created [{0}] task: {1} ({2})",
                            targetTaskType.DisplayName,
                            createdTask.ID,
                            createdTask.DisplayName);
                    }
                    else
                    {
                        Log.Debug("--> Task does not require {0}", TargetTaskTypeName);
                    }
                }
            }
        }

        bool ShouldCreateTargetTaskType(TaskResource task)
        {
            return true;
        }
    }
}
using System.Threading;
using NLog;
using SampleOrbitEventListenerService.Configuration;
using SampleOrbitEventListenerService.Extensions;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class RapidAssessmentCompletedHandler : IMessageHandler<TaskCompleted>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly TaskServicesApi _api = new TaskServicesApi();

        const string SourceTaskTypeName = "RapidAssessment";
        static TaskTypeResource _sourceTaskType;

        const string TargetTaskTypeName = "DetailedAssessment";
        static TaskTypeResource _targetTaskType;

        public async void Handle(TaskCompleted message)
        {
            EnsureInitialized();
            Config config = Config.Global;
            TaskResource task = await _api.GetTaskAsync(message.TaskId);

            // For example: is this a 
            if (IsSourceTaskType(task))
            {
                Log.Debug("Processing completed {0} task: {1}", SourceTaskTypeName, message.TaskId);
                if (ShouldCreateTargetTaskType(task))
                {
                    Log.Debug("--> Task needs {0}", TargetTaskTypeName);
                    InspectorResource inspector = await _api.GetInspectorAsync(config.AssignToUpn);

                    TaskResource targetTask = _targetTaskType.ConstructTask();
                    targetTask.Status = Status.New;
                    targetTask.AssignTo(inspector);
                    targetTask.CopyLocationFrom(task);
                    targetTask.CopyPropertiesFrom(task);

                    TaskResource createdTask = await _api.CreateTask(targetTask);
                    Log.Info("--> Created [{0}] task: {1} ({2})", 
                        _targetTaskType.DisplayName, createdTask.ID, createdTask.DisplayName);
                }
                else
                {
                    Log.Debug("--> Task does not require {0}", TargetTaskTypeName);
                }
            }
        }

        void EnsureInitialized()
        {
            LazyInitializer.EnsureInitialized(ref _sourceTaskType,
                () => _api.GetCurrentTaskTypeAsync(SourceTaskTypeName).Result);

            LazyInitializer.EnsureInitialized(ref _targetTaskType, 
                () => _api.GetCurrentTaskTypeAsync(TargetTaskTypeName).Result);
        }

        bool IsSourceTaskType(TaskResource task)
        {
            return task != null && _sourceTaskType != null && task.TaskTypeID == _sourceTaskType.ID;
        }

        bool ShouldCreateTargetTaskType(TaskResource task)
        {
            return true;
        }
    }
}
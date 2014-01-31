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
        readonly TaskServicesClient _client;

        const string SourceTaskTypeName = "RapidAssessment";
        static TaskTypeResource _sourceTaskType;

        const string TargetTaskTypeName = "DetailedAssessment";
        static TaskTypeResource _targetTaskType;

        public RapidAssessmentCompletedHandler(TaskServicesClient client)
        {
            _client = client;
        }

        public async void Handle(TaskCompleted message)
        {
            EnsureInitialized();
            Config config = Config.Global;
            TaskResource task = await _client.Tasks.GetAsync(message.TaskId);

            // For example: is this a 
            if (task.HasTaskType(_sourceTaskType))
            {
                Log.Debug("Processing completed {0} task: {1}", SourceTaskTypeName, message.TaskId);
                if (ShouldCreateTargetTaskType(task))
                {
                    Log.Debug("--> Task needs {0}", TargetTaskTypeName);
                    InspectorResource inspector = await _client.Inspectors.GetAsync(config.AssignToUpn);

                    TaskResource targetTask = _targetTaskType.ConstructTask();
                    targetTask.Status = Status.New;
                    targetTask.AssignTo(inspector);
                    targetTask.CopyLocationFrom(task);
                    targetTask.CopyPropertiesFrom(task);

                    TaskResource createdTask = await _client.Tasks.PostAsync(targetTask);
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
                () => _client.TaskTypes.GetAsync(SourceTaskTypeName).Result);

            LazyInitializer.EnsureInitialized(ref _targetTaskType,
                () => _client.TaskTypes.GetAsync(TargetTaskTypeName).Result);
        }

        bool ShouldCreateTargetTaskType(TaskResource task)
        {
            return true;
        }
    }
}
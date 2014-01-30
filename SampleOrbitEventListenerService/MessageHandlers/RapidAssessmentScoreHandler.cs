using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SampleOrbitEventListenerService.BusinessLogic;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class RapidAssessmentScoreHandler : IAsyncMessageHandler<TaskUpdated>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly TaskServicesClient _client;

        const string SourceTaskTypeName = "RapidAssessment";
        static TaskTypeResource _sourceTaskType;

        public RapidAssessmentScoreHandler(TaskServicesClient client)
        {
            _client = client;
        }

        public async Task Handle(TaskUpdated message)
        {
            EnsureInitialized();
            TaskResource task = await _client.Tasks.GetAsync(message.TaskId);

            if (IsSourceTaskType(task)/* && task.IsCompleted()*/)
            {
                var calculator = new RapidAssessmentScoreCalculator();
                int oldScore = calculator.ReadScore(task);

                Log.Debug("Calculating score for task: {0}", message.TaskId);
                int newScore = calculator.CalculateScore(task);

                // WARN: only update the task if the score actually changed or
                // this could easily result in an infinite loop!
                if (newScore != oldScore)
                {
                    calculator.WriteScore(task, newScore);

                    TaskResource updatedTask = await _client.Tasks.PutAsync(task);
                    Log.Info("--> Score has changed... updating task {0}", updatedTask.ID);
                }
                else
                {
                    Log.Debug("--> Score is unchanged");
                }
            }
        }

        void EnsureInitialized()
        {
            LazyInitializer.EnsureInitialized(ref _sourceTaskType,
                () => _client.TaskTypes.GetAsync(SourceTaskTypeName).Result);
        }

        bool IsSourceTaskType(TaskResource task)
        {
            return task != null && _sourceTaskType != null && task.TaskTypeID == _sourceTaskType.ID;
        }
    }
}
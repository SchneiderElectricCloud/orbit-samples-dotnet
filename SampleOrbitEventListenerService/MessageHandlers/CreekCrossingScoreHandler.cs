using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SampleOrbitEventListenerService.BusinessLogic;
using SampleOrbitEventListenerService.Extensions;
using SampleOrbitEventListenerService.Services;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class CreekCrossingScoreHandler : IAsyncMessageHandler<TaskUpdated>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly OrbitServiceFacade _orbit;

        const string SourceTaskTypeName = "WastewaterCreekCrossing";

        public CreekCrossingScoreHandler(OrbitServiceFacade orbit)
        {
            _orbit = orbit;
        }

        public async Task Handle(TaskUpdated message)
        {
            TaskTypeResource sourceTaskType = await _orbit.GetTaskTypeAsync(SourceTaskTypeName);
            if (sourceTaskType != null)
            {
                TaskResource task = await _orbit.GetTaskAsync(message.TaskId);
                if (task.HasTaskType(sourceTaskType) && task.IsCompleted())
                {
                    var calculator = new CreekCrossingScoreCalculator();
                    int oldScore = calculator.ReadScore(task);

                    Log.Debug("Calculating score for task: {0}", message.TaskId);
                    int newScore = calculator.CalculateScore(task);

                    // WARN: only update the task if the score actually changed or
                    // this could easily result in an infinite loop!
                    if (newScore != oldScore)
                    {
                        calculator.WriteScore(task, newScore);

                        TaskResource updatedTask = await _orbit.UpdateTaskAsync(task);
                        Log.Info("--> Score has changed... updating task {0}", updatedTask.ID);
                    }
                    else
                    {
                        Log.Debug("--> Score is unchanged");
                    }
                }
            }
        }
    }
}
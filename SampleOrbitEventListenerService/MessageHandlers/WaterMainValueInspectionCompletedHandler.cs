using System;
using System.Threading;
using NLog;
using SampleOrbitEventListenerService.Configuration;
using SampleOrbitEventListenerService.Extensions;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class WaterMainValueInspectionCompletedHandler : 
        IMessageHandler<TaskCompleted>,
        IMessageHandler<TaskUpdated>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly TaskServicesApi _api = new TaskServicesApi();

        const string InspectionTaskTypeName = "WaterMainValveInspection";
        static TaskTypeResource _inspectionTaskType;

        const string RepairTaskTypeName = "WaterMainValveRepair";
        static TaskTypeResource _repairTaskType;

        public async void Handle(TaskUpdated message)
        {
            EnsureInitialized();
            Config config = Config.Global;
            TaskResource task = await _api.GetTaskAsync(message.TaskId);

            // For example: is this a 
            if (IsWaterMainValveInspectionTaskType(task))
            {
                Log.Debug("Processing updated WaterMainValveInspection task: {0}", message.TaskId);
                if (TaskNeedsRepair(task) && TaskIsCompleted(task))
                {
                    Log.Warn("--> Task {0} was updated yet (already) completed and needs repair..." +
                    "a repair task may be required (more information required)", task.ID);
                }
                else
                {
                    Log.Trace("--> Task does not require repair -- taking no further action");
                }
            }
        }

        public async void Handle(TaskCompleted message)
        {
            EnsureInitialized();
            Config config = Config.Global;
            TaskResource task = await _api.GetTaskAsync(message.TaskId);

            // For example: is this a 
            if (IsWaterMainValveInspectionTaskType(task))
            {
                Log.Debug("Processing completed WaterMainValveInspection task: {0}", message.TaskId);
                if (TaskNeedsRepair(task))
                {
                    Log.Debug("--> Task needs repair");
                    InspectorResource inspector = await _api.GetInspectorAsync(config.AssignToUpn);

                    TaskResource targetTask = _repairTaskType.ConstructTask();
                    targetTask.Status = Status.New;
                    targetTask.AssignTo(inspector);
                    targetTask.CopyLocationFrom(task);
                    targetTask.CopyPropertiesFrom(task);

                    TaskResource createdTask = await _api.CreateTask(targetTask);
                    Log.Info("--> Created [{0}] task: {1} ({2})", 
                        _repairTaskType.DisplayName, createdTask.ID, createdTask.DisplayName);
                }
                else
                {
                    Log.Debug("--> Task does not require repair");
                }
            }
        }

        void EnsureInitialized()
        {
            LazyInitializer.EnsureInitialized(ref _inspectionTaskType,
                () => _api.GetCurrentTaskTypeAsync(InspectionTaskTypeName).Result);

            LazyInitializer.EnsureInitialized(ref _repairTaskType, 
                () => _api.GetCurrentTaskTypeAsync(RepairTaskTypeName).Result);
        }

        bool IsWaterMainValveInspectionTaskType(TaskResource task)
        {
            return task.TaskTypeID == _inspectionTaskType.ID;
        }

        bool TaskIsCompleted(TaskResource task)
        {
            return task.Status == Status.Completed;
        }

        bool TaskNeedsRepair(TaskResource task)
        {
            string valveRepairNeeded = task.Properties.Get<string>("ValveRepairNeeded");
            string valveBoxRepairNeeded = task.Properties.Get<string>("ValveBoxRepairNeeded");
            string conditionOfValueBox = task.Properties.Get<string>("ConditionOfValveBox");

            bool needsRepair = (valveRepairNeeded != null 
                || valveBoxRepairNeeded != null
                || conditionOfValueBox == "NP");
            return needsRepair;
        }
    }
}
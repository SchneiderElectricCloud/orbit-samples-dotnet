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
    public class WaterMainValueInspectionCompletedHandler :
        IAsyncMessageHandler<TaskCompleted>,
        IAsyncMessageHandler<TaskUpdated>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly TaskServicesClient _orbit;

        const string InspectionTaskTypeName = "WaterMainValveInspection";
        const string RepairTaskTypeName = "WaterMainValveRepair";

        public WaterMainValueInspectionCompletedHandler(TaskServicesClient orbit)
        {
            _orbit = orbit;
        }

        public async Task Handle(TaskUpdated message)
        {
            TaskTypeResource inspectionTaskType = await _orbit.GetTaskTypeAsync(InspectionTaskTypeName);
            if (inspectionTaskType != null)
            {
                TaskResource task = await _orbit.GetTaskAsync(message.TaskId);

                // For example: is this a 
                if (task.HasTaskType(inspectionTaskType))
                {
                    Log.Debug("Processing updated WaterMainValveInspection task: {0}", message.TaskId);
                    if (TaskNeedsRepair(task) && task.IsCompleted())
                    {
                        Log.Warn("--> Task {0} was updated yet (already) completed and needs repair..." +
                                 "a repair task may be required (more information required)",
                            task.ID);
                    }
                    else
                    {
                        Log.Trace("--> Task does not require repair -- taking no further action");
                    }
                }
            }
        }

        public async Task Handle(TaskCompleted message)
        {
            TaskTypeResource inspectionTaskType = await _orbit.GetTaskTypeAsync(InspectionTaskTypeName);
            TaskTypeResource repairTaskType = await _orbit.GetTaskTypeAsync(RepairTaskTypeName);

            if (inspectionTaskType != null && repairTaskType != null)
            {
                Config config = Config.Global;
                TaskResource task = await _orbit.GetTaskAsync(message.TaskId);

                if (task.HasTaskType(inspectionTaskType))
                {
                    Log.Debug("Processing completed WaterMainValveInspection task: {0}", message.TaskId);
                    if (TaskNeedsRepair(task))
                    {
                        Log.Debug("--> Task needs repair");
                        InspectorResource inspector = await _orbit.GetInspectorAsync(config.AssignToUpn);

                        TaskResource targetTask = repairTaskType.ConstructTask();
                        targetTask.Status = Status.New;
                        targetTask.AssignTo(inspector);
                        targetTask.CopyLocationFrom(task);
                        targetTask.CopyPropertiesFrom(task);

                        TaskResource createdTask = await _orbit.UpdateTaskAsync(targetTask);
                        Log.Info("--> Created [{0}] task: {1} ({2})",
                            repairTaskType.DisplayName,
                            createdTask.ID,
                            createdTask.DisplayName);
                    }
                    else
                    {
                        Log.Debug("--> Task does not require repair");
                    }
                }
            }
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
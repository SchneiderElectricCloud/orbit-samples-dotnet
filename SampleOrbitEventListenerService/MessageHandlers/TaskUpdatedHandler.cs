using System;
using NLog;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class TaskUpdatedHandler : IMessageHandler<TaskUpdated>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void Handle(TaskUpdated message)
        {
            Log.Trace("Detected task updated: {0}", message.TaskId);
        }
    }
}
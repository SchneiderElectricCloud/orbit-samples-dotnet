using System;
using NLog;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class TaskDeletedHandler : IMessageHandler<TaskDeleted>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void Handle(TaskDeleted message)
        {
            Log.Trace("Detected task deleted: {0}", message.TaskId);
        }
    }
}
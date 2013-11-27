using System;
using NLog;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class TaskCreatedHandler : IMessageHandler<TaskCreated>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void Handle(TaskCreated message)
        {
            Log.Trace("Detected task created: {0}", message.TaskId);
        }
    }
}
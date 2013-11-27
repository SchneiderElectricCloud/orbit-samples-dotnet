using System;
using SampleOrbitEventListenerService.Configuration;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService
{
    // TODO: replace this with TaskServicesClient hooked into IoC Container!
    sealed class OrbitApi
    {
        public static OrbitApi Global = new OrbitApi();

        public OrbitApi()
        {
            TaskServices = new TaskServicesClient();
            TaskServices.UseApiKey(Guid.Parse(Config.Global.ApiKey));
        }

        public TaskServicesClient TaskServices { get; private set; }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;
using SampleOrbitEventListenerService.Configuration;
using SE.Orbit.Services.Extensions;
using SE.Orbit.Services.Interfaces;
using SE.Orbit.Services.ServiceBus;
using SE.Orbit.TaskServices;
using SE.Orbit.TaskServices.Authentication;
using TinyIoC;
using Topshelf;

namespace SampleOrbitEventListenerService
{
    class TaskProcessor : ServiceControl
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        SubscriptionClient _client;

        public bool Start(HostControl hostControl)
        {
            // By default, Azure Service Bus will use TCP ports 9350-9354. If the corporate
            // firewall blocks these outgoing ports, then you'll need to instruct the Service
            // Bus connection to use HTTP instead. 
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;

            DisableSslCertificateValidation();

            // Create a (Windows Azure Service Bus) subscription client referencing the
            // configured topic/subscription name. This will be unique for each client.
            // Note: in order to use this overload you will also need to specify the
            // connection string in 'Microsoft.ServiceBus.ConnectionString' app-setting.
            string topic = Config.Global.OrbitEventsTopicName;
            string subscription = Config.Global.OrbitEventsSubscriptionName;
            _client = SubscriptionClient.Create(topic, subscription, ReceiveMode.PeekLock);
            Log.Debug("Using topic/subscription={0}/{1}", topic, subscription);

            // Setup the task services client used to communicate with Orbit services.
            // Since this application needs to run without user-interaction (i.e., as
            // a Windows Service), we use an ApiKey for authentication.
            var serviceClient = new TaskServicesClient();
            serviceClient.UseApiKey(Guid.Parse(Config.Global.ApiKey));
            LogOrbitTaskServicesEndpoint(serviceClient);

            // Register the task services client instance with the IoC container. This
            // allows the IMessageHandler to access it (e.g., using constructor injection).
            TinyIoCContainer container = TinyIoCContainer.Current;
            container.Register(serviceClient);

            // You can use the subscription client's reliable receive loop directly by
            // calling the OnMessage() method. However, in this sample, we use the Orbit
            // teams DispatchingMessageConsumer class that will dispatch the messages
            // to classes that implement IMessageHandler<TEvent>.
            var options = SetupMessageOptions();
            var consumer = new DispatchingMessageConsumer(_client)
                .AutoRegisterHandlers()
                .StartDispatchingMessages(options);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (_client != null)
            {
                // Calling Close on the SubscriptionClient will terminate the
                // reliable receive loop (OnMessage) that is used by the
                // DispatchingMessageConsumer.
                _client.Close();
            }
            return true;
        }

        [Conditional("DEBUG")]
        static void DisableSslCertificateValidation()
        {
            // Only disable SSL certificate validation for development purposes... this will
            // prevent the HttpClient from throwing an exception when calling an https endpoint
            // using a self-signed certificate.
            Log.Warn("Disabled SSL certificate validation (for development only)!");
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
        }

        static OnMessageOptions SetupMessageOptions()
        {
            // See the Windows Azure Service Bus documentation for details on the
            // the OnMessageOptions properties.
            var options = new OnMessageOptions
            {
                AutoComplete = true,
                MaxConcurrentCalls = 1
            };

            options.ExceptionReceived += (sender, e) =>
            {
                if (e != null && e.Exception != null)
                {
                    Log.ErrorException("Exception while processing event", e.Exception);
                }
            };
            return options;
        }

        static void LogOrbitTaskServicesEndpoint(TaskServicesClient serviceClient)
        {
            if (serviceClient == null) throw new ArgumentNullException("serviceClient");

            try
            {
                // ReSharper disable once CSharpWarnings::CS0618
                IPHostEntry entry = Dns.Resolve(serviceClient.Client.BaseAddress.Host);
                if (entry != null && entry.AddressList != null && entry.AddressList.Length > 0)
                {
                    Log.Trace("Resolved host {0} to {1}", entry.HostName, entry.AddressList[0]);
                }
                else
                {
                    Log.Warn("Failed to resolve host: {0}", serviceClient.Client.BaseAddress.Host);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error resolving Orbit task services host", e);
            }
        }
    }
}
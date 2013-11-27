using System;
using System.Diagnostics;
using System.Net;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;
using SampleOrbitEventListenerService.Configuration;
using SE.Orbit.Services.ServiceBus;
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
            Log.Debug("Settings: TaskServicesUrl={0}", Config.Global.TaskServicesUrl);
            Log.Debug("Settings: Topic/Subscription={0}/{1}", topic, subscription);

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
                    Console.WriteLine("Error processing message: {0}", e);
                }
            };
            return options;
        }
    }
}
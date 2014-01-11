using System;
using System.Threading;
using NLog;
using Topshelf;

namespace SampleOrbitEventListenerService
{
    class Program
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [MTAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Thread.CurrentThread.Name = "Main Thread";
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            HostFactory.Run(x =>
            {
                x.UseNLog();
                x.Service(() => new TaskProcessor());
                x.SetServiceName("OrbitEventListener");
                x.SetDisplayName("Orbit Event Listener Service");
                x.SetDescription("Listens and reacts to orbit events");

                x.StartAutomaticallyDelayed();
                x.RunAsNetworkService();
            });

            LogManager.Flush();
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.ErrorException("Unhandled exception", e.ExceptionObject as Exception);
        }
    }
}

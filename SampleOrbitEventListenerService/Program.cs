using System;
using System.Diagnostics;
using System.Reflection;
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
            Thread.CurrentThread.Name = "Main Thread";
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            AdviseToAppDomainEvents();

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

        static void AdviseToAppDomainEvents()
        {
            Log.Debug("Listening for AppDomain events...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs e)
        {
            Debug.Assert(e != null);
            if (!e.Name.Contains(".resources") && !e.Name.Contains(".XmlSerializers"))
            {
                Log.Warn("AssemblyResolve: {0} RequestedBy: {1}", e.Name, 
                    (e.RequestingAssembly != null) ? e.RequestingAssembly.FullName : null);
            }

            return null;
        }

        static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                Log.Error("Unhandled exception", exception);
            }
        }
    }
}

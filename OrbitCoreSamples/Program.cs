using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitCoreSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskBasics p = new TaskBasics();

            Task.Run((Func<Task>)p.GetAllCompletedTasks).Wait();
            Task.Run((Func<Task>)p.AddCompletedTaskToOrbit).Wait();

        }

        
    }
}

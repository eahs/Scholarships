using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scholarships.Services
{
    /*
        With this injected as Tasks, controllers simply call

        Tasks.Register(SomeAsyncMethod(Tasks.ApplicationStopping));     * 
    */
    public class TaskRegistry : ITaskRegistry, IDisposable
    {
        public CancellationToken ApplicationStopping { get; }
        private readonly CountdownEvent counter = new CountdownEvent(1);

        public TaskRegistry(IHostApplicationLifetime app)
        {
            ApplicationStopping = app.ApplicationStopping;
            ApplicationStopping.Register(() =>
            {
                counter.Signal();
                counter.Wait();
            });
        }

        public void Register(Task task)
        {
            if (task.Status == TaskStatus.Created)
            {
                throw new InvalidOperationException();
            }
            counter.AddCount();
            task.ContinueWith(t => counter.Signal());
        }

        public void Dispose()
        {
            counter.Dispose();
        }
    }
}

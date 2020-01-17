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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    counter.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}

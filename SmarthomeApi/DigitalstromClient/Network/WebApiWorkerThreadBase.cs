using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalstromClient.Network
{
    public class ErrorOccuredEventArgs : EventArgs
    {
        public Exception Error { get; set; }
    }

    public class ApiEventRaisedEventArgs<TApiEvent> : EventArgs
    {
        public TApiEvent ApiEvent { get; set; }
    }

    public abstract class WebApiWorkerThreadBase<TApiEvent> : IDisposable
    {
        public event EventHandler<ErrorOccuredEventArgs> ErrorOccured;

        public event EventHandler<ApiEventRaisedEventArgs<TApiEvent>> ApiEventRaised;

        private Task eventWorkerThread = null;

        private BlockingCollection<Func<Task>> actionQueue = new BlockingCollection<Func<Task>>();
        private Task actionWorkerThread = null;

        private CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private CancellationToken cancellationToken;

        public WebApiWorkerThreadBase()
        {
            cancellationToken = cancellationSource.Token;
            actionWorkerThread = Task.Factory.StartNew(() => ActionWorkerThread(), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            eventWorkerThread = Task.Factory.StartNew(() => EventWorkerThread(), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        
        protected void QueueAction(Func<Task> action)
        {
            actionQueue.TryAdd(action);
        }

        protected abstract Task<IEnumerable<TApiEvent>> ProcessEventPolling();

        private async void ActionWorkerThread()
        {
            foreach (var action in actionQueue.GetConsumingEnumerable())
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    OnErrorOccured(ex, actionWorkerThread);
                    return;
                }
            }
        }

        private async void EventWorkerThread()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var apiEvent in await ProcessEventPolling())
                        OnApiEventRaised(apiEvent);
                }
                catch (Exception ex)
                {
                    OnErrorOccured(ex, eventWorkerThread);
                    return;
                }
            }
        }

        private void OnApiEventRaised(TApiEvent eventItem)
        {
            if (ApiEventRaised == null)
                return;

            var eventargs = new ApiEventRaisedEventArgs<TApiEvent>() { ApiEvent = eventItem };
            Task task = eventWorkerThread.ContinueWith((t) => ApiEventRaised(this, eventargs));
        }

        private void OnErrorOccured(Exception exception, Task workerContext)
        {
            if (ErrorOccured == null)
                return;

            var eventargs = new ErrorOccuredEventArgs() { Error = exception };
            Task task = workerContext.ContinueWith((t) => ErrorOccured(this, eventargs));
        }

        #region IDisposable Support
        private bool disposedValue = false;
        
        public void Dispose()
        {
            if (!disposedValue)
            {
                // This ends the ConsumingEnumerable and lets the action worker
                // thread exit the loop and end the thread
                actionQueue.CompleteAdding();

                // This sets the cancellation token and ends the event long polling thread
                cancellationSource.Cancel();

                disposedValue = true;
            }
        }
        #endregion
    }
}
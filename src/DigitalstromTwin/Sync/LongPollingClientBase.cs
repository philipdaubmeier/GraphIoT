using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class ErrorOccuredEventArgs : EventArgs
    {
        public Exception Error { get; set; }
    }

    public class ApiEventRaisedEventArgs<TApiEvent> : EventArgs
    {
        public TApiEvent ApiEvent { get; set; }
    }

    public abstract class LongPollingClientBase<TApiEvent> : IDisposable
    {
        public event EventHandler<ErrorOccuredEventArgs> ErrorOccured;

        public event EventHandler<ApiEventRaisedEventArgs<TApiEvent>> ApiEventRaised;

        private readonly Task eventWorkerThread = null;

        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        public LongPollingClientBase()
        {
            cancellationToken = cancellationSource.Token;
            eventWorkerThread = Task.Factory.StartNew(() => EventWorkerThread(), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected abstract Task<IEnumerable<TApiEvent>> ProcessEventPolling();

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
                    OnErrorOccured(ex);
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

        private void OnErrorOccured(Exception exception)
        {
            if (ErrorOccured == null)
                return;

            var eventargs = new ErrorOccuredEventArgs() { Error = exception };
            Task task = eventWorkerThread.ContinueWith((t) => ErrorOccured(this, eventargs));
        }

        #region IDisposable Support
        private bool isDisposed = false;
        
        public void Dispose()
        {
            if (isDisposed)
                return;

            // This sets the cancellation token and ends the event long polling thread
            cancellationSource.Cancel();

            isDisposed = true;
        }
        #endregion
    }
}
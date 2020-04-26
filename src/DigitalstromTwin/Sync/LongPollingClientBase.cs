using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class ErrorOccuredEventArgs : EventArgs
    {
        public Exception Error { get; set; }

        public ErrorOccuredEventArgs(Exception error) => Error = error;
    }

    public class ApiEventRaisedEventArgs<TApiEvent> : EventArgs
    {
        public TApiEvent ApiEvent { get; set; }

        public ApiEventRaisedEventArgs(TApiEvent apiEvent) => ApiEvent = apiEvent;
    }

    public abstract class LongPollingClientBase<TApiEvent> : IDisposable
    {
        public event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;

        public event EventHandler<ApiEventRaisedEventArgs<TApiEvent>>? ApiEventRaised;

        private readonly Task? eventWorkerThread = null;

        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        protected ExponentialBackoff Backoff { get; } = new ExponentialBackoff();

        protected class ExponentialBackoff
        {
            private const int _initialMs = 200;
            private const int _maxMs = 60000;
            private const double _factor = 1.5d;

            private int _current = _initialMs;

            public void Reset()
            {
                _current = _initialMs;
            }

            public async Task Delay()
            {
                await Task.Delay(_current);

                _current = Math.Min(_maxMs, (int)(_current * _factor));
            }
        }

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

                    Backoff.Reset();
                }
                catch (Exception ex)
                {
                    OnErrorOccured(ex);
                    await Backoff.Delay();
                }
            }
        }

        private void OnApiEventRaised(TApiEvent eventItem)
        {
            if (ApiEventRaised is null || eventWorkerThread is null)
                return;

            var eventargs = new ApiEventRaisedEventArgs<TApiEvent>(eventItem);
            Task task = eventWorkerThread.ContinueWith(t => ApiEventRaised(this, eventargs));
        }

        private void OnErrorOccured(Exception exception)
        {
            if (ErrorOccured is null || eventWorkerThread is null)
                return;

            var eventargs = new ErrorOccuredEventArgs(exception);
            Task task = eventWorkerThread.ContinueWith(t => ErrorOccured(this, eventargs));
        }

        #region IDisposable Support
        private bool isDisposed = false;

        public void Dispose()
        {
            if (isDisposed)
                return;

            // This sets the cancellation token and ends the event long polling thread
            cancellationSource.Cancel();
            cancellationSource.Dispose();

            isDisposed = true;
        }
        #endregion
    }
}
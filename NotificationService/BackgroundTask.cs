namespace NotificationService
{
    public abstract class BackgroundTask : IHostedService, IDisposable
    {
        private Task? _task;
        private readonly CancellationTokenSource _cts = new();

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _task = ExecuteAsync(_cts.Token);
            if (_task.IsCompleted) return _task;
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_task == null) return;
            try { _cts.Cancel(); }
            finally { await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken)); }
        }

        public virtual void Dispose()
        {
            _cts.Cancel();
        }
    }
}

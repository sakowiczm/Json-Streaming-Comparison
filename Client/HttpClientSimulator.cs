using System.Diagnostics;

public class HttpClientSimulator : IDisposable
{
    private readonly int _totalRequests;
    private readonly int _concurrentRequests;
    private readonly TimeSpan _progressInterval;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposed;

    public HttpClientSimulator(int totalRequests, int concurrentRequests, TimeSpan? progressInterval = null)
    {
        _totalRequests = totalRequests;
        _concurrentRequests = concurrentRequests;
        _progressInterval = progressInterval ?? TimeSpan.FromSeconds(1);
    }

    public async Task<SimulationResult> Simulate(
        Func<Task> sendRequest,
        Func<Task<string>>? metricsProvider = null,
        IProgressReporter? progressReporter = null)
    {
        progressReporter ??= new ConsoleProgressReporter();
        _cancellationTokenSource = new CancellationTokenSource();

        int completed = 0;
        var stopwatch = Stopwatch.StartNew();

        // Start progress reporting task
        var progressTask = ReportProgressAsync(
            () => completed,
            stopwatch,
            metricsProvider,
            progressReporter,
            _cancellationTokenSource.Token);

        try
        {
            // Run concurrent requests
            await RunConcurrentRequestsAsync(sendRequest, () => Interlocked.Increment(ref completed));
        }
        finally
        {
            // Stop progress reporting
            _cancellationTokenSource.Cancel();
            await progressTask;

            stopwatch.Stop();
        }

        // Final report
        var result = new SimulationResult(_totalRequests, stopwatch.Elapsed);
        progressReporter.ReportCompletion(result.TotalTime, result.RequestsPerSecond);

        return result;
    }

    private async Task RunConcurrentRequestsAsync(Func<Task> sendRequest, Func<int> incrementCompleted)
    {
        var activeTasks = new HashSet<Task>();
        int started = 0;

        // Start initial concurrent requests
        for (int i = 0; i < Math.Min(_concurrentRequests, _totalRequests); i++)
        {
            activeTasks.Add(sendRequest());
            started++;
        }

        // Wait for requests to complete and start new ones
        while (activeTasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(activeTasks);
            activeTasks.Remove(completedTask);
            incrementCompleted();

            // Start a new request if there are more to process
            if (started < _totalRequests)
            {
                activeTasks.Add(sendRequest());
                started++;
            }
        }
    }

    private async Task ReportProgressAsync(
        Func<int> getCompleted,
        Stopwatch stopwatch,
        Func<Task<string>>? metricsProvider,
        IProgressReporter progressReporter,
        CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_progressInterval, cancellationToken);

                int completed = getCompleted();
                if (completed >= _totalRequests)
                    break;

                string? metrics = null;
                if (metricsProvider != null)
                {
                    try
                    {
                        metrics = await metricsProvider();
                    }
                    catch
                    {
                        metrics = "Error";
                    }
                }

                progressReporter.ReportProgress(completed, _totalRequests, stopwatch.Elapsed, metrics);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cancellationTokenSource?.Dispose();
            _disposed = true;
        }
    }
}

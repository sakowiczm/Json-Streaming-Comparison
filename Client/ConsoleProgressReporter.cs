public class ConsoleProgressReporter : IProgressReporter
{
    public void ReportProgress(int completed, int total, TimeSpan elapsed, string? metrics)
    {
        double percentComplete = (double)completed / total * 100;
        string metricsText = metrics != null ? $" - {metrics}" : "";

        Console.WriteLine($"[{elapsed.TotalSeconds:F0}s] Progress: {completed}/{total} requests completed ({percentComplete:F1}%){metricsText}");
    }

    public void ReportCompletion(TimeSpan totalTime, double requestsPerSecond)
    {
        Console.WriteLine($"[{totalTime.TotalSeconds:F0}s] Completed - {requestsPerSecond:F1} req/s");
    }
}

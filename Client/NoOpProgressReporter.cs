public class NoOpProgressReporter : IProgressReporter
{
    public void ReportProgress(int completed, int total, TimeSpan elapsed, string? metrics)
    {
        // No operation - silent reporter
    }

    public void ReportCompletion(TimeSpan totalTime, double requestsPerSecond)
    {
        // No operation - silent reporter
    }
}

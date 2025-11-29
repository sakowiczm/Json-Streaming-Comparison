public interface IProgressReporter
{
    /// <summary>
    /// Reports progress during simulation
    /// </summary>
    /// <param name="completed">Number of completed requests</param>
    /// <param name="total">Total number of requests</param>
    /// <param name="elapsed">Elapsed time</param>
    /// <param name="metrics">Optional metrics (e.g., memory usage)</param>
    void ReportProgress(int completed, int total, TimeSpan elapsed, string? metrics);

    /// <summary>
    /// Reports completion of simulation
    /// </summary>
    /// <param name="totalTime">Total elapsed time</param>
    /// <param name="requestsPerSecond">Average requests per second</param>
    void ReportCompletion(TimeSpan totalTime, double requestsPerSecond);
}

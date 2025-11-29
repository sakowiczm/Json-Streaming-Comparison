public class SimulationResult
{
    public int TotalRequests { get; init; }
    public TimeSpan TotalTime { get; init; }
    public double RequestsPerSecond { get; init; }

    public SimulationResult(int totalRequests, TimeSpan totalTime)
    {
        TotalRequests = totalRequests;
        TotalTime = totalTime;
        RequestsPerSecond = totalRequests / totalTime.TotalSeconds;
    }
}

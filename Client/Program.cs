using System.Net.Http.Json;

// Parse port number from command line arguments, default to 5130
int portNumber = args.Length > 0 && int.TryParse(args[0], out int port) ? port : 5130;

Console.WriteLine($"Using port: {portNumber}");
Console.Write("Initializing test data...");

var dataSource = new DataSource();

Console.WriteLine("done.");
Console.WriteLine();

async Task runClient(HttpClient client, int requestSize, int delayUs)
{
    requestSize = Random.Shared.Next(2 * requestSize);
    var people = dataSource.GetPeople().Take(requestSize);

    using var content = JsonContent.Create(people);
    using var response = await client.PostAsync($"?delayUs={delayUs}", content);

    var _ = await response.Content.ReadAsStringAsync();
}

int requestSize = 100_000;
int delayUs = 1;
int totalRequests = 1_000;
int concurrentRequests = 400;

// int requestSize = 100_000;
// int delayUs = 10;
// int totalRequests = 5_000;
// int concurrentRequests = 100;

using HttpClient deserializingEndpoint1 = new()
{
    BaseAddress = new Uri($"http://localhost:{portNumber}/classic")
};

// Create metrics provider for memory monitoring
using var metricsClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{portNumber}") };
async Task<string> getMemoryMetrics()
{
    var memResponse = await metricsClient.GetStringAsync("/mem");
    if (long.TryParse(memResponse, out long memoryBytes))
    {
        double memoryMB = memoryBytes / (1024.0 * 1024.0);
        return $"Memory: {memoryMB:F1} MB";
    }
    return "Memory: N/A";
}

Console.WriteLine("Full JSON deserialization server:");

using var simulator1 = new HttpClientSimulator(totalRequests, concurrentRequests);
var result1 = await simulator1.Simulate(
    async () => await runClient(deserializingEndpoint1, requestSize, delayUs),
    getMemoryMetrics,
    new ConsoleProgressReporter());

Console.WriteLine();
Console.WriteLine(new string('_', 80));
Console.WriteLine();

using HttpClient deserializingEndpoint2 = new()
{
    BaseAddress = new Uri($"http://localhost:{portNumber}/streaming")
};

Console.WriteLine("Streaming JSON deserialization server:");

using var simulator2 = new HttpClientSimulator(totalRequests, concurrentRequests);
var result2 = await simulator2.Simulate(
    async () => await runClient(deserializingEndpoint2, requestSize, delayUs),
    getMemoryMetrics,
    new ConsoleProgressReporter());

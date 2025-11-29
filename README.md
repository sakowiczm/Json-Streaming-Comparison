# Json-Streaming-Comparison

Experiment comparing two JSON deserialization approaches:
- **Classic deserialization**: Deserializes entire JSON array into memory before processing
- **Streaming deserialization**: Uses `JsonSerializer.DeserializeAsyncEnumerable<T>` to process JSON items as they arrive

The project demonstrates performance and memory usage differences when handling large JSON payloads under concurrent load.

## Run

```bash
dotnet run --project ./Client --configuration Release -- 5130
dotnet run --project ./Server --configuration Release
```

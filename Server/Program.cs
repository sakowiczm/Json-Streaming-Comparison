using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/mem", () => GC.GetTotalMemory(true));

app.MapPost("/classic", async (Person[] people, [FromQuery] int? delaysUs) =>
{
    TimeSpan delayPerItem = delaysUs is null ? TimeSpan.Zero : TimeSpan.FromMicroseconds(delaysUs.Value);
    TimeSpan pendingDelay = TimeSpan.Zero;
    TimeSpan threshold = TimeSpan.FromMilliseconds(1);

    foreach(var person in people)
    {
        pendingDelay += delayPerItem;
        if(pendingDelay >= threshold)
        {
            await Task.Delay(pendingDelay);
            pendingDelay = TimeSpan.Zero;
        }
    }

    return Results.Ok();
});

app.MapPost("/streaming", async (HttpContext context, [FromQuery] int? delaysUs) =>
{
    using var body = context.Request.Body;

    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    IAsyncEnumerable<Person?> people = JsonSerializer.DeserializeAsyncEnumerable<Person>(body, jsonOptions);

    TimeSpan delayPerItem = delaysUs is null ? TimeSpan.Zero : TimeSpan.FromMicroseconds(delaysUs.Value);
    TimeSpan pendingDelay = TimeSpan.Zero;
    TimeSpan threshold = TimeSpan.FromMilliseconds(1);

    await foreach(var person in people)
    {
        if(person is null) continue;
        pendingDelay += delayPerItem;
        if(pendingDelay >= threshold)
        {
            await Task.Delay(pendingDelay);
            pendingDelay = TimeSpan.Zero;
        }
    }

    return Results.Ok();
});


app.Run();

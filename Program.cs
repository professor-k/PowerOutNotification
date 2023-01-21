using System.Collections.Concurrent;
using PowerOutNotification;

var timeStamps = new ConcurrentDictionary<string, DateTime>();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(timeStamps);
builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/powerOn", (string address) =>
{
    timeStamps[address] = DateTime.UtcNow;
});

app.Run();
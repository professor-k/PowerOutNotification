using System.Collections.Concurrent;
using PowerOutNotification;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var timeStamps = new ConcurrentDictionary<string, DateTime>();
var telegramBotKey = builder.Configuration["TelegramBotKey"];

builder.Services.AddSingleton(timeStamps);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton(new TelegramBotClient(telegramBotKey));

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/powerOn", (string address) =>
{
    timeStamps[address] = DateTime.UtcNow;
    return DateTime.UtcNow.ToLongTimeString();
});

app.Run();
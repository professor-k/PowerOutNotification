using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PowerOutNotification;

public class Worker : IHostedService, IDisposable
{
    private readonly ConcurrentDictionary<string, DateTime> timeStamps;
    private readonly TelegramBotClient bot;
    private readonly string channelId;
    private readonly ConcurrentDictionary<string, bool> lastStates = new();
    private readonly TimeSpan threshold = TimeSpan.FromSeconds(30);
    private readonly TimeSpan runSpan = TimeSpan.FromSeconds(30);
    private Timer? timer;

    public Worker(ConcurrentDictionary<string, DateTime> timeStamps, TelegramBotClient bot, IConfiguration config)
    {
        this.timeStamps = timeStamps;
        this.bot = bot;
        channelId = config["TelegramChannelId"];
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        timer = new Timer(DoWork, null, TimeSpan.Zero, runSpan);
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        foreach (var timeStamp in timeStamps)
        {
            bool currentState = DateTime.UtcNow - timeStamp.Value < threshold;
            if (!lastStates.TryGetValue(timeStamp.Key, out var lastState) || currentState != lastState)
            {
                await bot.SendTextMessageAsync(
                    new ChatId(channelId),
                    text: $"{timeStamp.Key}: Електрика {(currentState ? "зʼявилась" : "зникла")}",
                    cancellationToken: CancellationToken.None);
            }
            lastStates[timeStamp.Key] = currentState;
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}
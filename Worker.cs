using System.Collections.Concurrent;

namespace PowerOutNotification;

public class Worker : IHostedService, IDisposable
{
    private readonly ConcurrentDictionary<string, DateTime> timeStamps;
    private readonly ConcurrentDictionary<string, bool> lastStates = new();
    private readonly TimeSpan threshold = TimeSpan.FromSeconds(10);
    private readonly TimeSpan runSpan = TimeSpan.FromSeconds(10);
    private Timer? timer;

    public Worker(ConcurrentDictionary<string, DateTime> timeStamps)
    {
        this.timeStamps = timeStamps;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        timer = new Timer(DoWork, null, TimeSpan.Zero, runSpan);
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        foreach (var timeStamp in timeStamps)
        {
            bool currentState = DateTime.UtcNow - timeStamp.Value < threshold;
            if (!lastStates.TryGetValue(timeStamp.Key, out var lastState) || currentState != lastState)
            {
                // TODO: logger.Log(LogLevel.Information, $"{timeStamp.Key}: {currentState}");
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
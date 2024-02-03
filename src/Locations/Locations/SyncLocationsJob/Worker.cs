using System.Diagnostics;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.Services;

namespace TL.Locations.Locations.SyncLocationsTool;

public sealed class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var syncer = scope.ServiceProvider.GetRequiredService<SyncLocationsService>();

            using var logWatcher = _logger.BeginLogWatch("Sync locations");
            await syncer.Sync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Execution error");
        }
    }
}

public static class LoggerExtensions
{
    public static LogWatcher BeginLogWatch(this ILogger logger, string operationName)
    {
        return new LogWatcher(logger, operationName);
    }
}

public sealed class LogWatcher : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch = new();

    public LogWatcher(ILogger logger, string operationName)
    {
        _logger = logger;
        _operationName = operationName;
        Start();
    }

    public void Dispose()
    {
        Stop();
    }

    private void Start()
    {
        _logger.LogInformation("Begin operation {OperationName}", _operationName);
        _stopwatch.Start();
    }

    private void Stop()
    {
        _stopwatch.Stop();
        _logger.LogInformation("End operation {OperationName}", _operationName);
    }
}
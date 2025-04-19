using Garmin.Notifier.Email;
using Garmin.Notifier.Notifiers;

namespace Garmin.Notifier;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEmailReader _emailReader;
    private readonly ILogger<Worker> _logger;
    private DateTime _lastRun = DateTime.MinValue;

    public Worker(
        IServiceProvider serviceProvider,
        IEmailReader emailReader,
        ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _emailReader = emailReader;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _lastRun = DateTime.Now;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var isAvailable = await _emailReader.IsEmailAvailable(stoppingToken, _lastRun);

            if (isAvailable)
            {
                var url = _emailReader.GetUrl();
                if (string.IsNullOrEmpty(url))
                {
                    _logger.LogError($"url is null or empty");
                    await Wait(stoppingToken);
                    continue;
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    _logger.LogError($"url is not valid: {url}");
                    await Wait(stoppingToken);
                    continue;
                }
                _logger.LogInformation($"url: {url}");

                var notifiers = GetNotifiers();
                foreach (var notifier in notifiers)
                {
                    await notifier.Notify(url, stoppingToken);
                }
            }

            _lastRun = DateTime.Now;
            _logger.LogInformation("No new emails found, waiting for 1 minute...");
            await Wait(stoppingToken);
        }
    }

    private IEnumerable<INotifier> GetNotifiers()
    {
        using var scope = _serviceProvider.CreateScope();
        var notifiers = scope.ServiceProvider.GetServices<INotifier>();
        return notifiers;
    }
    
    private async Task Wait(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
}

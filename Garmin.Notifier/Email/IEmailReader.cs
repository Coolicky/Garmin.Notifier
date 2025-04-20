namespace Garmin.Notifier.Email;

public interface IEmailReader
{
    Task<bool> Connect();
    Task<bool> IsEmailAvailable(CancellationToken stoppingToken, DateTime lastRun);
    string? GetUrl();
}
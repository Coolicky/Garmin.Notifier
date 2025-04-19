namespace Garmin.Notifier.Email;

public interface IEmailReader
{
    Task<bool> IsEmailAvailable(CancellationToken stoppingToken, DateTime lastRun);
    string? GetUrl();
}
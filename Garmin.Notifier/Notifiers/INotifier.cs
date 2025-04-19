namespace Garmin.Notifier.Notifiers;

public interface INotifier
{
    Task<bool> Notify(string garminUrl, CancellationToken cancellationToken = default);
}